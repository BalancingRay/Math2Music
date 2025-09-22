using MathToMusic.Contracts;
using MathToMusic.Models;

namespace MathToMusic.Processors
{
    /// <summary>
    /// Sequence processor that applies timber profiles to sequences by calculating overtones and coefficients
    /// </summary>
    public class TimberSequenceProcessor : ISequenceProcessor<float[]>
    {
        /// <summary>
        /// Process a single sequence applying the timber profile
        /// </summary>
        public Sequiention Process(Sequiention sequence, float[] timber)
        {
            if (sequence == null)
                throw new ArgumentNullException(nameof(sequence));

            if (timber == null || timber.Length == 0)
                return sequence;

            // Create new sequence with processed tones and applied timber
            var processedTones = new List<Tone>();
            
            foreach (var tone in sequence.Tones)
            {
                var processedTone = ProcessTone(tone, timber);
                processedTones.Add(processedTone);
            }

            return new Sequiention
            {
                TotalDuration = sequence.TotalDuration,
                Tones = processedTones,
                Title = sequence.Title,
                Timber = timber // Apply the passed timber to the sequence
            };
        }

        /// <summary>
        /// Process multiple sequences with the same timber
        /// </summary>
        public IList<Sequiention> Process(IList<Sequiention> sequences, float[] timber)
        {
            if (sequences == null)
                throw new ArgumentNullException(nameof(sequences));

            var processedSequences = new List<Sequiention>();
            
            foreach (var sequence in sequences)
            {
                processedSequences.Add(Process(sequence, timber));
            }

            return processedSequences;
        }

        /// <summary>
        /// Process a single tone by generating overtones based on timber profile
        /// </summary>
        private Tone ProcessTone(Tone tone, float[] timberCoefficients)
        {
            // If the tone is silence (0 Hz), return as-is
            if (tone.ObertonFrequencies.Length == 0 || tone.ObertonFrequencies[0] == 0)
                return tone;

            double fundamentalFreq = tone.ObertonFrequencies[0];
            
            // Generate overtones: fundamental + harmonics (2x, 3x, 4x, etc.)
            var overtones = new List<double>();
            
            for (int i = 0; i < timberCoefficients.Length; i++)
            {
                // Generate harmonic frequency (i+1 because harmonic series starts at 1x fundamental)
                double overtoneFreq = fundamentalFreq * (i + 1);
                overtones.Add(overtoneFreq);
            }

            return new Tone
            {
                Duration = tone.Duration,
                ObertonFrequencies = overtones.ToArray()
            };
        }

        /// <summary>
        /// Create a sequence processor for a specific timber profile
        /// </summary>
        public static TimberSequenceProcessor ForProfile(string profileName)
        {
            var processor = new TimberSequenceProcessor();
            return processor;
        }

        /// <summary>
        /// Apply a timber profile to a sequence
        /// </summary>
        public static Sequiention ApplyTimber(Sequiention sequence, string timberProfileName)
        {
            if (sequence == null)
                throw new ArgumentNullException(nameof(sequence));

            var timberProfile = TimberProfiles.GetProfile(timberProfileName);
            if (timberProfile == null)
                throw new ArgumentException($"Unknown timber profile: {timberProfileName}", nameof(timberProfileName));

            var processor = new TimberSequenceProcessor();
            return processor.Process(sequence, timberProfile);
        }

        /// <summary>
        /// Apply timber profiles to multiple sequences
        /// </summary>
        public static IList<Sequiention> ApplyTimber(IList<Sequiention> sequences, string timberProfileName)
        {
            if (sequences == null)
                throw new ArgumentNullException(nameof(sequences));

            return sequences.Select(seq => ApplyTimber(seq, timberProfileName)).ToList();
        }
    }
}