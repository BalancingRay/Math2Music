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

            Dictionary<double, bool[]> notesUsings = new Dictionary<double, bool[]>();

            foreach (var sequention in input)
            {
                TimeSpan currentTime = TimeSpan.Zero;
                foreach (var note in sequention.Tones)
                {
                    if (note.BaseTone > 0) // Skip silence (0 Hz)
                    {
                        if (!notesUsings.TryGetValue(note.BaseTone, out var data))
                        {
                            data = new bool[maxProcessedTact];
                            notesUsings[note.BaseTone] = data;
                        }

                        int startPosition = (int)(currentTime / discretization);
                        if (startPosition < maxProcessedTact)
                        {
                            data[startPosition] = true;
                        }
                    }
                    currentTime += note.Duration;
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
