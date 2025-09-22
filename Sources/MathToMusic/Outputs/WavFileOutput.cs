using MathToMusic.Contracts;

namespace MathToMusic.Outputs
{
    internal class WavFileOutput : ITonesFileOutput
    {
        private const int SampleRate = 44100; // Standard CD quality
        private const short BitsPerSample = 16;
        private const short NumChannels = 2; // Stereo for polyphonic support

        // Stereo positioning constants
        /// <summary>
        /// Minimal shift parameter for stereo positioning. Range: [0.05, 0.5]
        /// Low values (0.05): Subtle stereo separation, centered sound
        /// High values (0.5): Wide stereo separation, more pronounced positioning effect
        /// Current: 0.2 provides good stereo separation without being too extreme
        /// For 2 sequences: coefficients will be 1.0/0.8 and 0.8/1.0
        /// </summary>
        private const double MinimalShift = 0.2;

        /// <summary>
        /// Maximum shift parameter to prevent excessive stereo effects. Range: [0.6, 1.0]
        /// Low values (0.6): Conservative stereo spread, prevents extreme panning
        /// High values (1.0): Allows full stereo separation (complete left/right isolation)
        /// Current: 0.8 provides wide stereo spread while maintaining some center presence
        /// Prevents coefficients like 1.0/0.0 which would completely isolate to one channel
        /// </summary>
        private const double MaximalShift = 0.9;

        // Audio amplitude and scaling constants
        /// <summary>
        /// Base amplitude for tone generation. Range: [0.1, 0.8]
        /// Low values (0.1): Quiet output, safe for mixing but may be inaudible
        /// High values (0.8): Loud output, risk of clipping when mixed
        /// Current: 0.3 provides good balance between audibility and safety
        /// </summary>
        private const double BaseAmplitude = 0.3;

        /// <summary>
        /// Power law exponent for sequence amplitude scaling. Range: [0.5, 1.0]
        /// Low values (0.5): More aggressive scaling, quieter but better separation
        /// High values (1.0): Linear scaling, louder but higher clipping risk
        /// Current: 0.7 preserves dynamic range while preventing clipping
        /// </summary>
        private const double SequenceScalingExponent = 0.7;

        /// <summary>
        /// Minimum amplitude scaling factor to prevent inaudible sounds. Range: [0.05, 0.3]
        /// Low values (0.05): Allows very quiet sounds, may become inaudible
        /// High values (0.3): Ensures audibility but may cause clipping with many sequences
        /// Current: 0.1 ensures sounds remain audible even with many sequences
        /// </summary>
        private const double MinimumScalingFactor = 0.1;

        // Audio normalization constants
        /// <summary>
        /// Normalization activation threshold as fraction of max amplitude. Range: [0.8, 1.0]
        /// Low values (0.8): Aggressive normalization, may reduce natural dynamics
        /// High values (1.0): Only normalize when clipping occurs, preserves dynamics
        /// Current: 0.95 leaves small safety margin while preserving natural sound
        /// </summary>
        private const float NormalizationThreshold = 0.95f;

        /// <summary>
        /// Target peak level after normalization as fraction of max amplitude. Range: [0.7, 0.95]
        /// Low values (0.7): Conservative with large headroom, may sound quiet
        /// High values (0.95): Louder output with minimal headroom, risk of artifacts
        /// Current: 0.9 provides good loudness while maintaining safety margin
        /// </summary>
        private const float NormalizationTargetPeak = 0.9f;

        // Frequency-based amplitude adjustment constants
        /// <summary>
        /// Reference frequency for amplitude scaling in Hz. Range: [440, 1760]
        /// Low values (440): A4, gives more boost to very low frequencies
        /// High values (1760): A6, gives more conservative low frequency boost
        /// Current: 880 (A5) provides balanced frequency response
        /// </summary>
        private const double ReferenceFrequency = 880.0;

        /// <summary>
        /// Amplification factor for frequency-based scaling. Range: [1.5, 3.0]
        /// Low values (1.5): Subtle frequency compensation, flatter response
        /// High values (3.0): Strong low frequency boost, more pronounced effect
        /// Current: 2.0 provides noticeable but not excessive frequency shaping
        /// </summary>
        private const double AmplificationFactor = 2.0;

        /// <summary>
        /// Maximum amplification multiplier to prevent excessive boosting. Range: [2.0, 5.0]
        /// Low values (2.0): Conservative boost, flatter frequency response
        /// High values (5.0): Strong boost for very low frequencies, risk of clipping
        /// Current: 3.0 provides significant low frequency presence without distortion
        /// </summary>
        private const double MaxAmplificationMultiplier = 3.0;

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
                GenerateToneSamples(tone, leftChannel, rightChannel, sampleIndex, toneSamples, sequence?.Timber);
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

            // Calculate dynamic amplitude scaling based on number of sequences
            double sequenceAmplitudeScaling = CalculateSequenceAmplitudeScaling(sequences.Count);

            // Calculate stereo positioning coefficients for all sequences
            var stereoCoefficients = CalculateStereoCoefficients(sequences.Count);

            // Mix all sequences simultaneously with stereo positioning
            for (int seqIndex = 0; seqIndex < sequences.Count; seqIndex++)
            {
                var sequence = sequences[seqIndex];
                var (leftCoeff, rightCoeff) = stereoCoefficients[seqIndex];

                int sampleIndex = 0;
                foreach (var tone in sequence.Tones)
                {
                    int toneSamples = (int)(tone.Duration.TotalSeconds * SampleRate);
                    if (sampleIndex + toneSamples <= totalSamples)
                    {
                        AddToneSamples(tone, leftChannel, rightChannel, sampleIndex, toneSamples, sequenceAmplitudeScaling, leftCoeff, rightCoeff, sequence?.Timber);
                    }
                    sampleIndex += toneSamples;
                }
            }

            // Apply normalization to prevent clipping
            NormalizeAudioData(leftChannel, rightChannel);

            WriteWavFile(filePath, leftChannel, rightChannel, totalSamples);
        }

        private int CalculateTotalSamples(IEnumerable<Sequiention> sequences)
        {
            double maxDurationSeconds = sequences.Max(s => s.TotalDuration.TotalSeconds);
            return (int)(maxDurationSeconds * SampleRate);
        }

        private void GenerateToneSamples(Tone tone, float[] leftChannel, float[] rightChannel, int startIndex, int sampleCount, float[] timberCoefficients = null)
        {
            if (tone.ObertonFrequencies.Length == 0 || tone.ObertonFrequencies[0] == 0) // Rest/silence
            {
                // Leave samples as 0 (silence)
                return;
            }

            // Timber coefficients are now passed directly as parameter
            
            for (int i = 0; i < sampleCount; i++)
            {
                if (startIndex + i < leftChannel.Length)
                {
                    double time = (double)i / SampleRate;
                    float totalSample = 0.0f;

                    // Generate samples for all overtones
                    for (int overtoneIndex = 0; overtoneIndex < tone.ObertonFrequencies.Length; overtoneIndex++)
                    {
                        double frequency = tone.ObertonFrequencies[overtoneIndex];
                        if (frequency > 0)
                        {
                            // Get timber coefficient for this overtone (default to 1.0 if not available)
                            float timberCoeff = 1.0f;
                            if (timberCoefficients != null && overtoneIndex < timberCoefficients.Length)
                            {
                                timberCoeff = timberCoefficients[overtoneIndex];
                            }

                            if (timberCoeff > 0) // Only generate sound for positive coefficients
                            {
                                double amplitude = CalculateAmplitudeForFrequency(frequency, BaseAmplitude) * timberCoeff;
                                float sample = (float)(amplitude * Math.Sin(2 * Math.PI * frequency * time));
                                totalSample += sample;
                            }
                        }
                    }

                    leftChannel[startIndex + i] = totalSample;
                    rightChannel[startIndex + i] = totalSample;
                }
            }
        }

        private void AddToneSamples(Tone tone, float[] leftChannel, float[] rightChannel, int startIndex, int sampleCount, float[] timberCoefficients = null)
        {
            AddToneSamples(tone, leftChannel, rightChannel, startIndex, sampleCount, 1.0, 1.0, 1.0, timberCoefficients);
        }

        private void AddToneSamples(Tone tone, float[] leftChannel, float[] rightChannel, int startIndex, int sampleCount, double additionalScaling, float[] timberCoefficients = null)
        {
            AddToneSamples(tone, leftChannel, rightChannel, startIndex, sampleCount, additionalScaling, 1.0, 1.0, timberCoefficients);
        }

        private void AddToneSamples(Tone tone, float[] leftChannel, float[] rightChannel, int startIndex, int sampleCount, double additionalScaling, double leftCoeff, double rightCoeff, float[] timberCoefficients = null)
        {
            if (tone.ObertonFrequencies.Length == 0 || tone.ObertonFrequencies[0] == 0) // Rest/silence
            {
                return;
            }

            // Timber coefficients are now passed directly as parameter
            // float[] timberCoefficients parameter is already provided

            for (int i = 0; i < sampleCount; i++)
            {
                if (startIndex + i < leftChannel.Length)
                {
                    double time = (double)i / SampleRate;
                    float totalSample = 0.0f;

                    // Generate samples for all overtones
                    for (int overtoneIndex = 0; overtoneIndex < tone.ObertonFrequencies.Length; overtoneIndex++)
                    {
                        double frequency = tone.ObertonFrequencies[overtoneIndex];
                        if (frequency > 0)
                        {
                            // Get timber coefficient for this overtone (default to 1.0 if not available)
                            float timberCoeff = 1.0f;
                            if (timberCoefficients != null && overtoneIndex < timberCoefficients.Length)
                            {
                                timberCoeff = timberCoefficients[overtoneIndex];
                            }

                            if (timberCoeff > 0) // Only generate sound for positive coefficients
                            {
                                double amplitude = CalculateAmplitudeForFrequency(frequency, BaseAmplitude) * additionalScaling * timberCoeff;
                                float sample = (float)(amplitude * Math.Sin(2 * Math.PI * frequency * time));
                                totalSample += sample;
                            }
                        }
                    }

                    // Apply stereo coefficients
                    leftChannel[startIndex + i] += totalSample * (float)leftCoeff;
                    rightChannel[startIndex + i] += totalSample * (float)rightCoeff;
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
            if (frequency <= 0)
                return baseAmplitude;

            // Calculate amplification: lower frequencies get higher multiplier
            // The formula gives higher values for lower frequencies
            double frequencyRatio = ReferenceFrequency / frequency;
            double amplificationMultiplier = 1.0 + Math.Log(frequencyRatio) / Math.Log(AmplificationFactor);

            // Ensure minimum of 1.0 (no reduction for high frequencies)
            amplificationMultiplier = Math.Max(1.0, amplificationMultiplier);

            // Cap the maximum amplification to avoid clipping
            amplificationMultiplier = Math.Min(amplificationMultiplier, MaxAmplificationMultiplier);

            return baseAmplitude * amplificationMultiplier;
        }

        private double CalculateSequenceAmplitudeScaling(int sequenceCount)
        {
            // Scale down amplitude based on number of sequences to prevent clipping
            // Use a more sophisticated scaling that preserves dynamic range
            if (sequenceCount <= 1)
                return 1.0;

            // Conservative scaling: reduces amplitude but not as aggressively as simple division
            // This preserves some dynamic range while preventing clipping
            double scaling = 1.0 / Math.Pow(sequenceCount, SequenceScalingExponent);

            // Ensure minimum scaling to prevent sounds from becoming too quiet
            return Math.Max(scaling, MinimumScalingFactor);
        }

        private void NormalizeAudioData(float[] leftChannel, float[] rightChannel)
        {
            // Find the maximum absolute value in both channels
            float maxSample = 0.0f;

            for (int i = 0; i < leftChannel.Length; i++)
            {
                maxSample = Math.Max(maxSample, Math.Abs(leftChannel[i]));
                maxSample = Math.Max(maxSample, Math.Abs(rightChannel[i]));
            }

            // If audio is already within bounds, no normalization needed
            if (maxSample <= NormalizationThreshold)
                return;

            // Calculate normalization factor to bring peak to target level (leaving headroom)
            float normalizationFactor = NormalizationTargetPeak / maxSample;

            // Apply normalization
            for (int i = 0; i < leftChannel.Length; i++)
            {
                leftChannel[i] *= normalizationFactor;
                rightChannel[i] *= normalizationFactor;
            }
        }

        /// <summary>
        /// Calculates stereo positioning coefficients for multiple sequences.
        /// Distributes sequences across the stereo field from left to right.
        /// </summary>
        /// <param name="sequenceCount">Number of sequences to position</param>
        /// <returns>Array of (leftCoeff, rightCoeff) pairs for each sequence</returns>
        private (double leftCoeff, double rightCoeff)[] CalculateStereoCoefficients(int sequenceCount)
        {
            if (sequenceCount <= 1)
            {
                // Single sequence stays centered
                return new[] { (1.0, 1.0) };
            }

            var coefficients = new (double leftCoeff, double rightCoeff)[sequenceCount];

            // Calculate individual shift between adjacent sequences
            double individualShift = MinimalShift;

            // Check if total shift would exceed maximum and scale down if needed
            double totalRequiredShift = individualShift * (sequenceCount - 1);
            if (totalRequiredShift > MaximalShift)
            {
                individualShift = MaximalShift / (sequenceCount - 1);
            }

            for (int i = 0; i < sequenceCount; i++)
            {
                double leftCoeff, rightCoeff;

                if (sequenceCount == 2)
                {
                    // For 2 sequences: 1.0/0.9 and 0.9/1.0 (per requirements)
                    if (i == 0)
                    {
                        leftCoeff = 1.0;
                        rightCoeff = 1.0 - individualShift;
                    }
                    else
                    {
                        leftCoeff = 1.0 - individualShift;
                        rightCoeff = 1.0;
                    }
                }
                else
                {
                    // For 3+ sequences: distribute evenly from left to right
                    // Calculate position relative to center
                    double centerIndex = (sequenceCount - 1) / 2.0;
                    double positionFromCenter = i - centerIndex;
                    double shift = positionFromCenter * individualShift;

                    // Apply shift to create stereo coefficients
                    // Positive shift = more right, negative shift = more left
                    leftCoeff = 1.0 - Math.Max(0, shift);
                    rightCoeff = 1.0 + Math.Min(0, shift);
                }

                coefficients[i] = (leftCoeff, rightCoeff);
            }

            return coefficients;
        }
    }
}