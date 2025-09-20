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
                var lastToneByValue = new Dictionary<int, int>(); // Track last tone position by tone value

                for (var i = 0; i < processedSequence.Length; i++)
                {
                    if (toneMap.TryGetValue(processedSequence[i], out var toneValue))
                    {
                        // Check if this tone belongs to the current octave group
                        if (group.ToneValues.Contains(toneValue))
                        {
                            // Use base duration to maintain timeline synchronization
                            // Duration multiplier affects the "reach" (how long tones sustain when interrupted)
                            int baseDuration = baseDurationMilliseconds;
                            int maxReachDuration = baseDurationMilliseconds * group.DurationMultiplier;

                            // If there was a previous occurrence of the same tone value in this group, 
                            // check if we need to shorten it based on the reach duration
                            if (lastToneByValue.ContainsKey(toneValue))
                            {
                                var previousToneIndex = lastToneByValue[toneValue];
                                if (previousToneIndex >= 0 && previousToneIndex < track.Count)
                                {
                                    var previousTone = track[previousToneIndex];
                                    int positionsSinceLastTone = i - previousToneIndex;
                                    int timeSinceLastTone = positionsSinceLastTone * baseDurationMilliseconds;

                                    // If the current tone (same value) starts before the previous tone's reach would naturally end,
                                    // shorten the previous tone's duration
                                    if (timeSinceLastTone < maxReachDuration)
                                    {
                                        var shortenedTone = new Tone
                                        {
                                            ObertonFrequencies = previousTone.ObertonFrequencies,
                                            Duration = TimeSpan.FromMilliseconds(timeSinceLastTone)
                                        };
                                        track[previousToneIndex] = shortenedTone;
                                    }
                                }
                            }

                            // Calculate duration for this tone: 
                            // Start with base duration, extend up to max reach if no interruption
                            int actualDuration = baseDuration;
                            
                            // Look ahead to see if same tone value appears later
                            int nextSameTonePosition = -1;
                            for (int j = i + 1; j < processedSequence.Length; j++)
                            {
                                if (toneMap.TryGetValue(processedSequence[j], out var futureValue) && 
                                    futureValue == toneValue && group.ToneValues.Contains(futureValue))
                                {
                                    nextSameTonePosition = j;
                                    break;
                                }
                            }
                            
                            if (nextSameTonePosition > 0)
                            {
                                // Calculate time until next same tone
                                int timeUntilNext = (nextSameTonePosition - i) * baseDurationMilliseconds;
                                actualDuration = Math.Min(maxReachDuration, timeUntilNext);
                            }
                            else
                            {
                                // No future same tone, use maximum reach but don't exceed sequence end
                                int remainingTime = (processedSequence.Length - i) * baseDurationMilliseconds;
                                actualDuration = Math.Min(maxReachDuration, remainingTime);
                            }

                            track.Add(new Tone(baseToneHz * toneValue, actualDuration));
                            lastToneByValue[toneValue] = track.Count - 1; // Update last occurrence index for this tone value
                        }
                        else
                        {
                            // Add silence for tones not in this octave group - use base duration only
                            track.Add(new Tone(0, baseDurationMilliseconds)); // 0 frequency = silence
                        }
                    }
                }

                // Calculate total duration as the sum of all base durations (timeline synchronization)
                int totalSequenceDuration = processedSequence.Length * baseDurationMilliseconds;

                sequences.Add(new Sequiention()
                {
                    Tones = track,
                    TotalDuration = TimeSpan.FromMilliseconds(totalSequenceDuration),
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
                NumberFormats.Dec => new List<OctaveGroup>
                {
                    new OctaveGroup("Low", new[] { 1 }, 4),
                    new OctaveGroup("MidLow", new[] { 2, 3 }, 2),
                    new OctaveGroup("High", new[] { 4, 5, 6, 7, 8, 9 }, 1),
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