using MathToMusic.Contracts;
using MathToMusic.Models;
using MathToMusic.Processors;
using NUnit.Framework;

namespace MathToMusic.Tests
{
    [TestFixture]
    public class SingleTrackProcessorTests
    {
        private const int DefaultBaseDurationMs = 300; // Default base duration for processors
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
            // Arrange - using characters not in any supported format
            string input = "1X2"; // X is invalid for any supported format (beyond Base32's V)

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
            int baseDuration = DefaultBaseDurationMs; // milliseconds per tone

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
            int expectedDuration = DefaultBaseDurationMs;

            // Act
            var result = _processor.Process(input, NumberFormats.Dec, NumberFormats.Dec);

            // Assert
            foreach (var tone in result[0].Tones)
            {
                Assert.That(tone.Duration.TotalMilliseconds, Is.EqualTo(expectedDuration));
            }
        }

        [Test]
        public void Process_DecimalToBinary_ConvertsCorrectly()
        {
            // Arrange
            string input = "10"; // Decimal 10 should convert to binary 1010

            // Act
            var result = _processor.Process(input, NumberFormats.Bin, NumberFormats.Dec);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result[0].Tones, Has.Count.EqualTo(4)); // "1010" has 4 characters

            // Check tone values: '1'=1, '0'=0, '1'=1, '0'=0
            Assert.That(result[0].Tones[0].ObertonFrequencies[0], Is.EqualTo(180 * 1)); // '1'
            Assert.That(result[0].Tones[1].ObertonFrequencies[0], Is.EqualTo(180 * 0)); // '0' 
            Assert.That(result[0].Tones[2].ObertonFrequencies[0], Is.EqualTo(180 * 1)); // '1'
            Assert.That(result[0].Tones[3].ObertonFrequencies[0], Is.EqualTo(180 * 0)); // '0'
        }

        [Test]
        public void Process_BinaryToDecimal_ConvertsCorrectly()
        {
            // Arrange
            string input = "1010"; // Binary 1010 should convert to decimal 10

            // Act
            var result = _processor.Process(input, NumberFormats.Dec, NumberFormats.Bin);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result[0].Tones, Has.Count.EqualTo(2)); // "10" has 2 characters

            // Check tone values: '1'=1, '0'=0
            Assert.That(result[0].Tones[0].ObertonFrequencies[0], Is.EqualTo(180 * 1)); // '1'
            Assert.That(result[0].Tones[1].ObertonFrequencies[0], Is.EqualTo(180 * 0)); // '0'
        }

        [Test]
        public void SingleTrackProcessor_CustomDuration_250ms()
        {
            // Test SingleTrackProcessor with custom 250ms base duration
            var customDuration = 250;
            var customProcessor = new SingleTrackProcessor(customDuration);
            
            string input = "12345"; // 5 tones
            var expectedTotalDuration = input.Length * customDuration; // 5 * 250ms = 1250ms
            
            // Act
            var result = ((ITonesProcessor)customProcessor).Process(input, NumberFormats.Dec, NumberFormats.Dec);
            
            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].Tones, Has.Count.EqualTo(5));
            
            // Check total duration
            Assert.That(result[0].TotalDuration.TotalMilliseconds, Is.EqualTo(expectedTotalDuration));
            
            // Check individual tone durations
            foreach (var tone in result[0].Tones)
            {
                Assert.That(tone.Duration.TotalMilliseconds, Is.EqualTo(customDuration),
                    $"Each tone should have custom duration of {customDuration}ms");
            }
            
            // Verify frequencies are still correct (180Hz * tone value)
            Assert.That(result[0].Tones[0].ObertonFrequencies[0], Is.EqualTo(180 * 1)); // '1'
            Assert.That(result[0].Tones[1].ObertonFrequencies[0], Is.EqualTo(180 * 2)); // '2'
            Assert.That(result[0].Tones[2].ObertonFrequencies[0], Is.EqualTo(180 * 3)); // '3'
            Assert.That(result[0].Tones[3].ObertonFrequencies[0], Is.EqualTo(180 * 4)); // '4'
            Assert.That(result[0].Tones[4].ObertonFrequencies[0], Is.EqualTo(180 * 5)); // '5'
        }

        [Test]
        public void SingleTrackProcessor_CustomDurationAndFrequency()
        {
            // Test SingleTrackProcessor with both custom duration and frequency
            var customDuration = 400;
            var customBaseFreq = 220.0; // A3 note
            var customProcessor = new SingleTrackProcessor(customDuration, customBaseFreq);
            
            string input = "123";
            
            // Act
            var result = ((ITonesProcessor)customProcessor).Process(input, NumberFormats.Dec, NumberFormats.Dec);
            
            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result[0].Tones, Has.Count.EqualTo(3));
            
            // Check durations
            foreach (var tone in result[0].Tones)
            {
                Assert.That(tone.Duration.TotalMilliseconds, Is.EqualTo(customDuration));
            }
            
            // Check frequencies use custom base frequency
            Assert.That(result[0].Tones[0].ObertonFrequencies[0], Is.EqualTo(customBaseFreq * 1)); // '1'
            Assert.That(result[0].Tones[1].ObertonFrequencies[0], Is.EqualTo(customBaseFreq * 2)); // '2'
            Assert.That(result[0].Tones[2].ObertonFrequencies[0], Is.EqualTo(customBaseFreq * 3)); // '3'
            
            // Check total duration
            var expectedTotalDuration = 3 * customDuration;
            Assert.That(result[0].TotalDuration.TotalMilliseconds, Is.EqualTo(expectedTotalDuration));
        }

        [Test]
        public void Process_Base32Input_ReturnsCorrectTones()
        {
            // Arrange - test Base32 characters including higher values
            string input = "0123456789ABCDEFGHIJKLMNOPQRSTUV"; // All valid Base32 characters (0-31)

            // Act
            var result = _processor.Process(input, NumberFormats.Base32, NumberFormats.Base32);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].Tones, Has.Count.EqualTo(32)); // All 32 characters

            // Check specific tone values for key characters
            Assert.That(result[0].Tones[0].ObertonFrequencies[0], Is.EqualTo(180 * 0));  // '0' = 0
            Assert.That(result[0].Tones[9].ObertonFrequencies[0], Is.EqualTo(180 * 9));  // '9' = 9
            Assert.That(result[0].Tones[10].ObertonFrequencies[0], Is.EqualTo(180 * 10)); // 'A' = 10
            Assert.That(result[0].Tones[15].ObertonFrequencies[0], Is.EqualTo(180 * 15)); // 'F' = 15
            Assert.That(result[0].Tones[16].ObertonFrequencies[0], Is.EqualTo(180 * 16)); // 'G' = 16 (new for Base32)
            Assert.That(result[0].Tones[25].ObertonFrequencies[0], Is.EqualTo(180 * 25)); // 'P' = 25
            Assert.That(result[0].Tones[31].ObertonFrequencies[0], Is.EqualTo(180 * 31)); // 'V' = 31 (max Base32)
        }

        [Test]
        public void Process_Base32ToDecimal_ConvertsCorrectly()
        {
            // Arrange
            string input = "V"; // Base32 'V' = 31 decimal

            // Act
            var result = _processor.Process(input, NumberFormats.Dec, NumberFormats.Base32);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result[0].Tones, Has.Count.EqualTo(2)); // "31" has 2 characters

            // Check tone values: '3'=3, '1'=1
            Assert.That(result[0].Tones[0].ObertonFrequencies[0], Is.EqualTo(180 * 3)); // '3'
            Assert.That(result[0].Tones[1].ObertonFrequencies[0], Is.EqualTo(180 * 1)); // '1'
        }

        [Test]
        public void Process_DecimalToBase32_ConvertsCorrectly()
        {
            // Arrange
            string input = "31"; // Decimal 31 should convert to Base32 'V'

            // Act
            var result = _processor.Process(input, NumberFormats.Base32, NumberFormats.Dec);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result[0].Tones, Has.Count.EqualTo(1)); // "V" has 1 character

            // Check tone value: 'V'=31
            Assert.That(result[0].Tones[0].ObertonFrequencies[0], Is.EqualTo(180 * 31)); // 'V' = 31
        }
    }
}