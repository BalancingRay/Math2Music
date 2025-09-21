using MathToMusic.Contracts;
using MathToMusic.Extensions;
using MathToMusic.Outputs;

namespace MathToMusic.Demo
{
    /// <summary>
    /// Demonstration program showing the new fluent API for WavFileOutput with Windows post-processing
    /// </summary>
    public static class FluentApiDemo
    {
        public static void DemonstrateFluentApi()
        {
            Console.WriteLine("=== WavFileOutput Fluent API Demonstration ===");
            
            // Create some sample tones
            var tones = new List<Tone>
            {
                new Tone(440.0, 500),   // A4
                new Tone(523.25, 500),  // C5
                new Tone(659.25, 500)   // E5
            };
            
            var sequence = new Sequiention
            {
                Tones = tones,
                TotalDuration = TimeSpan.FromSeconds(1.5),
                Title = "Demo"
            };
            
            var input = new List<Sequiention> { sequence };
            
            Console.WriteLine("\n1. Basic WavFileOutput (no post-processing):");
            var basicOutput = new WavFileOutput();
            string? filePath1 = basicOutput.SendAndGetFilePath(input);
            Console.WriteLine($"Created file: {filePath1}");
            
            Console.WriteLine("\n2. WavFileOutput with OpenFileLocation():");
            var outputWithLocation = new WavFileOutput().OpenFileLocation();
            string? filePath2 = outputWithLocation.SendAndGetFilePath(input);
            Console.WriteLine($"Created file and opened location: {filePath2}");
            
            Console.WriteLine("\n3. WavFileOutput with OpenFile():");
            var outputWithFile = new WavFileOutput().OpenFile();
            string? filePath3 = outputWithFile.SendAndGetFilePath(input);
            Console.WriteLine($"Created file and opened in app: {filePath3}");
            
            Console.WriteLine("\n4. WavFileOutput with chained post-processing:");
            var chainedOutput = new WavFileOutput().OpenFileLocation().OpenFile();
            string? filePath4 = chainedOutput.SendAndGetFilePath(input);
            Console.WriteLine($"Created file, opened location AND opened in app: {filePath4}");
            
            Console.WriteLine("\n=== Demonstration Complete ===");
            Console.WriteLine("\nNote: Windows Explorer actions will only work on Windows platform.");
            Console.WriteLine("On non-Windows platforms, informational messages are displayed instead.");
        }
    }
}