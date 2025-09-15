using MathToMusic.Contracts;
using MathToMusic.Models;
using MathToMusic.Utils;

namespace MathToMusic
{
    public class SingleTrackProcessor : ITonesProcessor
    {
        double baseToneHz = 180;
        int baseDurationMilliseconds = 300;

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
        };

        IList<Sequiention> ITonesProcessor.Process(string numericSequention, NumberFormats outputFormat, NumberFormats inputFormat)
        {
            var track = new List<Tone>();
            if (inputFormat == outputFormat)
            {
                for (var i = 0; i < numericSequention.Length; i++)
                {
                    if (toneMap.TryGetValue(numericSequention[i], out var tone))
                    {
                        track.Add(new Tone(baseToneHz * tone, baseDurationMilliseconds));
                    }
                }
            }
            else if (inputFormat is NumberFormats.Bin
                && outputFormat is not NumberFormats.Dec)
            {
                // Use the NumberConverter utility to convert binary with grouping
                var convertedChars = NumberConverter.ConvertBinaryWithGrouping(numericSequention, outputFormat);
                foreach (var convertedChar in convertedChars)
                {
                    if (toneMap.TryGetValue(convertedChar, out var convertedTone))
                    {
                        track.Add(new Tone(baseToneHz * convertedTone, baseDurationMilliseconds));
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
