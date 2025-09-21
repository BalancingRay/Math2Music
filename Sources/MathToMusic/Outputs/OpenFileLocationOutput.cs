using System.Diagnostics;
using MathToMusic.Contracts;

namespace MathToMusic.Outputs
{
    /// <summary>
    /// Wrapper that opens the file location in Windows Explorer after the underlying output creates the file
    /// </summary>
    internal class OpenFileLocationOutput : ITonesFileOutput
    {
        private readonly ITonesFileOutput _innerOutput;

        public OpenFileLocationOutput(ITonesFileOutput innerOutput)
        {
            _innerOutput = innerOutput ?? throw new ArgumentNullException(nameof(innerOutput));
        }

        public void Send(IList<Sequiention> input)
        {
            SendAndGetFilePath(input);
        }

        public string? SendAndGetFilePath(IList<Sequiention> input)
        {
            string? filePath = _innerOutput.SendAndGetFilePath(input);
            
            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                OpenFileLocationInExplorer(filePath);
            }
            
            return filePath;
        }

        private static void OpenFileLocationInExplorer(string filePath)
        {
            try
            {
                if (OperatingSystem.IsWindows())
                {
                    // Use explorer.exe with /select parameter to highlight the file
                    string arguments = $"/select,\"{filePath}\"";
                    Process.Start("explorer.exe", arguments);
                    Console.WriteLine($"Opened file location in Explorer: {Path.GetDirectoryName(filePath)}");
                }
                else
                {
                    Console.WriteLine("Opening file location is only supported on Windows.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to open file location: {ex.Message}");
            }
        }
    }
}