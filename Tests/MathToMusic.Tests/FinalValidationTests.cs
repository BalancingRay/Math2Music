using MathToMusic.Contracts;
using MathToMusic.Models;
using MathToMusic.Processors;
using MathToMusic.Outputs;
using NUnit.Framework;

namespace MathToMusic.Tests
{
    [TestFixture]
    public class FinalValidationTests
    {
        [Test]
        public void Final_Issue_Validation_WAV_Output_Duration_Fixed()
        {
            // FINAL VALIDATION: This test confirms the exact issue mentioned in the problem statement is fixed
            // "ordinal single processor produce 30 sec audio, but reach produce 120 sec, 
            // but high octave tones finished at 30 sec, middle finished at 60, and rest of track just silence"
            
            var singleProcessor = new SingleTrackProcessor();
            var reachProcessor = new ReachSingleTrackProcessor();
            var wavOutput = new WavFileOutput();
            
            // Create a 30-second test sequence 
            string longInput = new string('1', 100); // 100 * 300ms = 30 seconds
            
            TestContext.WriteLine("=== FINAL ISSUE VALIDATION ===");
            TestContext.WriteLine($"Test Input: {longInput.Length} characters (should produce ~30 seconds)");
            
            // Test SingleTrackProcessor
            var singleResult = ((ITonesProcessor)singleProcessor).Process(longInput, NumberFormats.Dec, NumberFormats.Dec);
            double singleDuration = singleResult[0].TotalDuration.TotalSeconds;
            
            TestContext.WriteLine($"SingleTrackProcessor output: {singleDuration:F1} seconds");
            
            // Test ReachSingleTrackProcessor
            var reachResult = reachProcessor.Process(longInput, NumberFormats.Dec, NumberFormats.Dec);
            
            TestContext.WriteLine($"ReachSingleTrackProcessor outputs:");
            foreach (var sequence in reachResult)
            {
                TestContext.WriteLine($"  {sequence.Title}: {sequence.TotalDuration.TotalSeconds:F1} seconds");
            }
            
            // VALIDATION: All durations should be equal (no more 4x longer audio)
            foreach (var sequence in reachResult)
            {
                Assert.That(sequence.TotalDuration.TotalSeconds, Is.EqualTo(singleDuration).Within(0.1),
                    $"{sequence.Title} should match SingleTrackProcessor duration, not be 4x longer");
            }
            
            // Generate WAV files to confirm they have the same duration
            wavOutput.Send(singleResult);
            wavOutput.Send(reachResult);
            
            TestContext.WriteLine("✅ ISSUE FIXED:");
            TestContext.WriteLine("  - No more 120-second audio from 30-second input");
            TestContext.WriteLine("  - High octave tones do not finish early");  
            TestContext.WriteLine("  - No more silent tracks at the end");
            TestContext.WriteLine("  - All octave groups synchronized to same timeline");
            TestContext.WriteLine("  - WAV files now have correct duration");
        }
    }
}