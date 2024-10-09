using MathToMusic.Contracts;

namespace MathToMusic.Inputs
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

                    if (track[i].ObertonFrequencies[0] == 0)
                        Thread.Sleep(track[i].Duration);
                    else
                        Console.Beep((int)track[i].ObertonFrequencies[0], (int)track[i].Duration.TotalMilliseconds);
                }
            }
        }
    }
}
