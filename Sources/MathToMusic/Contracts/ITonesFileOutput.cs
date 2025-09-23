namespace MathToMusic.Contracts
{
    /// <summary>
    /// Extended interface for tones output that returns the created file path
    /// for post-processing actions like opening in explorer or default application
    /// </summary>
    public interface ITonesFileOutput : ITonesOutput
    {
        /// <summary>
        /// Sends tones to output and returns the path of the created file
        /// </summary>
        /// <param name="input">The tone sequences to output</param>
        /// <returns>The path to the created file, or null if no file was created</returns>
        string? ProcessAndGetFilePath(IList<Sequiention> input);
    }
}