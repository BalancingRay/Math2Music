using MathToMusic.Contracts;
using MathToMusic.Models;
using MathToMusic.Processors;
using NUnit.Framework;

namespace MathToMusic.Tests
{
    [TestFixture]
    [Ignore("123+456 now work as two simultaneousely sequentions")]
    public class MultiTrackProcessorTests
    {
        private ITonesProcessor _processor;

        [SetUp]
        public void Setup()
        {
            _processor = new MultiTrackProcessor();
        }

        [Test]
        public void Process_SimplePolyphonic_CreatesHarmonicChords()
        {
            // Arrange
            string input = "123+456";

            // Act
            var result = _processor.Process(input, NumberFormats.Dec, NumberFormats.Dec);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].Title, Is.EqualTo("Harmonic"));
            Assert.That(result[0].Tones, Has.Count.EqualTo(3));

            // Check first chord: (1,4) = frequencies (180*1, 180*4) = (180, 720)
            Assert.That(result[0].Tones[0].ObertonFrequencies, Has.Length.EqualTo(2));
            Assert.That(result[0].Tones[0].ObertonFrequencies, Contains.Item(180.0)); // '1' = 1 * 180
            Assert.That(result[0].Tones[0].ObertonFrequencies, Contains.Item(720.0)); // '4' = 4 * 180

            // Check second chord: (2,5) = frequencies (360, 900)
            Assert.That(result[0].Tones[1].ObertonFrequencies, Has.Length.EqualTo(2));
            Assert.That(result[0].Tones[1].ObertonFrequencies, Contains.Item(360.0)); // '2' = 2 * 180
            Assert.That(result[0].Tones[1].ObertonFrequencies, Contains.Item(900.0)); // '5' = 5 * 180

            // Check third chord: (3,6) = frequencies (540, 1080)
            Assert.That(result[0].Tones[2].ObertonFrequencies, Has.Length.EqualTo(2));
            Assert.That(result[0].Tones[2].ObertonFrequencies, Contains.Item(540.0)); // '3' = 3 * 180
            Assert.That(result[0].Tones[2].ObertonFrequencies, Contains.Item(1080.0)); // '6' = 6 * 180
        }

        [Test]
        public void Process_DifferentLengthSequences_HandlesShorterSequence()
        {
            // Arrange
            string input = "12+3456";

            // Act
            var result = _processor.Process(input, NumberFormats.Dec, NumberFormats.Dec);

            // Assert
            Assert.That(result[0].Tones, Has.Count.EqualTo(4));

            // First two positions have both sequences: (1,3), (2,4)
            Assert.That(result[0].Tones[0].ObertonFrequencies, Has.Length.EqualTo(2));
            Assert.That(result[0].Tones[1].ObertonFrequencies, Has.Length.EqualTo(2));

            // Last two positions have only the longer sequence: (5), (6)
            Assert.That(result[0].Tones[2].ObertonFrequencies, Has.Length.EqualTo(1));
            Assert.That(result[0].Tones[2].ObertonFrequencies[0], Is.EqualTo(900.0)); // '5' = 5 * 180
            Assert.That(result[0].Tones[3].ObertonFrequencies, Has.Length.EqualTo(1));
            Assert.That(result[0].Tones[3].ObertonFrequencies[0], Is.EqualTo(1080.0)); // '6' = 6 * 180
        }

        [Test]
        public void Process_HexFormat_ProcessesCorrectly()
        {
            // Arrange
            string input = "ABC+DEF";

            // Act
            var result = _processor.Process(input, NumberFormats.Hex, NumberFormats.Hex);

            // Assert
            Assert.That(result[0].Tones, Has.Count.EqualTo(3));

            // Check first chord: (A,D) = (10,13) = frequencies (1800, 2340)
            Assert.That(result[0].Tones[0].ObertonFrequencies, Contains.Item(1800.0)); // 'A' = 10 * 180
            Assert.That(result[0].Tones[0].ObertonFrequencies, Contains.Item(2340.0)); // 'D' = 13 * 180

            // Check second chord: (B,E) = (11,14) = frequencies (1980, 2520)
            Assert.That(result[0].Tones[1].ObertonFrequencies, Contains.Item(1980.0)); // 'B' = 11 * 180
            Assert.That(result[0].Tones[1].ObertonFrequencies, Contains.Item(2520.0)); // 'E' = 14 * 180

            // Check third chord: (C,F) = (12,15) = frequencies (2160, 2700)
            Assert.That(result[0].Tones[2].ObertonFrequencies, Contains.Item(2160.0)); // 'C' = 12 * 180
            Assert.That(result[0].Tones[2].ObertonFrequencies, Contains.Item(2700.0)); // 'F' = 15 * 180
        }

        [Test]
        public void Process_MixedFormatsWithConversion_ProcessesCorrectly()
        {
            // Arrange - Input in decimal but process as binary output
            string input = "10+11"; // decimal 10,11 -> binary "1010","1011"

            // Act
            var result = _processor.Process(input, NumberFormats.Bin, NumberFormats.Dec);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result[0].Tones, Has.Count.EqualTo(4)); // "1010" and "1011" both have 4 characters

            // All positions should have chords since both converted sequences have same length
            Assert.That(result[0].Tones[0].ObertonFrequencies, Has.Length.EqualTo(2)); // (1,1)
            Assert.That(result[0].Tones[1].ObertonFrequencies, Has.Length.EqualTo(2)); // (0,0) 
            Assert.That(result[0].Tones[2].ObertonFrequencies, Has.Length.EqualTo(2)); // (1,1)
            Assert.That(result[0].Tones[3].ObertonFrequencies, Has.Length.EqualTo(2)); // (0,1)
        }

        [Test]
        public void Process_ThreeOrMoreTerms_CombinesAllTracks()
        {
            // Arrange
            string input = "1+2+3";

            // Act
            var result = _processor.Process(input, NumberFormats.Dec, NumberFormats.Dec);

            // Assert
            Assert.That(result[0].Tones, Has.Count.EqualTo(1));

            // Should have chord with all three frequencies: (1,2,3) = (180, 360, 540)
            Assert.That(result[0].Tones[0].ObertonFrequencies, Has.Length.EqualTo(3));
            Assert.That(result[0].Tones[0].ObertonFrequencies, Contains.Item(180.0));  // '1'
            Assert.That(result[0].Tones[0].ObertonFrequencies, Contains.Item(360.0));  // '2'
            Assert.That(result[0].Tones[0].ObertonFrequencies, Contains.Item(540.0));  // '3'
        }

        [Test]
        public void Process_EmptyInput_ReturnsEmptySequence()
        {
            // Act
            var result = _processor.Process("", NumberFormats.Dec, NumberFormats.Dec);

            // Assert
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].Title, Is.EqualTo("Empty"));
            Assert.That(result[0].Tones, Has.Count.EqualTo(0));
        }

        [Test]
        public void Process_NullInput_ReturnsEmptySequence()
        {
            // Act
            var result = _processor.Process(null, NumberFormats.Dec, NumberFormats.Dec);

            // Assert
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].Title, Is.EqualTo("Empty"));
            Assert.That(result[0].Tones, Has.Count.EqualTo(0));
        }

        [Test]
        public void Process_InvalidCharacters_SkipsInvalidChars()
        {
            // Arrange - G is invalid for decimal
            string input = "1G2+3H4";

            // Act
            var result = _processor.Process(input, NumberFormats.Dec, NumberFormats.Dec);

            // Assert - Should process valid chars: "12" + "34"
            Assert.That(result[0].Tones, Has.Count.EqualTo(2));

            // First chord: (1,3)
            Assert.That(result[0].Tones[0].ObertonFrequencies, Contains.Item(180.0));
            Assert.That(result[0].Tones[0].ObertonFrequencies, Contains.Item(540.0));

            // Second chord: (2,4)
            Assert.That(result[0].Tones[1].ObertonFrequencies, Contains.Item(360.0));
            Assert.That(result[0].Tones[1].ObertonFrequencies, Contains.Item(720.0));
        }

        [Test]
        public void Process_CalculatesTotalDurationCorrectly()
        {
            // Arrange
            string input = "12+34";
            int baseDuration = 300; // milliseconds per tone

            // Act
            var result = _processor.Process(input, NumberFormats.Dec, NumberFormats.Dec);

            // Assert - 2 chords * 300ms each = 600ms total
            Assert.That(result[0].TotalDuration.TotalMilliseconds, Is.EqualTo(baseDuration * 2));
        }
    }
}