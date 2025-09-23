using MathToMusic.Contracts;
using MathToMusic.Models;
using MathToMusic.Processors;
using NUnit.Framework;

namespace MathToMusic.Tests
{
    [TestFixture]
    public class ReachSingleTrackProcessorTests
    {
        private const int DefaultBaseDurationMs = 300; // Default base duration for processors
        private ITonesProcessor _processor;

        [SetUp]
        public void Setup()
        {
            _processor = new ReachSingleTrackProcessor();
        }

        [Test]
        public void Process_HexFormat_CreatesCorrectOctaveGroups()
        {
            // Arrange
            string input = "123456789ABCDEF";

            // Act
            var result = _processor.Process(input, NumberFormats.Hex, NumberFormats.Hex);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(4)); // 4 octave groups for HEX

            // Check group names and counts
            Assert.That(result[0].Title, Is.EqualTo("Octave_Low"));
            Assert.That(result[1].Title, Is.EqualTo("Octave_MidLow"));
            Assert.That(result[2].Title, Is.EqualTo("Octave_MidHigh"));
            Assert.That(result[3].Title, Is.EqualTo("Octave_High"));
        }

        [Test]
        public void Process_HexFormat_AppliesCorrectDurationMultipliers()
        {
            // Arrange
            string input = "1234"; // One tone from each major group
            int baseDuration = DefaultBaseDurationMs;

            // Act
            var result = _processor.Process(input, NumberFormats.Hex, NumberFormats.Hex);

            // Assert
            Assert.That(result, Has.Count.EqualTo(4));

            // After fix: Duration multipliers control "reach" duration within the sequence timeline
            // Based on actual behavior from debug test:
            
            // Group 1 (tone 1 at position 0): gets full sequence remaining duration = 4*DefaultBaseDurationMs = 1200ms
            var group1ActiveTone = result[0].Tones.First(t => t.ObertonFrequencies[0] != 0);
            Assert.That(group1ActiveTone.Duration.TotalMilliseconds, Is.EqualTo(1200), 
                "Group 1 tone should get full remaining sequence duration");

            // Group 2 (tone 2 at position 1): gets remaining from position 1 = 3*DefaultBaseDurationMs = 900ms  
            var group2ActiveTone = result[1].Tones.First(t => t.ObertonFrequencies[0] != 0);
            Assert.That(group2ActiveTone.Duration.TotalMilliseconds, Is.EqualTo(900), 
                "Group 2 tone should get remaining duration from its position");

            // Group 3 (tone 3 at position 2): gets limited by reach multiplier and remaining positions
            var group2SecondTone = result[1].Tones[2]; // Tone 3 in MidLow group
            Assert.That(group2SecondTone.Duration.TotalMilliseconds, Is.EqualTo(600), 
                "Group 2 second tone should get remaining duration from its position");

            // Group 4 (tone 4 at position 3): gets base duration only (last position)
            var group3ActiveTone = result[2].Tones.First(t => t.ObertonFrequencies[0] != 0);
            Assert.That(group3ActiveTone.Duration.TotalMilliseconds, Is.EqualTo(300), 
                "Group 3 tone should get base duration (last position)");
        }

        [Test]
        public void Process_HexFormat_CorrectToneGrouping()
        {
            // Arrange
            string input = "123456789ABCDEF";
            double baseFreq = 180;

            // Act
            var result = _processor.Process(input, NumberFormats.Hex, NumberFormats.Hex);

            // Assert
            // Group 1 should only have tone 1 active
            var group1ActiveTones = result[0].Tones.Where(t => t.ObertonFrequencies[0] != 0).ToList();
            Assert.That(group1ActiveTones, Has.Count.EqualTo(1));
            Assert.That(group1ActiveTones[0].ObertonFrequencies[0], Is.EqualTo(baseFreq * 1));

            // Group 2 should have tones 2,3 active
            var group2ActiveTones = result[1].Tones.Where(t => t.ObertonFrequencies[0] != 0).ToList();
            Assert.That(group2ActiveTones, Has.Count.EqualTo(2));
            Assert.That(group2ActiveTones[0].ObertonFrequencies[0], Is.EqualTo(baseFreq * 2));
            Assert.That(group2ActiveTones[1].ObertonFrequencies[0], Is.EqualTo(baseFreq * 3));

            // Group 3 should have tones 4,5,6,7 active
            var group3ActiveTones = result[2].Tones.Where(t => t.ObertonFrequencies[0] != 0).ToList();
            Assert.That(group3ActiveTones, Has.Count.EqualTo(4));

            // Group 4 should have tones 8,9,A,B,C,D,E,F active
            var group4ActiveTones = result[3].Tones.Where(t => t.ObertonFrequencies[0] != 0).ToList();
            Assert.That(group4ActiveTones, Has.Count.EqualTo(8));
        }

        [Test]
        public void Process_OctFormat_CreatesCorrectOctaveGroups()
        {
            // Arrange
            string input = "1234567";

            // Act
            var result = _processor.Process(input, NumberFormats.Oct, NumberFormats.Oct);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(3)); // 3 octave groups for OCT

            Assert.That(result[0].Title, Is.EqualTo("Octave_Low"));
            Assert.That(result[1].Title, Is.EqualTo("Octave_Mid"));
            Assert.That(result[2].Title, Is.EqualTo("Octave_High"));
        }

        [Test]
        public void Process_OctFormat_AppliesCorrectDurationMultipliers()
        {
            // Arrange
            string input = "124"; // One tone from each group
            int baseDuration = DefaultBaseDurationMs;

            // Act
            var result = _processor.Process(input, NumberFormats.Oct, NumberFormats.Oct);

            // Assert
            Assert.That(result, Has.Count.EqualTo(3));

            // Based on actual behavior: tones get remaining sequence duration from their position
            // Group 1 (tone 1 at position 0): gets 3*300 = 900ms (full remaining)
            var group1ActiveTone = result[0].Tones.First(t => t.ObertonFrequencies[0] != 0);
            Assert.That(group1ActiveTone.Duration.TotalMilliseconds, Is.EqualTo(900), 
                "Group 1 tone should get full remaining sequence duration");

            // Group 2 (tone 2 at position 1): gets 2*300 = 600ms (remaining from position 1)
            var group2ActiveTone = result[1].Tones.First(t => t.ObertonFrequencies[0] != 0);
            Assert.That(group2ActiveTone.Duration.TotalMilliseconds, Is.EqualTo(600), 
                "Group 2 tone should get remaining duration from its position");

            // Group 3 (tone 4 at position 2): gets 1*300 = 300ms (last position)
            var group3ActiveTone = result[2].Tones.First(t => t.ObertonFrequencies[0] != 0);
            Assert.That(group3ActiveTone.Duration.TotalMilliseconds, Is.EqualTo(300),
                "Group 3 tone should get base duration (last position)");
        }

        [Test]
        public void Process_QadFormat_CreatesCorrectOctaveGroups()
        {
            // Arrange
            string input = "123";

            // Act
            var result = _processor.Process(input, NumberFormats.Qad, NumberFormats.Qad);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(2)); // 2 octave groups for QUAD

            Assert.That(result[0].Title, Is.EqualTo("Octave_Low"));
            Assert.That(result[1].Title, Is.EqualTo("Octave_High"));
        }

        [Test]
        public void Process_QadFormat_AppliesCorrectDurationMultipliers()
        {
            // Arrange
            string input = "12"; // One tone from each group
            int baseDuration = DefaultBaseDurationMs;

            // Act
            var result = _processor.Process(input, NumberFormats.Qad, NumberFormats.Qad);

            // Assert
            Assert.That(result, Has.Count.EqualTo(2));

            // Group 1 (tone 1): duration x2 = 600ms
            var group1ActiveTone = result[0].Tones.First(t => t.ObertonFrequencies[0] != 0);
            Assert.That(group1ActiveTone.Duration.TotalMilliseconds, Is.EqualTo(baseDuration * 2));

            // Group 2 (tone 2): duration x1 = 300ms
            var group2ActiveTone = result[1].Tones.First(t => t.ObertonFrequencies[0] != 0);
            Assert.That(group2ActiveTone.Duration.TotalMilliseconds, Is.EqualTo(baseDuration * 1));
        }

        [Test]
        public void Process_SilenceForNonGroupTones_CreatesCorrectPauses()
        {
            // Arrange
            string input = "13"; // Only tones from different groups

            // Act
            var result = _processor.Process(input, NumberFormats.Hex, NumberFormats.Hex);

            // Assert
            Assert.That(result, Has.Count.EqualTo(4));

            // Group 1: Should have tone 1 active, tone 3 silent
            var group1Tones = result[0].Tones;
            Assert.That(group1Tones[0].ObertonFrequencies[0], Is.EqualTo(180 * 1)); // Active
            Assert.That(group1Tones[1].ObertonFrequencies[0], Is.EqualTo(0)); // Silent

            // Group 2: Should have tone 1 silent, tone 3 active
            var group2Tones = result[1].Tones;
            Assert.That(group2Tones[0].ObertonFrequencies[0], Is.EqualTo(0)); // Silent
            Assert.That(group2Tones[1].ObertonFrequencies[0], Is.EqualTo(180 * 3)); // Active
        }

        [Test]
        public void Process_EmptyInput_ReturnsEmptySequences()
        {
            // Arrange
            string input = "";

            // Act
            var result = _processor.Process(input, NumberFormats.Hex, NumberFormats.Hex);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(4)); // Still creates groups but they're empty
            foreach (var sequence in result)
            {
                Assert.That(sequence.Tones, Has.Count.EqualTo(0));
            }
        }

        [Test]
        public void Process_InvalidCharacters_SkipsInvalidChars()
        {
            // Arrange - using character beyond Base32 range
            string input = "1X2"; // X is invalid for any supported format (beyond Base32's V)

            // Act
            var result = _processor.Process(input, NumberFormats.Dec, NumberFormats.Dec);

            // Assert
            Assert.That(result, Is.Not.Null);
            // Should only process '1' and '2', skipping 'X'
            foreach (var sequence in result)
            {
                Assert.That(sequence.Tones, Has.Count.EqualTo(2));
            }
        }

        [Test]
        public void Process_TotalDuration_CalculatedCorrectly()
        {
            // Arrange
            string input = "12";
            int baseDuration = DefaultBaseDurationMs;

            // Act
            var result = _processor.Process(input, NumberFormats.Hex, NumberFormats.Hex);

            // Assert
            // After fix: All sequences should have the same total duration (timeline synchronization)
            int expectedTotalDuration = input.Length * baseDuration; // 2 * 300ms = 600ms
            
            foreach (var sequence in result)
            {
                Assert.That(sequence.TotalDuration.TotalMilliseconds, Is.EqualTo(expectedTotalDuration),
                    $"All octave groups should have synchronized duration of {expectedTotalDuration}ms");
            }
        }

        [Test]
        public void Compare_SingleTrack_vs_ReachSingleTrack_SameInput()
        {
            // Arrange
            var singleProcessor = new SingleTrackProcessor();
            var reachProcessor = new ReachSingleTrackProcessor();
            string input = "123456";

            // Act
            var singleResult = ((ITonesProcessor)singleProcessor).Process(input, NumberFormats.Dec, NumberFormats.Dec);
            var reachResult = reachProcessor.Process(input, NumberFormats.Dec, NumberFormats.Dec);

            // Debug output
            TestContext.WriteLine("=== SingleTrackProcessor Result ===");
            TestContext.WriteLine($"Sequences: {singleResult.Count}");
            foreach (var seq in singleResult)
            {
                TestContext.WriteLine($"  {seq.Title}: {seq.Tones.Count} tones, {seq.TotalDuration.TotalMilliseconds}ms total");
                foreach (var tone in seq.Tones.Take(10))
                {
                    TestContext.WriteLine($"    {tone.ObertonFrequencies[0]}Hz, {tone.Duration.TotalMilliseconds}ms");
                }
            }

            TestContext.WriteLine("=== ReachSingleTrackProcessor Result ===");
            TestContext.WriteLine($"Sequences: {reachResult.Count}");
            foreach (var seq in reachResult)
            {
                TestContext.WriteLine($"  {seq.Title}: {seq.Tones.Count} tones, {seq.TotalDuration.TotalMilliseconds}ms total");
                foreach (var tone in seq.Tones.Take(6))
                {
                    TestContext.WriteLine($"    {tone.ObertonFrequencies[0]}Hz, {tone.Duration.TotalMilliseconds}ms");
                }
            }

            // Basic assertions
            Assert.That(singleResult, Is.Not.Null);
            Assert.That(reachResult, Is.Not.Null);
        }
        
        [Test]
        public void Test_ReachSingleTrack_Duration_Problem()
        {
            // Test the specific problem mentioned: duration should be reduced when next tone 
            // from same octave starts before original duration finishes
            
            var processor = new ReachSingleTrackProcessor();
            
            TestContext.WriteLine("=== Test tone overlap in same octave group ===");
            string input = "1F1"; // Two '1's (Octave_Low) with 'F' (Octave_High) in between
            var result = processor.Process(input, NumberFormats.Hex, NumberFormats.Hex);
            
            // Focus on Octave_Low which should have the overlapping '1' tones
            var octaveLow = result.FirstOrDefault(s => s.Title == "Octave_Low");
            Assert.That(octaveLow, Is.Not.Null);
            
            TestContext.WriteLine($"Octave_Low has {octaveLow.Tones.Count} tones:");
            for (int i = 0; i < octaveLow.Tones.Count; i++)
            {
                var tone = octaveLow.Tones[i];
                TestContext.WriteLine($"  Tone {i+1}: {tone.ObertonFrequencies[0]}Hz, {tone.Duration.TotalMilliseconds}ms");
            }
            
            // After fix: The first '1' (position 0) should be shortened because the second '1' (position 2)
            // starts at 2*300ms = 600ms, which is before the original reach duration would finish
            var firstTone = octaveLow.Tones[0];
            var thirdTone = octaveLow.Tones[2];
            
            TestContext.WriteLine($"First '1' tone: {firstTone.Duration.TotalMilliseconds}ms (should be 600ms)");
            TestContext.WriteLine($"Third '1' tone: {thirdTone.Duration.TotalMilliseconds}ms (should be limited by remaining sequence)");
            
            // Verify the first tone was shortened to the time until the next same tone
            Assert.That(firstTone.Duration.TotalMilliseconds, Is.EqualTo(600), 
                "First '1' tone should be shortened to 600ms when second '1' starts");
                
            // The third tone should only get the remaining sequence duration (1 position * 300ms)
            Assert.That(thirdTone.Duration.TotalMilliseconds, Is.EqualTo(300),
                "Third '1' tone should get remaining sequence duration (300ms)");
        }
        
        [Test]
        public void Compare_SingleTrack_vs_ReachSingleTrack_ProduceSameMelody()
        {
            // This test verifies that ReachSingleTrackProcessor produces the same melody
            // as SingleTrackProcessor but with polyphonic structure
            
            var singleProcessor = new SingleTrackProcessor();
            var reachProcessor = new ReachSingleTrackProcessor();
            string input = "123456";
            
            var singleResult = ((ITonesProcessor)singleProcessor).Process(input, NumberFormats.Dec, NumberFormats.Dec);
            var reachResult = reachProcessor.Process(input, NumberFormats.Dec, NumberFormats.Dec);
            
            TestContext.WriteLine("=== Melody Comparison: SingleTrack vs ReachSingleTrack ===");
            TestContext.WriteLine($"SingleTrack: {singleResult.Count} sequence(s)");
            TestContext.WriteLine($"ReachSingleTrack: {reachResult.Count} sequence(s)");
            
            // Both should have the same sequence of frequencies (same melody)
            var singleFreqs = singleResult[0].Tones.Select(t => t.ObertonFrequencies[0]).ToList();
            
            // For ReachSingleTrack with Dec format, there should be only one octave group (default behavior)
            var reachFreqs = new List<double>();
            foreach (var seq in reachResult)
            {
                foreach (var tone in seq.Tones)
                {
                    if (tone.ObertonFrequencies[0] != 0) // Skip silent tones
                    {
                        reachFreqs.Add(tone.ObertonFrequencies[0]);
                    }
                }
            }
            
            TestContext.WriteLine("Single frequencies: " + string.Join(", ", singleFreqs));
            TestContext.WriteLine("Reach frequencies:  " + string.Join(", ", reachFreqs));
            
            Assert.That(reachFreqs, Is.EqualTo(singleFreqs), 
                "ReachSingleTrackProcessor should produce the same melody as SingleTrackProcessor");
        }

        [Test]
        public void Process_BinaryToHex_PerformsConversionAndGrouping()
        {
            // Arrange
            string input = "00010010"; // Binary that converts to 1,2 in hex

            // Act
            var result = _processor.Process(input, NumberFormats.Hex, NumberFormats.Bin);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(4));

            // Should have processed the converted hex values
            Assert.That(result[0].Tones, Has.Count.GreaterThan(0));
        }

        [Test]
        public void Test_Duration_Issue_Reproduction()
        {
            // This test reproduces the specific issue mentioned in the problem statement:
            // ReachSingleTrackProcessor USED TO produce audio 4x longer than SingleTrackProcessor
            // but AFTER FIX should produce the same duration with polyphonic octave groups
            
            var singleProcessor = new SingleTrackProcessor();
            var reachProcessor = new ReachSingleTrackProcessor();
            
            string testInput = "23456789"; // No tone '1' as mentioned in the issue
            
            TestContext.WriteLine("=== DURATION ISSUE FIXED ===");
            TestContext.WriteLine($"Input: {testInput}");
            
            // Test Single Track Processor
            var singleResult = ((ITonesProcessor)singleProcessor).Process(testInput, NumberFormats.Dec, NumberFormats.Dec);
            TestContext.WriteLine($"SingleTrackProcessor: {singleResult[0].TotalDuration.TotalSeconds}s");
            
            // Test Reach Single Track Processor
            var reachResult = reachProcessor.Process(testInput, NumberFormats.Dec, NumberFormats.Dec);
            
            double maxReachDuration = 0;
            foreach (var seq in reachResult)
            {
                maxReachDuration = Math.Max(maxReachDuration, seq.TotalDuration.TotalSeconds);
                TestContext.WriteLine($"ReachSingleTrackProcessor {seq.Title}: {seq.TotalDuration.TotalSeconds}s");
            }
            
            TestContext.WriteLine($"Duration ratio: {maxReachDuration / singleResult[0].TotalDuration.TotalSeconds:F1}x");
            
            // AFTER FIX: ReachSingleTrackProcessor should produce the same total duration as SingleTrackProcessor
            // All octave groups should have the same timeline duration, just with polyphonic content
            Assert.That(maxReachDuration, Is.EqualTo(singleResult[0].TotalDuration.TotalSeconds).Within(0.1),
                "ReachSingleTrackProcessor should produce the same total duration as SingleTrackProcessor");
                
            // All sequences should have the same total duration (synchronized timeline)
            foreach (var seq in reachResult)
            {
                Assert.That(seq.TotalDuration.TotalSeconds, Is.EqualTo(singleResult[0].TotalDuration.TotalSeconds).Within(0.1),
                    $"All octave groups should have synchronized timeline duration, but {seq.Title} differs");
            }
        }
        
        [Test]
        public void Test_Wav_Output_Duration_Fix()
        {
            // This test verifies the specific issue described in the problem statement:
            // "ordinal single processor produce 30 sec audio, but reach produce 120 sec, 
            // but high octave tones finished at 30 sec, middle finished at 60, and rest of track just silence"
            
            var singleProcessor = new SingleTrackProcessor();
            var reachProcessor = new ReachSingleTrackProcessor();
            
            // Create a test input that would produce ~30 seconds with SingleTrackProcessor
            string longInput = new string('1', 100); // 100 tones of '1' = 100*300ms = 30 seconds
            
            TestContext.WriteLine("=== WAV OUTPUT DURATION FIX TEST ===");
            TestContext.WriteLine($"Input: 100 tones of '1' (should be ~30 seconds)");
            
            // Test Single Track Processor
            var singleResult = ((ITonesProcessor)singleProcessor).Process(longInput, NumberFormats.Dec, NumberFormats.Dec);
            double singleDurationSec = singleResult[0].TotalDuration.TotalSeconds;
            TestContext.WriteLine($"SingleTrackProcessor: {singleDurationSec:F1}s");
            
            // Test Reach Single Track Processor  
            var reachResult = reachProcessor.Process(longInput, NumberFormats.Dec, NumberFormats.Dec);
            
            TestContext.WriteLine($"ReachSingleTrackProcessor sequences: {reachResult.Count}");
            foreach (var seq in reachResult)
            {
                TestContext.WriteLine($"  {seq.Title}: {seq.TotalDuration.TotalSeconds:F1}s");
            }
            
            // AFTER FIX: All sequences should have the same total duration as SingleTrackProcessor
            foreach (var seq in reachResult)
            {
                Assert.That(seq.TotalDuration.TotalSeconds, Is.EqualTo(singleDurationSec).Within(0.1),
                    $"{seq.Title} should have same duration as SingleTrackProcessor, not be 4x longer");
            }
            
            TestContext.WriteLine("✓ Fix confirmed: No more 120-second audio with silent tracks");
            TestContext.WriteLine("✓ All octave groups now synchronized to same timeline duration");
        }

        [Test]
        public void Test_Custom_Duration_150ms()
        {
            // Test processors with custom 150ms base duration
            var customDuration = 150;
            var singleProcessor = new SingleTrackProcessor(customDuration);
            var reachProcessor = new ReachSingleTrackProcessor(customDuration);
            
            string input = "123456"; // 6 tones
            var expectedDuration = input.Length * customDuration; // 6 * 150ms = 900ms
            
            TestContext.WriteLine($"=== CUSTOM DURATION TEST: {customDuration}ms ===");
            
            // Test SingleTrackProcessor with custom duration
            var singleResult = ((ITonesProcessor)singleProcessor).Process(input, NumberFormats.Dec, NumberFormats.Dec);
            TestContext.WriteLine($"SingleTrackProcessor: {singleResult[0].TotalDuration.TotalMilliseconds}ms");
            Assert.That(singleResult[0].TotalDuration.TotalMilliseconds, Is.EqualTo(expectedDuration),
                $"SingleTrackProcessor should use custom duration of {customDuration}ms per tone");
            
            // Verify individual tone durations
            foreach (var tone in singleResult[0].Tones)
            {
                Assert.That(tone.Duration.TotalMilliseconds, Is.EqualTo(customDuration),
                    $"Each tone should have duration of {customDuration}ms");
            }
            
            // Test ReachSingleTrackProcessor with custom duration
            var reachResult = reachProcessor.Process(input, NumberFormats.Dec, NumberFormats.Dec);
            foreach (var seq in reachResult)
            {
                TestContext.WriteLine($"  {seq.Title}: {seq.TotalDuration.TotalMilliseconds}ms");
                Assert.That(seq.TotalDuration.TotalMilliseconds, Is.EqualTo(expectedDuration),
                    $"{seq.Title} should use custom duration timeline of {expectedDuration}ms total");
            }
        }

        [Test]
        public void Test_Custom_Duration_500ms()
        {
            // Test processors with custom 500ms base duration
            var customDuration = 500;
            var singleProcessor = new SingleTrackProcessor(customDuration);
            var reachProcessor = new ReachSingleTrackProcessor(customDuration);
            
            string input = "ABC"; // 3 tones
            var expectedDuration = input.Length * customDuration; // 3 * 500ms = 1500ms
            
            TestContext.WriteLine($"=== CUSTOM DURATION TEST: {customDuration}ms ===");
            
            // Test SingleTrackProcessor with custom duration
            var singleResult = ((ITonesProcessor)singleProcessor).Process(input, NumberFormats.Hex, NumberFormats.Hex);
            TestContext.WriteLine($"SingleTrackProcessor: {singleResult[0].TotalDuration.TotalMilliseconds}ms");
            Assert.That(singleResult[0].TotalDuration.TotalMilliseconds, Is.EqualTo(expectedDuration),
                $"SingleTrackProcessor should use custom duration of {customDuration}ms per tone");
            
            // Test ReachSingleTrackProcessor with custom duration
            var reachResult = reachProcessor.Process(input, NumberFormats.Hex, NumberFormats.Hex);
            foreach (var seq in reachResult)
            {
                TestContext.WriteLine($"  {seq.Title}: {seq.TotalDuration.TotalMilliseconds}ms");
                Assert.That(seq.TotalDuration.TotalMilliseconds, Is.EqualTo(expectedDuration),
                    $"{seq.Title} should use custom duration timeline of {expectedDuration}ms total");
            }
        }

        [Test]
        public void Test_MultiTrack_Custom_Duration_200ms()
        {
            // Test MultiTrackProcessor with custom duration
            var customDuration = 200;
            var multiProcessor = new MultiTrackProcessor(customDuration);
            
            string input = "12+34"; // Two sequences with 2 tones each
            var expectedDuration = 2 * customDuration; // 2 * 200ms = 400ms per sequence
            
            TestContext.WriteLine($"=== MULTITRACK CUSTOM DURATION TEST: {customDuration}ms ===");
            
            var result = multiProcessor.Process(input, NumberFormats.Dec, NumberFormats.Dec);
            TestContext.WriteLine($"MultiTrackProcessor sequences: {result.Count}");
            
            // Should have 2 sequences (one for "12", one for "34")
            Assert.That(result, Has.Count.EqualTo(2));
            
            foreach (var seq in result)
            {
                TestContext.WriteLine($"  {seq.Title}: {seq.TotalDuration.TotalMilliseconds}ms, {seq.Tones.Count} tones");
                Assert.That(seq.TotalDuration.TotalMilliseconds, Is.EqualTo(expectedDuration),
                    $"Each sequence should have duration of {expectedDuration}ms");
                
                // Verify individual tone durations within each sequence
                foreach (var tone in seq.Tones)
                {
                    Assert.That(tone.Duration.TotalMilliseconds, Is.EqualTo(customDuration),
                        $"Each tone should have duration of {customDuration}ms");
                }
            }
        }

        [Test]
        public void Test_Simultaneous_Start_Times_Issue()
        {
            // This test demonstrates that tones should start at the same time positions
            // across all octave groups, and after the fix they maintain proper synchronization
            
            var processor = new ReachSingleTrackProcessor();
            string input = "24"; // Tone 2 (MidLow group), Tone 4 (High group) 
            
            var result = processor.Process(input, NumberFormats.Dec, NumberFormats.Dec);
            
            TestContext.WriteLine("=== SIMULTANEOUS START TIMES TEST ===");
            TestContext.WriteLine("Expected: All tones should start at positions 0ms and 300ms");
            TestContext.WriteLine("Current behavior: Extended durations prevent proper timing");
            
            foreach (var seq in result)
            {
                TestContext.WriteLine($"{seq.Title}:");
                for (int i = 0; i < seq.Tones.Count; i++)
                {
                    var tone = seq.Tones[i];
                    double startTime = i * 300; // Each position is 300ms apart
                    TestContext.WriteLine($"  Position {i} (t={startTime}ms): {tone.ObertonFrequencies[0]}Hz, duration={tone.Duration.TotalMilliseconds}ms");
                }
            }
            
            // After fix: All sequences have synchronized timeline duration
            double expectedTotalDuration = input.Length * 300; // 2 * 300ms = 600ms
            foreach (var seq in result)
            {
                Assert.That(seq.TotalDuration.TotalMilliseconds, Is.EqualTo(expectedTotalDuration),
                    "All octave groups should have synchronized timeline duration");
            }
        }

        [Test]
        public void Debug_HexFormat_ActualBehavior()
        {
            // Debug test to understand the actual behavior after the fix
            string input = "1234"; // One tone from each major group
            int baseDuration = DefaultBaseDurationMs;

            var result = _processor.Process(input, NumberFormats.Hex, NumberFormats.Hex);

            TestContext.WriteLine($"Input: {input}");
            TestContext.WriteLine($"Sequences: {result.Count}");

            for (int groupIndex = 0; groupIndex < result.Count; groupIndex++)
            {
                var seq = result[groupIndex];
                TestContext.WriteLine($"\n{seq.Title}: {seq.TotalDuration.TotalMilliseconds}ms total");
                TestContext.WriteLine($"  Tones: {seq.Tones.Count}");
                for (int i = 0; i < seq.Tones.Count; i++)
                {
                    var tone = seq.Tones[i];
                    TestContext.WriteLine($"    Position {i}: {tone.ObertonFrequencies[0]}Hz, {tone.Duration.TotalMilliseconds}ms");
                }
            }
        }

        [Test]
        public void Process_AllSequencesHaveSameLength()
        {
            // Arrange
            string input = "123456";

            // Act
            var result = _processor.Process(input, NumberFormats.Hex, NumberFormats.Hex);

            // Assert
            // All sequences should have the same number of tones (with silence where appropriate)
            int expectedLength = input.Length;
            foreach (var sequence in result)
            {
                Assert.That(sequence.Tones, Has.Count.EqualTo(expectedLength));
            }
        }
    }
}