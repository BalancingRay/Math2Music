using MathToMusic.Contracts;

namespace MathToMusic.Contracts
{
    /// <summary>
    /// Interface for sequence processors that can transform sequences before audio processing
    /// </summary>
    /// <typeparam name="TTimber">The type of timber/effect to apply</typeparam>
    public interface ISequenceProcessor<TTimber>
    {
        /// <summary>
        /// Process and transform a sequence, potentially adding overtones, effects, or other modifications
        /// </summary>
        /// <param name="sequence">Input sequence to process</param>
        /// <param name="timber">Timber profile to apply</param>
        /// <returns>Processed sequence with transformations applied</returns>
        Sequiention Process(Sequiention sequence, TTimber timber);
        
        /// <summary>
        /// Process multiple sequences with the same timber
        /// </summary>
        /// <param name="sequences">Input sequences to process</param>
        /// <param name="timber">Timber profile to apply</param>
        /// <returns>Processed sequences with transformations applied</returns>
        IList<Sequiention> Process(IList<Sequiention> sequences, TTimber timber);
    }
}