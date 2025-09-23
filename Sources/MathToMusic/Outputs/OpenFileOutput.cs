using MathToMusic.Contracts;
using System.Diagnostics;

namespace MathToMusic.Outputs
{
    /// <summary>
    /// Wrapper that opens the file in the default Windows application after the underlying output creates the file
    /// </summary>
    internal class OpenFileOutput : ITonesFileOutput
    {
        private readonly ITonesFileOutput _innerOutput;

        public OpenFileOutput(ITonesFileOutput innerOutput)
        {
            _innerOutput = innerOutput ?? throw new ArgumentNullException(nameof(innerOutput));
        }

        public void Send(IList<Sequiention> input)
        {
            ProcessAndGetFilePath(input);
        }

        public string? ProcessAndGetFilePath(IList<Sequiention> input)
        {
            string? filePath = _innerOutput.ProcessAndGetFilePath(input);

            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                OpenFileInDefaultApplication(filePath);
            }

            return filePath;
        }

        private static void OpenFileInDefaultApplication(string filePath)
        {
            try
            {
                if (OperatingSystem.IsWindows())
                {
                    // Use Process.Start with UseShellExecute to open with default app
                    var startInfo = new ProcessStartInfo
                    {
                        FileName = filePath,
                        UseShellExecute = true
                    };
                    Process.Start(startInfo);
                    Console.WriteLine($"Opened file in default application: {Path.GetFileName(filePath)}");
                }
                else
                {
                    Console.WriteLine("Opening file in default application is only supported on Windows.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to open file: {ex.Message}");
            }
        }
    }
}