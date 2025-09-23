using MathToMusic.Contracts;
using MathToMusic.Models;
using MathToMusic.Utils;

namespace MathToMusic.Processors
{
    public class SingleTrackProcessor : ITonesProcessor
    {
        private const double DefaultBaseToneHz = 180;
        private const int DefaultBaseDurationMilliseconds = 300;
        
        private readonly double baseToneHz;
        private readonly int baseDurationMilliseconds;
        
        public SingleTrackProcessor(int baseDurationMilliseconds = DefaultBaseDurationMilliseconds, double baseToneHz = DefaultBaseToneHz)
        {
            this.baseDurationMilliseconds = baseDurationMilliseconds;
            this.baseToneHz = baseToneHz;
        }

        Dictionary<char, int> toneMap = new Dictionary<char, int>()
        {
            { '0', 0 },
            { '1', 1 },
            { '2', 2 },
            { '3', 3 },
            { '4', 4 },
            { '5', 5 },
            { '6', 6 },
            { '7', 7 },
            { '8', 8 },
            { '9', 9 },
            { 'A', 10},
            { 'B', 11},
            { 'C', 12},
            { 'D', 13},
            { 'E', 14},
            { 'F', 15},
            // Base32 additional characters (G-V for values 16-31)
            { 'G', 16},
            { 'H', 17},
            { 'I', 18},
            { 'J', 19},
            { 'K', 20},
            { 'L', 21},
            { 'M', 22},
            { 'N', 23},
            { 'O', 24},
            { 'P', 25},
            { 'Q', 26},
            { 'R', 27},
            { 'S', 28},
            { 'T', 29},
            { 'U', 30},
            { 'V', 31},
        };

        IList<Sequiention> ITonesProcessor.Process(string numericSequention, NumberFormats outputFormat, NumberFormats inputFormat)
        {
            var track = new List<Tone>();
            if (CommonNumbers.Collection.TryGetValue(numericSequention, out string commonNumber))
            {
                numericSequention = commonNumber;
            }
            if (inputFormat == outputFormat)
            {
                // Same format - process each character directly
                for (var i = 0; i < numericSequention.Length; i++)
                {
                    if (toneMap.TryGetValue(numericSequention[i], out var tone))
                    {
                        track.Add(new Tone(baseToneHz * tone, baseDurationMilliseconds));
                    }
                }
            }
            else
            {
                // Different formats - handle conversions
                if (inputFormat == NumberFormats.Bin && outputFormat != NumberFormats.Dec)
                {
                    // For binary input to non-decimal output, use the specific grouping method to maintain compatibility
                    var convertedChars = NumberConverter.ConvertBinaryWithGrouping(numericSequention, outputFormat);
                    foreach (var convertedChar in convertedChars)
                    {
                        if (toneMap.TryGetValue(convertedChar, out var convertedTone))
                        {
                            track.Add(new Tone(baseToneHz * convertedTone, baseDurationMilliseconds));
                        }
                    }
                }
                else
                {
                    // For all other conversions (including decimal conversions), use the enhanced Convert method
                    try
                    {
                        string convertedSequence = NumberConverter.Convert(numericSequention, inputFormat, outputFormat);

                        // Process each character of the converted sequence
                        for (var i = 0; i < convertedSequence.Length; i++)
                        {
                            if (toneMap.TryGetValue(convertedSequence[i], out var tone))
                            {
                                track.Add(new Tone(baseToneHz * tone, baseDurationMilliseconds));
                            }
                        }
                    }
                    catch (ArgumentException)
                    {
                        // If conversion fails, leave track empty
                    }
                }
            }

            var song = new List<Sequiention>()
            {
                new Sequiention()
                {
                    Tones = track,
                    TotalDuration = TimeSpan.FromMilliseconds(track.Sum(t=> t.Duration.TotalMilliseconds)),
                    Title = "Single"
                }
            };
            return song;
        }
    }
}
