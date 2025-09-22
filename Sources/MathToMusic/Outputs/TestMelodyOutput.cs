using MathToMusic.Contracts;
using System.Text;

namespace MathToMusic.Outputs
{
    internal class TestMelodyOutput
    {
        private readonly TimeSpan discretization;
        private readonly TimeSpan traseLimitation;

        public TestMelodyOutput(double discretizationMilliseconds, double traseLimitationSec = 60)
        {
            this.discretization = TimeSpan.FromMilliseconds(discretizationMilliseconds);
            this.traseLimitation = TimeSpan.FromSeconds(traseLimitationSec);
        }

        public string GetMelodyTracing(IList<Sequiention> input)
        {
            var durationLimit = input.Max(i => i.TotalDuration);
            if (traseLimitation < durationLimit)
            {
                durationLimit = traseLimitation;
            }
            int maxProcessedTact = (int)(durationLimit / discretization) + 1;

            Dictionary<double, bool[]> notesUsings = new Dictionary<double, bool[]>();

            foreach (var sequention in input)
            {
                int position = 0;
                foreach (var note in sequention.Tones)
                {
                    bool[]? data;
                    if (!notesUsings.TryGetValue(note.BaseTone, out data))
                    {
                        data = new bool[maxProcessedTact];
                    }
                    int length = (int)Math.Round(note.Duration / discretization, 0);

                    data[position] = true;
                    position += length;
                }
            }
            var builder = new StringBuilder();
            foreach (var item in notesUsings.ToList().OrderBy(i => i.Key))
            {
                builder.Append(item.Key);
                builder.Append(item.Value.Select(i => i ? '+' : ' ').ToArray());
                builder.Append(Environment.NewLine);
            }
            return builder.ToString();
        }
    }
}
