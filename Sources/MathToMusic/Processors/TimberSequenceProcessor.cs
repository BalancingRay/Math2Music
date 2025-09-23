using MathToMusic.Contracts;

namespace MathToMusic.Processors
{
    /// <summary>
    /// Sequence processor that applies timber profiles to sequences by calculating overtones and coefficients
    /// </summary>
    public class TimberSequenceProcessor : ISequenceProcessor
    {
        float[] timber;
        public TimberSequenceProcessor(float[] timber)
        {
            this.timber = timber;
        }
        /// <summary>
        /// Process a single sequence applying the timber profile
        /// </summary>
        public Sequiention Process(Sequiention sequence)
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
        public IList<Sequiention> Process(IList<Sequiention> sequences)
        {
            if (sequences == null)
                throw new ArgumentNullException(nameof(sequences));

            var processedSequences = new List<Sequiention>();

            foreach (var sequence in sequences)
            {
                processedSequences.Add(Process(sequence));
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
    }
}