using MathToMusic.Contracts;
using MathToMusic.Models;
using MathToMusic.Utils;

namespace MathToMusic.Processors
{
    public class ReachSingleTrackProcessor : ITonesProcessor
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

        public IList<Sequiention> Process(string numericSequention, NumberFormats outputFormat, NumberFormats inputFormat)
        {
            if (CommonNumbers.Collection.TryGetValue(numericSequention, out string commonNumber))
            {
                numericSequention = commonNumber;
            }
            // Convert input if formats differ

            string processedSequence = numericSequention;
            if (inputFormat != outputFormat)
            {
                if (inputFormat == NumberFormats.Bin && outputFormat != NumberFormats.Dec)
                {
                    var convertedChars = NumberConverter.ConvertBinaryWithGrouping(numericSequention, outputFormat);
                    processedSequence = new string(convertedChars.ToArray());
                }
                else
                {
                    try
                    {
                        processedSequence = NumberConverter.Convert(numericSequention, inputFormat, outputFormat);
                    }
                    catch (ArgumentException)
                    {
                        return new List<Sequiention>();
                    }
                }
            }

            // Get octave groups and their duration multipliers based on the output format
            var octaveGroups = GetOctaveGroups(outputFormat);
            var sequences = new List<Sequiention>();

            // Create a sequence for each octave group
            foreach (var group in octaveGroups)
            {
                var track = new List<Tone>();

                for (var i = 0; i < processedSequence.Length; i++)
                {
                    if (toneMap.TryGetValue(processedSequence[i], out var toneValue))
                    {
                        // Check if this tone belongs to the current octave group
                        if (group.ToneValues.Contains(toneValue))
                        {
                            int duration = baseDurationMilliseconds * group.DurationMultiplier;
                            track.Add(new Tone(baseToneHz * toneValue, duration));
                        }
                        else
                        {
                            // Add silence for tones not in this octave group
                            int duration = baseDurationMilliseconds * group.DurationMultiplier;
                            track.Add(new Tone(0, duration)); // 0 frequency = silence
                        }
                    }
                }

                // Always add a sequence for each octave group, even if empty
                sequences.Add(new Sequiention()
                {
                    Tones = track,
                    TotalDuration = TimeSpan.FromMilliseconds(track.Sum(t => t.Duration.TotalMilliseconds)),
                    Title = $"Octave_{group.Name}"
                });
            }

            return sequences;
        }

        private List<OctaveGroup> GetOctaveGroups(NumberFormats format)
        {
            return format switch
            {
                NumberFormats.Hex => new List<OctaveGroup>
                {
                    new OctaveGroup("Low", new[] { 1 }, 8),
                    new OctaveGroup("MidLow", new[] { 2, 3 }, 4),
                    new OctaveGroup("MidHigh", new[] { 4, 5, 6, 7 }, 2),
                    new OctaveGroup("High", new[] { 8, 9, 10, 11, 12, 13, 14, 15 }, 1)
                },
                NumberFormats.Oct => new List<OctaveGroup>
                {
                    new OctaveGroup("Low", new[] { 1 }, 4),
                    new OctaveGroup("Mid", new[] { 2, 3 }, 2),
                    new OctaveGroup("High", new[] { 4, 5, 6, 7 }, 1)
                },
                NumberFormats.Qad => new List<OctaveGroup>
                {
                    new OctaveGroup("Low", new[] { 1 }, 2),
                    new OctaveGroup("High", new[] { 2, 3 }, 1)
                },
                _ => new List<OctaveGroup>
                {
                    new OctaveGroup("Single", Enumerable.Range(0, 16).ToArray(), 1)
                }
            };
        }

        private class OctaveGroup
        {
            public string Name { get; }
            public int[] ToneValues { get; }
            public int DurationMultiplier { get; }

            public OctaveGroup(string name, int[] toneValues, int durationMultiplier)
            {
                Name = name;
                ToneValues = toneValues;
                DurationMultiplier = durationMultiplier;
            }
        }
    }
}