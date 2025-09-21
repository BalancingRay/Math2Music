using MathToMusic.Contracts;

namespace MathToMusic.Outputs
{
    internal class WavFileOutput : ITonesFileOutput
    {
        private const int SampleRate = 44100; // Standard CD quality
        private const short BitsPerSample = 16;
        private const short NumChannels = 2; // Stereo for polyphonic support
        
        public void Send(IList<Sequiention> input)
        {
            SendAndGetFilePath(input);
        }

        public string? SendAndGetFilePath(IList<Sequiention> input)
        {
            if (input == null || input.Count == 0)
                return null;

            // Create Results directory if it doesn't exist
            string resultsPath = GetResultsPath();
            Directory.CreateDirectory(resultsPath);

            // Generate filename with timestamp
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string filename = input.Count == 1 
                ? $"mono_{timestamp}.wav" 
                : $"poly_{timestamp}.wav";
            string filePath = Path.Combine(resultsPath, filename);

            // Generate WAV file
            if (input.Count == 1)
            {
                GenerateMonophonicWav(input[0], filePath);
            }
            else
            {
                GeneratePolyphonicWav(input, filePath);
            }

            Console.WriteLine($"WAV file saved: {filePath}");
            return filePath;
        }

        private string GetResultsPath()
        {
            // Get the directory where the executable is located
            string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;
            return Path.Combine(exeDirectory, "Results");
        }

        private void GenerateMonophonicWav(Sequiention sequence, string filePath)
        {
            // Calculate total samples needed
            int totalSamples = CalculateTotalSamples(new[] { sequence });
            
            // Generate samples for single channel
            float[] leftChannel = new float[totalSamples];
            float[] rightChannel = new float[totalSamples];
            
            int sampleIndex = 0;
            foreach (var tone in sequence.Tones)
            {
                int toneSamples = (int)(tone.Duration.TotalSeconds * SampleRate);
                GenerateToneSamples(tone, leftChannel, rightChannel, sampleIndex, toneSamples);
                sampleIndex += toneSamples;
            }

            WriteWavFile(filePath, leftChannel, rightChannel, totalSamples);
        }

        private void GeneratePolyphonicWav(IList<Sequiention> sequences, string filePath)
        {
            // Calculate total samples needed (longest sequence)
            int totalSamples = CalculateTotalSamples(sequences);
            
            // Generate mixed samples
            float[] leftChannel = new float[totalSamples];
            float[] rightChannel = new float[totalSamples];
            
            // Mix all sequences simultaneously
            foreach (var sequence in sequences)
            {
                int sampleIndex = 0;
                foreach (var tone in sequence.Tones)
                {
                    int toneSamples = (int)(tone.Duration.TotalSeconds * SampleRate);
                    if (sampleIndex + toneSamples <= totalSamples)
                    {
                        AddToneSamples(tone, leftChannel, rightChannel, sampleIndex, toneSamples);
                    }
                    sampleIndex += toneSamples;
                }
            }

            WriteWavFile(filePath, leftChannel, rightChannel, totalSamples);
        }

        private int CalculateTotalSamples(IEnumerable<Sequiention> sequences)
        {
            double maxDurationSeconds = sequences.Max(s => s.TotalDuration.TotalSeconds);
            return (int)(maxDurationSeconds * SampleRate);
        }

        private void GenerateToneSamples(Tone tone, float[] leftChannel, float[] rightChannel, int startIndex, int sampleCount)
        {
            if (tone.ObertonFrequencies[0] == 0) // Rest/silence
            {
                // Leave samples as 0 (silence)
                return;
            }

            double frequency = tone.ObertonFrequencies[0];
            double amplitude = CalculateAmplitudeForFrequency(frequency, 0.3); // Lower tones get higher amplitude
            
            for (int i = 0; i < sampleCount; i++)
            {
                if (startIndex + i < leftChannel.Length)
                {
                    double time = (double)i / SampleRate;
                    float sample = (float)(amplitude * Math.Sin(2 * Math.PI * frequency * time));
                    
                    leftChannel[startIndex + i] = sample;
                    rightChannel[startIndex + i] = sample;
                }
            }
        }

        private void AddToneSamples(Tone tone, float[] leftChannel, float[] rightChannel, int startIndex, int sampleCount)
        {
            if (tone.ObertonFrequencies[0] == 0) // Rest/silence
            {
                return;
            }

            double frequency = tone.ObertonFrequencies[0];
            double baseAmplitude = 0.3 / Math.Sqrt(2); // Reduce amplitude for mixing to avoid clipping
            double amplitude = CalculateAmplitudeForFrequency(frequency, baseAmplitude); // Lower tones get higher amplitude
            
            for (int i = 0; i < sampleCount; i++)
            {
                if (startIndex + i < leftChannel.Length)
                {
                    double time = (double)i / SampleRate;
                    float sample = (float)(amplitude * Math.Sin(2 * Math.PI * frequency * time));
                    
                    leftChannel[startIndex + i] += sample;
                    rightChannel[startIndex + i] += sample;
                }
            }
        }

        private void WriteWavFile(string filePath, float[] leftChannel, float[] rightChannel, int totalSamples)
        {
            using var fileStream = new FileStream(filePath, FileMode.Create);
            using var writer = new BinaryWriter(fileStream);

            // WAV Header
            int dataSize = totalSamples * NumChannels * (BitsPerSample / 8);
            int fileSize = 44 + dataSize - 8; // Header size (44 bytes) + data size - 8

            // RIFF Header
            writer.Write("RIFF".ToCharArray());
            writer.Write(fileSize);
            writer.Write("WAVE".ToCharArray());

            // Format chunk
            writer.Write("fmt ".ToCharArray());
            writer.Write(16); // Format chunk size
            writer.Write((short)1); // PCM format
            writer.Write(NumChannels);
            writer.Write(SampleRate);
            writer.Write(SampleRate * NumChannels * (BitsPerSample / 8)); // Byte rate
            writer.Write((short)(NumChannels * (BitsPerSample / 8))); // Block align
            writer.Write(BitsPerSample);

            // Data chunk
            writer.Write("data".ToCharArray());
            writer.Write(dataSize);

            // Write audio data
            for (int i = 0; i < totalSamples; i++)
            {
                // Convert float samples to 16-bit PCM
                short leftSample = (short)(Math.Max(-1.0f, Math.Min(1.0f, leftChannel[i])) * short.MaxValue);
                short rightSample = (short)(Math.Max(-1.0f, Math.Min(1.0f, rightChannel[i])) * short.MaxValue);
                
                writer.Write(leftSample);
                writer.Write(rightSample);
            }
        }

        private double CalculateAmplitudeForFrequency(double frequency, double baseAmplitude)
        {
            // Lower frequencies get higher amplitude to make them more intensive
            // Using logarithmic scaling where lower frequencies get more boost
            const double referenceFrequency = 880.0; // A5 as reference (high frequency)
            const double amplificationFactor = 2.0;
            
            if (frequency <= 0)
                return baseAmplitude;
                
            // Calculate amplification: lower frequencies get higher multiplier
            // The formula gives higher values for lower frequencies
            double frequencyRatio = referenceFrequency / frequency;
            double amplificationMultiplier = 1.0 + Math.Log(frequencyRatio) / Math.Log(amplificationFactor);
            
            // Ensure minimum of 1.0 (no reduction for high frequencies)
            amplificationMultiplier = Math.Max(1.0, amplificationMultiplier);
            
            // Cap the maximum amplification to avoid clipping
            amplificationMultiplier = Math.Min(amplificationMultiplier, 3.0);
            
            return baseAmplitude * amplificationMultiplier;
        }
    }
}