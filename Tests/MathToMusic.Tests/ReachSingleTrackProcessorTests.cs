using NUnit.Framework;
using MathToMusic;
using MathToMusic.Contracts;
using MathToMusic.Models;

namespace MathToMusic.Tests
{
    [TestFixture]
    public class ReachSingleTrackProcessorTests
    {
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
            int baseDuration = 300;
            
            // Act
            var result = _processor.Process(input, NumberFormats.Hex, NumberFormats.Hex);
            
            // Assert
            Assert.That(result, Has.Count.EqualTo(4));
            
            // Group 1 (tone 1): duration x8 = 2400ms
            var group1ActiveTone = result[0].Tones.First(t => t.ObertonFrequencies[0] != 0);
            Assert.That(group1ActiveTone.Duration.TotalMilliseconds, Is.EqualTo(baseDuration * 8));
            
            // Group 2 (tone 2): duration x4 = 1200ms
            var group2ActiveTone = result[1].Tones.First(t => t.ObertonFrequencies[0] != 0);
            Assert.That(group2ActiveTone.Duration.TotalMilliseconds, Is.EqualTo(baseDuration * 4));
            
            // Group 3 (tone 4): duration x2 = 600ms
            var group3ActiveTone = result[2].Tones.First(t => t.ObertonFrequencies[0] != 0);
            Assert.That(group3ActiveTone.Duration.TotalMilliseconds, Is.EqualTo(baseDuration * 2));
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
            int baseDuration = 300;
            
            // Act
            var result = _processor.Process(input, NumberFormats.Oct, NumberFormats.Oct);
            
            // Assert
            Assert.That(result, Has.Count.EqualTo(3));
            
            // Group 1 (tone 1): duration x4 = 1200ms
            var group1ActiveTone = result[0].Tones.First(t => t.ObertonFrequencies[0] != 0);
            Assert.That(group1ActiveTone.Duration.TotalMilliseconds, Is.EqualTo(baseDuration * 4));
            
            // Group 2 (tone 2): duration x2 = 600ms
            var group2ActiveTone = result[1].Tones.First(t => t.ObertonFrequencies[0] != 0);
            Assert.That(group2ActiveTone.Duration.TotalMilliseconds, Is.EqualTo(baseDuration * 2));
            
            // Group 3 (tone 4): duration x1 = 300ms
            var group3ActiveTone = result[2].Tones.First(t => t.ObertonFrequencies[0] != 0);
            Assert.That(group3ActiveTone.Duration.TotalMilliseconds, Is.EqualTo(baseDuration * 1));
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
            int baseDuration = 300;
            
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
            // Arrange
            string input = "1G2"; // G is invalid for most formats
            
            // Act
            var result = _processor.Process(input, NumberFormats.Dec, NumberFormats.Dec);
            
            // Assert
            Assert.That(result, Is.Not.Null);
            // Should only process '1' and '2', skipping 'G'
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
            int baseDuration = 300;
            
            // Act
            var result = _processor.Process(input, NumberFormats.Hex, NumberFormats.Hex);
            
            // Assert
            // Group 1: 2 tones * 8 * 300ms = 4800ms
            Assert.That(result[0].TotalDuration.TotalMilliseconds, Is.EqualTo(2 * 8 * baseDuration));
            
            // Group 2: 2 tones * 4 * 300ms = 2400ms
            Assert.That(result[1].TotalDuration.TotalMilliseconds, Is.EqualTo(2 * 4 * baseDuration));
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