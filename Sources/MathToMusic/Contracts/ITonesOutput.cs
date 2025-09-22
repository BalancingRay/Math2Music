namespace MathToMusic.Contracts
{
    public interface ITonesOutput
    {
        void Send(IList<Sequiention> input);
    }

    public struct Tone
    {
        public TimeSpan Duration { get; set; }
        public double[] ObertonFrequencies { get; set; }

        /// <summary>
        /// Single tone
        /// </summary>
        /// <param name="frequency"></param>
        /// <param name="durationMillisecond"></param>
        public Tone(double frequency, int durationMillisecond)
        {
            Duration = TimeSpan.FromMilliseconds(durationMillisecond);
            ObertonFrequencies = new double[] { frequency };
        }
    }

    public class Sequiention
    {
        public TimeSpan TotalDuration { get; set; }
        public IList<Tone> Tones { get; set; }
        public string Title { get; set; }
        public float[]? Timber { get; set; }
    }
}
