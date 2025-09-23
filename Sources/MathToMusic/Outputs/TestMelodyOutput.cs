using MathToMusic.Contracts;
using System.Text;

namespace MathToMusic.Outputs
{
    public class TestMelodyOutput : ITonesOutput
    {
        private readonly TimeSpan discretization;
        private readonly TimeSpan traseLimitation;

        public TestMelodyOutput(double discretizationMilliseconds, double traseLimitationSec = 60)
        {
            this.discretization = TimeSpan.FromMilliseconds(discretizationMilliseconds);
            this.traseLimitation = TimeSpan.FromSeconds(traseLimitationSec);
        }

        public void Send(IList<Sequiention> input)
        {
            var result = GetMelodyTracing(input);
            Console.WriteLine(result);
        }

        public string GetMelodyTracing(IList<Sequiention> input)
        {
            var durationLimit = input.Max(i => i.TotalDuration);
            if (traseLimitation < durationLimit)
            {
                durationLimit = traseLimitation;
            }
            int maxProcessedTact = (int)(durationLimit / discretization) + 1;

            Dictionary<string, bool[]> notesUsings = new Dictionary<string, bool[]>();

            foreach (var sequention in input)
            {
                // For each tone sequence, calculate the position based on base duration intervals
                // rather than accumulating actual tone durations
                for (int i = 0; i < sequention.Tones.Count; i++)
                {
                    var note = sequention.Tones[i];
                    if (note.BaseTone > 0) // Skip silence (0 Hz)
                    {
                        string toneKey = ((int)note.BaseTone).ToString();
                        if (!notesUsings.TryGetValue(toneKey, out var data))
                        {
                            data = new bool[maxProcessedTact];
                            notesUsings[toneKey] = data;
                        }

                        // Calculate position based on tone index, not accumulated duration
                        int position = (int)((i * discretization.TotalMilliseconds) / discretization.TotalMilliseconds);
                        if (position < maxProcessedTact)
                        {
                            data[position] = true;
                        }
                    }
                }
            }

            var builder = new StringBuilder();
            foreach (var item in notesUsings.ToList().OrderBy(i => i.Key))
            {
                builder.Append($"{item.Key:F0}:");
                builder.Append(string.Join("", item.Value.Select(i => i ? '!' : '.')));
                builder.AppendLine();
            }
            return builder.ToString();
        }
    }
}
