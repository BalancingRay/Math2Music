using NUnit.Framework;
using MathToMusic;
using MathToMusic.Contracts;
using MathToMusic.Models;

namespace MathToMusic.Tests
{
    [TestFixture]
    public class SingleTrackProcessorTests
    {
        private ITonesProcessor _processor;

        [SetUp]
        public void Setup()
        {
            _processor = new SingleTrackProcessor();
        }

        [Test]
        public void Process_SameInputOutputFormat_ReturnsCorrectTones()
        {
            // Arrange
            string input = "123";
            
            // Act
            var result = _processor.Process(input, NumberFormats.Dec, NumberFormats.Dec);
            
            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].Title, Is.EqualTo("Single"));
            Assert.That(result[0].Tones, Has.Count.EqualTo(3));
            
            // Check tone values: '1'=1, '2'=2, '3'=3
            Assert.That(result[0].Tones[0].ObertonFrequencies[0], Is.EqualTo(180 * 1)); // base 180Hz * 1
            Assert.That(result[0].Tones[1].ObertonFrequencies[0], Is.EqualTo(180 * 2)); // base 180Hz * 2  
            Assert.That(result[0].Tones[2].ObertonFrequencies[0], Is.EqualTo(180 * 3)); // base 180Hz * 3
        }

        [Test]
        public void Process_HexadecimalInput_ReturnsCorrectTones()
        {
            // Arrange
            string input = "ABC";
            
            // Act
            var result = _processor.Process(input, NumberFormats.Hex, NumberFormats.Hex);
            
            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result[0].Tones, Has.Count.EqualTo(3));
            
            // Check tone values: 'A'=10, 'B'=11, 'C'=12
            Assert.That(result[0].Tones[0].ObertonFrequencies[0], Is.EqualTo(180 * 10));
            Assert.That(result[0].Tones[1].ObertonFrequencies[0], Is.EqualTo(180 * 11));
            Assert.That(result[0].Tones[2].ObertonFrequencies[0], Is.EqualTo(180 * 12));
        }

        [Test]
        public void Process_InvalidCharacters_SkipsInvalidChars()
        {
            // Arrange
            string input = "1G2"; // G is invalid for decimal
            
            // Act
            var result = _processor.Process(input, NumberFormats.Dec, NumberFormats.Dec);
            
            // Assert
            Assert.That(result[0].Tones, Has.Count.EqualTo(2)); // Only '1' and '2' should produce tones
        }

        [Test]
        public void Process_EmptyInput_ReturnsEmptyToneSequence()
        {
            // Arrange
            string input = "";
            
            // Act
            var result = _processor.Process(input, NumberFormats.Dec, NumberFormats.Dec);
            
            // Assert
            Assert.That(result[0].Tones, Has.Count.EqualTo(0));
        }

        [Test]
        public void Process_BinaryToHex_PerformsBinaryGroupConversion()
        {
            // Arrange
            string input = "10101111"; // Binary that converts to AF in hex
            
            // Act
            var result = _processor.Process(input, NumberFormats.Hex, NumberFormats.Bin);
            
            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result[0].Tones, Has.Count.EqualTo(2));
            
            // The algorithm processes right-to-left: 1111 (F=15) then 1010 (A=10)
            // But inserts at position 0, so the order becomes F, A
            Assert.That(result[0].Tones[0].ObertonFrequencies[0], Is.EqualTo(180 * 15)); // 'F'
            Assert.That(result[0].Tones[1].ObertonFrequencies[0], Is.EqualTo(180 * 10)); // 'A'
        }

        [Test]
        public void Process_BinaryToOctal_PerformsBinaryGroupConversion()
        {
            // Arrange
            string input = "101011"; // Binary: processes right-to-left in groups of 3
            
            // Act  
            var result = _processor.Process(input, NumberFormats.Oct, NumberFormats.Bin);
            
            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result[0].Tones, Has.Count.EqualTo(2));
            
            // The algorithm processes right-to-left: 011 (3) then 101 (5)
            // Results in tones for 3, then 5
            Assert.That(result[0].Tones[0].ObertonFrequencies[0], Is.EqualTo(180 * 3)); // '3'
            Assert.That(result[0].Tones[1].ObertonFrequencies[0], Is.EqualTo(180 * 5)); // '5'
        }

        [Test]
        public void Process_TotalDuration_CalculatedCorrectly()
        {
            // Arrange
            string input = "12";
            int baseDuration = 300; // milliseconds per tone
            
            // Act
            var result = _processor.Process(input, NumberFormats.Dec, NumberFormats.Dec);
            
            // Assert
            Assert.That(result[0].TotalDuration.TotalMilliseconds, Is.EqualTo(baseDuration * 2));
        }

        [Test]
        public void Process_ToneDurations_AllHaveBaseDuration()
        {
            // Arrange
            string input = "123";
            int expectedDuration = 300;
            
            // Act
            var result = _processor.Process(input, NumberFormats.Dec, NumberFormats.Dec);
            
            // Assert
            foreach (var tone in result[0].Tones)
            {
                Assert.That(tone.Duration.TotalMilliseconds, Is.EqualTo(expectedDuration));
            }
        }
    }
}