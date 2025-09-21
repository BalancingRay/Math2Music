using MathToMusic.Contracts;
using MathToMusic.Models;
using MathToMusic.Outputs;
using MathToMusic.Processors;
using NUnit.Framework;

namespace MathToMusic.Tests
{
    [TestFixture]
    public class PolyphonicIntegrationTests
    {
        private const int DefaultBaseDurationMs = 300; // Default base duration for processors
        [Test]
        public void ReachSingleTrackProcessor_Integration_CreatesPolyphonicWav()
        {
            // Arrange
            var processor = new ReachSingleTrackProcessor();
            var output = new WavFileOutput();
            string input = "123456789ABCDEF"; // Full hex range

            // Act
            var result = processor.Process(input, NumberFormats.Hex, NumberFormats.Hex);
            output.Send(result);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(4)); // 4 octave groups

            // Verify different duration multipliers are applied
            Assert.That(result[0].Title, Is.EqualTo("Octave_Low"));
            Assert.That(result[1].Title, Is.EqualTo("Octave_MidLow"));
            Assert.That(result[2].Title, Is.EqualTo("Octave_MidHigh"));
            Assert.That(result[3].Title, Is.EqualTo("Octave_High"));

            // After fix: All groups now have synchronized timeline duration
            var expectedDuration = input.Length * DefaultBaseDurationMs; // sequence length * base duration
            foreach (var seq in result)
            {
                Assert.That(seq.TotalDuration.TotalMilliseconds, Is.EqualTo(expectedDuration),
                    $"All octave groups should have synchronized timeline duration of {expectedDuration}ms");
            }
        }

        [Test]
        public void ReachSingleTrackProcessor_OctFormat_CreatesPolyphonicOutput()
        {
            // Arrange
            var processor = new ReachSingleTrackProcessor();
            string input = "1234567"; // Oct range

            // Act
            var result = processor.Process(input, NumberFormats.Oct, NumberFormats.Oct);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(3)); // 3 octave groups for OCT

            // After fix: All groups now have synchronized timeline duration
            var expectedDuration = input.Length * DefaultBaseDurationMs; // sequence length * base duration
            foreach (var seq in result)
            {
                Assert.That(seq.TotalDuration.TotalMilliseconds, Is.EqualTo(expectedDuration),
                    $"All octave groups should have synchronized timeline duration");
            }
        }

        [Test]
        public void ReachSingleTrackProcessor_QadFormat_CreatesPolyphonicOutput()
        {
            // Arrange
            var processor = new ReachSingleTrackProcessor();
            string input = "123"; // Quad range

            // Act
            var result = processor.Process(input, NumberFormats.Qad, NumberFormats.Qad);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(2)); // 2 octave groups for QUAD

            // After fix: All groups now have synchronized timeline duration
            var expectedDuration = input.Length * DefaultBaseDurationMs; // sequence length * base duration
            foreach (var seq in result)
            {
                Assert.That(seq.TotalDuration.TotalMilliseconds, Is.EqualTo(expectedDuration),
                    $"All octave groups should have synchronized timeline duration");
            }
        }

        [Test]
        public void CompareProcessors_SingleVsReach_ProduceDifferentOutputStructure()
        {
            // Arrange
            var singleProcessor = new SingleTrackProcessor();
            var reachProcessor = new ReachSingleTrackProcessor();
            string input = "123456";

            // Act
            var singleResult = ((ITonesProcessor)singleProcessor).Process(input, NumberFormats.Hex, NumberFormats.Hex);
            var reachResult = reachProcessor.Process(input, NumberFormats.Hex, NumberFormats.Hex);

            // Assert
            // SingleTrackProcessor creates 1 sequence (monophonic)
            Assert.That(singleResult, Has.Count.EqualTo(1));
            Assert.That(singleResult[0].Title, Is.EqualTo("Single"));

            // ReachSingleTrackProcessor creates 4 sequences (polyphonic by octave groups)
            Assert.That(reachResult, Has.Count.EqualTo(4));

            var reachTitles = reachResult.Select(r => r.Title).ToList();
            Assert.That(reachTitles, Contains.Item("Octave_Low"));
            Assert.That(reachTitles, Contains.Item("Octave_MidLow"));
            Assert.That(reachTitles, Contains.Item("Octave_MidHigh"));
            Assert.That(reachTitles, Contains.Item("Octave_High"));
        }

        [TestCase("1", ExpectedResult = 1)]  // HEX: single tone gets remaining sequence duration (1*DefaultBaseDurationMs)
        [TestCase("2", ExpectedResult = 1)]  // HEX: single tone gets remaining sequence duration (1*DefaultBaseDurationMs)  
        [TestCase("4", ExpectedResult = 1)]  // HEX: single tone gets remaining sequence duration (1*DefaultBaseDurationMs)
        [TestCase("8", ExpectedResult = 1)]  // HEX: single tone gets remaining sequence duration (1*DefaultBaseDurationMs)
        public int HexFormat_IndividualTones_GetCorrectDurationMultipliers(string input)
        {
            // After fix: Individual tones get the remaining sequence duration from their position
            // For single character input, this is always 1*baseDuration = DefaultBaseDurationMs
            
            // Arrange
            var processor = new ReachSingleTrackProcessor();
            int baseDuration = DefaultBaseDurationMs;

            // Act
            var result = processor.Process(input, NumberFormats.Hex, NumberFormats.Hex);

            // Find the sequence with the active tone (non-zero frequency)
            var activeSequence = result.FirstOrDefault(s => s.Tones.Any(t => t.ObertonFrequencies[0] != 0));
            Assert.That(activeSequence, Is.Not.Null);

            var activeTone = activeSequence.Tones.First(t => t.ObertonFrequencies[0] != 0);

            // Return the duration multiplier (should always be 1 for single characters)
            return (int)(activeTone.Duration.TotalMilliseconds / baseDuration);
        }
    }
}