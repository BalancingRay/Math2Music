using MathToMusic.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MathToMusic.Processors
{
    /// <summary>
    /// Combines multiple sequential tone tracks into harmonic chords
    /// </summary>
    public static class HarmonicCombiner
    {
        /// <summary>
        /// Combine multiple sequences into a single sequence with harmonic chords
        /// For example: sequences [1,2,3] and [4,5,6] become [(1,4), (2,5), (3,6)]
        /// Different length sequences: [1,2] and [3,4,5,6] become [(1,3), (2,4), (5), (6)]
        /// </summary>
        /// <param name="sequences">List of sequences to combine harmonically</param>
        /// <returns>Single sequence with harmonic chords</returns>
        public static Sequiention CombineHarmonically(IList<Sequiention> sequences)
        {
            if (sequences == null || sequences.Count == 0)
                return new Sequiention { Tones = new List<Tone>(), Title = "Empty", TotalDuration = TimeSpan.Zero };

            if (sequences.Count == 1)
            {
                // Single sequence, return as-is but update title
                var singleSeq = sequences[0];
                return new Sequiention
                {
                    Tones = singleSeq.Tones,
                    Title = "Harmonic",
                    TotalDuration = singleSeq.TotalDuration
                };
            }

            // Find the maximum length among all sequences
            int maxLength = sequences.Max(seq => seq.Tones?.Count ?? 0);
            var combinedTones = new List<Tone>();

            // Combine tones at each position
            for (int i = 0; i < maxLength; i++)
            {
                var frequenciesAtPosition = new List<double>();
                TimeSpan maxDuration = TimeSpan.Zero;

                // Collect frequencies from each sequence at position i
                foreach (var sequence in sequences)
                {
                    if (sequence.Tones != null && i < sequence.Tones.Count)
                    {
                        var tone = sequence.Tones[i];
                        // Add all frequencies from this tone (it might already be a chord)
                        if (tone.ObertonFrequencies != null)
                        {
                            frequenciesAtPosition.AddRange(tone.ObertonFrequencies);
                        }
                        
                        // Use the maximum duration among all tones at this position
                        if (tone.Duration > maxDuration)
                            maxDuration = tone.Duration;
                    }
                }

                // Create combined tone with all frequencies
                if (frequenciesAtPosition.Count > 0)
                {
                    var combinedTone = new Tone
                    {
                        ObertonFrequencies = frequenciesAtPosition.ToArray(),
                        Duration = maxDuration
                    };
                    combinedTones.Add(combinedTone);
                }
            }

            // Calculate total duration
            var totalDuration = TimeSpan.FromMilliseconds(combinedTones.Sum(t => t.Duration.TotalMilliseconds));

            return new Sequiention
            {
                Tones = combinedTones,
                Title = "Harmonic",
                TotalDuration = totalDuration
            };
        }
    }
}