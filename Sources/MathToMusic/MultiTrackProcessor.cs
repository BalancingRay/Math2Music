using MathToMusic.Contracts;
using MathToMusic.Models;

namespace MathToMusic
{
    public class MultiTrackProcessor_AI_Default_Implementation : ITonesProcessor
    {
        private readonly SingleTrackProcessor _singleTrackProcessor = new SingleTrackProcessor();

        public IList<Sequiention> Process(string numericSequention, NumberFormats outputFormat, NumberFormats inputFormat)
        {
            // Create multiple tracks by splitting the input
            // For demo: split input in half and create two sequences with different base frequencies
            if (string.IsNullOrEmpty(numericSequention) || numericSequention.Length < 2)
            {
                // Fall back to single track
                return ((ITonesProcessor)_singleTrackProcessor).Process(numericSequention, outputFormat, inputFormat);
            }

            int midPoint = numericSequention.Length / 2;
            string firstHalf = numericSequention.Substring(0, midPoint);
            string secondHalf = numericSequention.Substring(midPoint);

            // Create two tracks with different characteristics
            var track1 = CreateTrack(firstHalf, outputFormat, inputFormat, 180, "Track1 - Low"); // Base frequency
            var track2 = CreateTrack(secondHalf, outputFormat, inputFormat, 360, "Track2 - High"); // Higher frequency

            var result = new List<Sequiention>();
            if (track1 != null) result.Add(track1);
            if (track2 != null) result.Add(track2);

            return result;
        }

        private Sequiention? CreateTrack(string input, NumberFormats outputFormat, NumberFormats inputFormat, double baseFreq, string title)
        {
            if (string.IsNullOrEmpty(input))
                return null;

            var singleTrackResult = ((ITonesProcessor)_singleTrackProcessor).Process(input, outputFormat, inputFormat);
            if (singleTrackResult.Count == 0)
                return null;

            var originalTrack = singleTrackResult[0];
            
            // Adjust frequencies for this track
            var adjustedTones = new List<Tone>();
            foreach (var tone in originalTrack.Tones)
            {
                if (tone.ObertonFrequencies[0] == 0)
                {
                    // Keep silence as is
                    adjustedTones.Add(tone);
                }
                else
                {
                    // Adjust frequency using our base frequency
                    double multiplier = tone.ObertonFrequencies[0] / 180; // Original base was 180
                    double newFrequency = baseFreq * multiplier;
                    adjustedTones.Add(new Tone(newFrequency, (int)tone.Duration.TotalMilliseconds));
                }
            }

            return new Sequiention
            {
                Tones = adjustedTones,
                TotalDuration = TimeSpan.FromMilliseconds(adjustedTones.Sum(t => t.Duration.TotalMilliseconds)),
                Title = title
            };
        }
    }
}