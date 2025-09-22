using MathToMusic.Contracts;

namespace MathToMusic.Contracts
{
    /// <summary>
    /// Interface for sequence processors that can transform sequences before audio processing
    /// </summary>
    public interface ISequenceProcessor
    {
        /// <summary>
        /// Process and transform a sequence, potentially adding overtones, effects, or other modifications
        /// </summary>
        /// <param name="sequence">Input sequence to process</param>
        /// <returns>Processed sequence with transformations applied</returns>
        Sequiention Process(Sequiention sequence);
        
        /// <summary>
        /// Process multiple sequences
        /// </summary>
        /// <param name="sequences">Input sequences to process</param>
        /// <returns>Processed sequences with transformations applied</returns>
        IList<Sequiention> Process(IList<Sequiention> sequences);
    }
}