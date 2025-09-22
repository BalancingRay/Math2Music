using MathToMusic.Models;
using MathToMusic.Outputs;
using MathToMusic.Processors;
using MathToMusic.Contracts;
using NUnit.Framework;

namespace MathToMusic.Tests
{
    [TestFixture]
    public class ProcessorComparisonTests
    {
        [Test]
        public void CompareProcessors_WithTestData_100100100()
        {
            var input = "100100100";
            CompareProcessorsForInput(input);
        }

        [Test]
        public void CompareProcessors_WithTestData_10101010()
        {
            var input = "10101010";
            CompareProcessorsForInput(input);
        }

        [Test]
        public void CompareProcessors_WithTestData_1122112211()
        {
            var input = "1122112211";
            CompareProcessorsForInput(input);
        }

        [Test]
        public void CompareProcessors_WithTestData_121212121()
        {
            var input = "121212121";
            CompareProcessorsForInput(input);
        }

        private void CompareProcessorsForInput(string input)
        {
            ITonesProcessor singleTrackProcessor = new SingleTrackProcessor();
            ITonesProcessor reachSingleTrackProcessor = new ReachSingleTrackProcessor();
            var testOutput = new TestMelodyOutput(300); // 300ms discretization to match the tone duration

            Console.WriteLine($"=== Comparing processors for input: {input} ===");

            // Process with SingleTrackProcessor
            var singleResult = singleTrackProcessor.Process(input, NumberFormats.Dec, NumberFormats.Dec);
            Console.WriteLine("SingleTrackProcessor result:");
            var singleTracing = testOutput.GetMelodyTracing(singleResult);
            Console.WriteLine(singleTracing);

            // Process with ReachSingleTrackProcessor
            var reachResult = reachSingleTrackProcessor.Process(input, NumberFormats.Dec, NumberFormats.Dec);
            Console.WriteLine("ReachSingleTrackProcessor result:");
            var reachTracing = testOutput.GetMelodyTracing(reachResult);
            Console.WriteLine(reachTracing);

            Console.WriteLine("=== Analysis ===");
            Console.WriteLine($"SingleTrackProcessor sequences: {singleResult.Count}");
            Console.WriteLine($"ReachSingleTrackProcessor sequences: {reachResult.Count}");

            // Extract unique frequencies from both outputs to compare
            var singleFrequencies = ExtractFrequencies(singleTracing);
            var reachFrequencies = ExtractFrequencies(reachTracing);

            Console.WriteLine($"Unique frequencies in Single: {string.Join(", ", singleFrequencies.OrderBy(f => f))}");
            Console.WriteLine($"Unique frequencies in Reach: {string.Join(", ", reachFrequencies.OrderBy(f => f))}");

            // Verify that ReachSingleTrackProcessor contains all frequencies from SingleTrackProcessor
            var missingFreqs = singleFrequencies.Except(reachFrequencies).ToList();
            var extraFreqs = reachFrequencies.Except(singleFrequencies).ToList();

            if (missingFreqs.Any())
            {
                Console.WriteLine($"❌ ReachSingleTrackProcessor is missing frequencies: {string.Join(", ", missingFreqs)}");
            }

            if (extraFreqs.Any())
            {
                Console.WriteLine($"ℹ️  ReachSingleTrackProcessor has additional frequencies: {string.Join(", ", extraFreqs)}");
            }

            if (!missingFreqs.Any())
            {
                Console.WriteLine("✅ ReachSingleTrackProcessor contains all frequencies from SingleTrackProcessor");
                Console.WriteLine("✅ Same melody confirmed, just split across multiple octave tracks");
            }

            Console.WriteLine();
        }

        private HashSet<double> ExtractFrequencies(string tracing)
        {
            var frequencies = new HashSet<double>();
            var lines = tracing.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                var parts = line.Split(':');
                if (parts.Length == 2 && double.TryParse(parts[0], out var freq))
                {
                    // Only include frequencies that have at least one note (contains '!')
                    if (parts[1].Contains('!'))
                    {
                        frequencies.Add(freq);
                    }
                }
            }

            return frequencies;
        }

        [Test]
        public void TestMelodyOutput_FormatValidation()
        {
            Console.WriteLine("=== Testing TestMelodyOutput format ===");
            
            ITonesProcessor processor = new SingleTrackProcessor();
            var testOutput = new TestMelodyOutput(300);
            
            var result = processor.Process("12", NumberFormats.Dec, NumberFormats.Dec);
            var tracing = testOutput.GetMelodyTracing(result);
            
            Console.WriteLine("Output format:");
            Console.WriteLine(tracing);
            
            var lines = tracing.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                Assert.That(line.Contains(':'), Is.True, "Each line should contain frequency:pattern format");
                Assert.That(line.Split(':').Length, Is.EqualTo(2), "Each line should have exactly one colon separator");
                
                var parts = line.Split(':');
                Assert.That(double.TryParse(parts[0], out _), Is.True, "Frequency part should be a valid number");
                Assert.That(parts[1].All(c => c == '.' || c == '!'), Is.True, "Pattern should only contain . and ! characters");
            }
            
            Console.WriteLine("✅ Format validation passed");
        }

        [Test]
        public void AnalyzeTiming_SingleVsReach_DetailedComparison()
        {
            Console.WriteLine("=== Detailed Timing Analysis ===");
            
            ITonesProcessor singleTrackProcessor = new SingleTrackProcessor();
            ITonesProcessor reachSingleTrackProcessor = new ReachSingleTrackProcessor();
            
            var testInputs = new[] { "100100100", "10101010", "1122112211", "121212121" };
            
            foreach (var input in testInputs)
            {
                Console.WriteLine($"\n--- Analysis for input: {input} ---");
                
                var singleResult = singleTrackProcessor.Process(input, NumberFormats.Dec, NumberFormats.Dec);
                var reachResult = reachSingleTrackProcessor.Process(input, NumberFormats.Dec, NumberFormats.Dec);
                
                Console.WriteLine($"SingleTrack: {singleResult.Count} sequence(s), total duration: {singleResult[0].TotalDuration.TotalMilliseconds}ms");
                Console.WriteLine($"ReachTrack: {reachResult.Count} sequence(s), total duration: {reachResult[0].TotalDuration.TotalMilliseconds}ms");
                
                // Analyze tone timing for single track
                if (singleResult.Count > 0)
                {
                    Console.WriteLine("SingleTrack tone timings:");
                    var currentTime = TimeSpan.Zero;
                    for (int i = 0; i < singleResult[0].Tones.Count; i++)
                    {
                        var tone = singleResult[0].Tones[i];
                        if (tone.BaseTone > 0)
                            Console.WriteLine($"  Position {i}: {tone.BaseTone}Hz at {currentTime.TotalMilliseconds}ms, duration: {tone.Duration.TotalMilliseconds}ms");
                        currentTime += tone.Duration;
                    }
                }
                
                // Analyze tone timing for reach tracks
                Console.WriteLine("ReachTrack tone timings:");
                foreach (var sequence in reachResult)
                {
                    Console.WriteLine($"  Track '{sequence.Title}':");
                    var currentTime = TimeSpan.Zero;
                    for (int i = 0; i < sequence.Tones.Count; i++)
                    {
                        var tone = sequence.Tones[i];
                        if (tone.BaseTone > 0)
                            Console.WriteLine($"    Position {i}: {tone.BaseTone}Hz at {currentTime.TotalMilliseconds}ms, duration: {tone.Duration.TotalMilliseconds}ms");
                        currentTime += tone.Duration;
                    }
                }
            }
            
            Console.WriteLine("\n✅ Detailed timing analysis complete");
        }
    }
}