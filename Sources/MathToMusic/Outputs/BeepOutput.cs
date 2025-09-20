using MathToMusic.Contracts;

namespace MathToMusic.Outputs
{
    internal class BeepOutput : ITonesOutput
    {
        public void Send(IList<Sequiention> input)
        {
            if (input.Count == 1)
            {
                var track = input[0].Tones;
                for (var i = 0; i < track.Count; i++)
                {
                    var tone = track[i];

                    // Handle chords (multiple frequencies) by playing them in quick succession
                    if (tone.ObertonFrequencies?.Length > 1)
                    {
                        // Calculate duration per frequency for chords
                        int durationPerFreq = Math.Max(50, (int)(tone.Duration.TotalMilliseconds / tone.ObertonFrequencies.Length));

                        foreach (var frequency in tone.ObertonFrequencies)
                        {
                            if (frequency == 0)
                                Thread.Sleep(durationPerFreq);
                            else
                                Console.Beep((int)frequency, durationPerFreq);
                        }
                    }
                    else if (tone.ObertonFrequencies?.Length == 1)
                    {
                        // Single frequency
                        if (tone.ObertonFrequencies[0] == 0)
                            Thread.Sleep(tone.Duration);
                        else
                            Console.Beep((int)tone.ObertonFrequencies[0], (int)tone.Duration.TotalMilliseconds);
                    }
                    else
                    {
                        // No frequencies, just wait
                        Thread.Sleep(tone.Duration);
                    }
                }
            }
        }
    }
}
