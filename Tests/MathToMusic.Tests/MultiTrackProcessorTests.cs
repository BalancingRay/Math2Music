using MathToMusic.Contracts;
using MathToMusic.Inputs;
using MathToMusic.Models;
using NUnit.Framework;

namespace MathToMusic.Tests
{
    [TestFixture]
    public class MultiTrackProcessorTests
    {
        private ITonesProcessor _processor;

        [SetUp]
        public void Setup()
        {
            _processor = new MultiTrackProcessor_AI_Default_Implementation();
        }

        [Test]
        public void Process_LongInput_CreatesMultipleTracks()
        {
            // Arrange
            string input = "123456"; // Long enough to split into two tracks

            // Act
            var result = _processor.Process(input, NumberFormats.Dec, NumberFormats.Dec);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2)); // Should create two tracks

            // Verify both tracks have content
            Assert.That(result[0].Tones.Count, Is.GreaterThan(0));
            Assert.That(result[1].Tones.Count, Is.GreaterThan(0));

            // Verify track titles
            Assert.That(result[0].Title, Is.EqualTo("Track1 - Low"));
            Assert.That(result[1].Title, Is.EqualTo("Track2 - High"));
        }

        [Test]
        public void Process_ShortInput_FallsBackToSingleTrack()
        {
            // Arrange
            string input = "1"; // Too short to split

            // Act
            var result = _processor.Process(input, NumberFormats.Dec, NumberFormats.Dec);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(1)); // Should fall back to single track
            Assert.That(result[0].Title, Is.EqualTo("Single")); // SingleTrackProcessor title
        }

        [Test]
        public void Process_EmptyInput_FallsBackToSingleTrack()
        {
            // Arrange
            string input = "";

            // Act
            var result = _processor.Process(input, NumberFormats.Dec, NumberFormats.Dec);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(1)); // Should fall back to single track
            Assert.That(result[0].Tones.Count, Is.EqualTo(0)); // Empty tones
        }

        [Test]
        public void Process_MultiTrack_HasDifferentFrequencies()
        {
            // Arrange
            string input = "1234"; // Will be split into "12" and "34"

            // Act
            var result = _processor.Process(input, NumberFormats.Dec, NumberFormats.Dec);

            // Assert
            Assert.That(result.Count, Is.EqualTo(2));

            // Track 1 should have base frequency 180Hz
            // Track 2 should have base frequency 360Hz (double)
            var track1Tone1 = result[0].Tones[0]; // '1' -> 180 * 1 = 180Hz
            var track2Tone1 = result[1].Tones[0]; // '3' -> 360 * 3 = 1080Hz

            Assert.That(track1Tone1.ObertonFrequencies[0], Is.EqualTo(180 * 1));
            Assert.That(track2Tone1.ObertonFrequencies[0], Is.EqualTo(360 * 3));
        }

        [Test]
        public void Process_WithWavOutput_CreatesPolyphonicWav()
        {
            // Arrange
            string input = "12345678"; // Long input to create multiple tracks
            var processor = new MultiTrackProcessor_AI_Default_Implementation();
            var wavOutput = new WavFileOutput();

            // Act
            var sequences = processor.Process(input, NumberFormats.Dec, NumberFormats.Dec);

            // Assert - Should create polyphonic output
            Assert.That(sequences.Count, Is.EqualTo(2));

            // Test that WavOutput handles polyphonic correctly
            Assert.DoesNotThrow(() => wavOutput.Send(sequences));
        }

        [Test]
        public void Process_BinaryToHex_MultiTrack_WorksCorrectly()
        {
            // Arrange
            string input = "11110000"; // Binary that will create multiple tracks

            // Act
            var result = _processor.Process(input, NumberFormats.Hex, NumberFormats.Bin);

            // Assert
            Assert.That(result.Count, Is.EqualTo(2));

            // Both tracks should have tones generated
            Assert.That(result[0].Tones.Count, Is.GreaterThan(0));
            Assert.That(result[1].Tones.Count, Is.GreaterThan(0));
        }

        [Test]
        public void Process_TotalDuration_CalculatedCorrectly()
        {
            // Arrange
            string input = "1234";

            // Act
            var result = _processor.Process(input, NumberFormats.Dec, NumberFormats.Dec);

            // Assert
            Assert.That(result.Count, Is.EqualTo(2));

            // Each track should have a total duration > 0
            Assert.That(result[0].TotalDuration, Is.GreaterThan(TimeSpan.Zero));
            Assert.That(result[1].TotalDuration, Is.GreaterThan(TimeSpan.Zero));
        }
    }
}