using MathToMusic.Contracts;
using MathToMusic.Outputs;

namespace MathToMusic.Extensions
{
    /// <summary>
    /// Fluent API extensions for creating composable file output with post-processing actions
    /// </summary>
    public static class FileOutputExtensions
    {
        /// <summary>
        /// Wraps the file output to open the file location in Windows Explorer after creation
        /// </summary>
        /// <param name="output">The file output to wrap</param>
        /// <returns>A wrapped output that will open the file location</returns>
        public static ITonesFileOutput OpenFileLocation(this ITonesFileOutput output)
        {
            return new OpenFileLocationOutput(output);
        }

        /// <summary>
        /// Wraps the file output to open the file in the default Windows application after creation
        /// </summary>
        /// <param name="output">The file output to wrap</param>
        /// <returns>A wrapped output that will open the file</returns>
        public static ITonesFileOutput OpenFile(this ITonesFileOutput output)
        {
            return new OpenFileOutput(output);
        }
    }
}