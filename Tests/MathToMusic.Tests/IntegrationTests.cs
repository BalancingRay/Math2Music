using NUnit.Framework;
using MathToMusic;
using MathToMusic.Contracts;
using MathToMusic.Models;
using System.Linq;

namespace MathToMusic.Tests
{
    [TestFixture]
    public class IntegrationTests
    {
        [Test]
        public void Integration_123Plus456_CreatesExpectedChords()
        {
            // Arrange
            var processor = new MultiTrackProcessor();
            
            // Act
            var result = processor.Process("123+456", NumberFormats.Dec, NumberFormats.Dec);
            
            // Assert - Should create chords (1,4), (2,5), (3,6)
            Assert.That(result[0].Tones, Has.Count.EqualTo(3));
            
            // Chord 1: (1,4) = (180*1, 180*4) = (180, 720)
            var chord1 = result[0].Tones[0];
            Assert.That(chord1.ObertonFrequencies, Has.Length.EqualTo(2));
            Assert.That(chord1.ObertonFrequencies, Contains.Item(180.0));
            Assert.That(chord1.ObertonFrequencies, Contains.Item(720.0));
            
            // Chord 2: (2,5) = (360, 900)
            var chord2 = result[0].Tones[1];
            Assert.That(chord2.ObertonFrequencies, Has.Length.EqualTo(2));
            Assert.That(chord2.ObertonFrequencies, Contains.Item(360.0));
            Assert.That(chord2.ObertonFrequencies, Contains.Item(900.0));
            
            // Chord 3: (3,6) = (540, 1080)
            var chord3 = result[0].Tones[2];
            Assert.That(chord3.ObertonFrequencies, Has.Length.EqualTo(2));
            Assert.That(chord3.ObertonFrequencies, Contains.Item(540.0));
            Assert.That(chord3.ObertonFrequencies, Contains.Item(1080.0));
        }

        [Test]
        public void Integration_12Plus3456_HandlesDifferentLengths()
        {
            // Arrange
            var processor = new MultiTrackProcessor();
            
            // Act
            var result = processor.Process("12+3456", NumberFormats.Dec, NumberFormats.Dec);
            
            // Assert - Should create (1,3), (2,4), (5), (6)
            Assert.That(result[0].Tones, Has.Count.EqualTo(4));
            
            // First two positions have chords
            Assert.That(result[0].Tones[0].ObertonFrequencies, Has.Length.EqualTo(2));
            Assert.That(result[0].Tones[1].ObertonFrequencies, Has.Length.EqualTo(2));
            
            // Last two positions have single notes from longer sequence
            Assert.That(result[0].Tones[2].ObertonFrequencies, Has.Length.EqualTo(1));
            Assert.That(result[0].Tones[2].ObertonFrequencies[0], Is.EqualTo(900.0)); // '5'
            Assert.That(result[0].Tones[3].ObertonFrequencies, Has.Length.EqualTo(1));
            Assert.That(result[0].Tones[3].ObertonFrequencies[0], Is.EqualTo(1080.0)); // '6'
        }

        [Test]
        public void Integration_ABCPlusDEF_HexFormat()
        {
            // Arrange
            var processor = new MultiTrackProcessor();
            
            // Act
            var result = processor.Process("ABC+DEF", NumberFormats.Hex, NumberFormats.Hex);
            
            // Assert - Should create chords (A,D), (B,E), (C,F)
            Assert.That(result[0].Tones, Has.Count.EqualTo(3));
            
            // Chord 1: (A,D) = (10,13) = (1800, 2340)
            var chord1 = result[0].Tones[0];
            Assert.That(chord1.ObertonFrequencies, Contains.Item(1800.0)); // A = 10 * 180
            Assert.That(chord1.ObertonFrequencies, Contains.Item(2340.0)); // D = 13 * 180
            
            // Chord 2: (B,E) = (11,14) = (1980, 2520)
            var chord2 = result[0].Tones[1];
            Assert.That(chord2.ObertonFrequencies, Contains.Item(1980.0)); // B = 11 * 180
            Assert.That(chord2.ObertonFrequencies, Contains.Item(2520.0)); // E = 14 * 180
            
            // Chord 3: (C,F) = (12,15) = (2160, 2700)
            var chord3 = result[0].Tones[2];
            Assert.That(chord3.ObertonFrequencies, Contains.Item(2160.0)); // C = 12 * 180
            Assert.That(chord3.ObertonFrequencies, Contains.Item(2700.0)); // F = 15 * 180
        }

        [Test]
        public void Integration_MixedFormats_123PlusABCWithFormatConversion()
        {
            // Arrange
            var processor = new MultiTrackProcessor();
            
            // Act - Input "123+ABC", input format Dec, output format Hex
            // This should process "123" as decimal and "ABC" as decimal
            // But "ABC" will be invalid decimal chars, so will be skipped
            var result = processor.Process("123+ABC", NumberFormats.Hex, NumberFormats.Dec);
            
            // Assert - "123" (decimal) converts to "7B" (hex), "ABC" has no valid decimal digits
            // So we get tones from "7B" + empty sequence
            Assert.That(result[0].Tones, Has.Count.EqualTo(2));
            
            // Should have single frequencies (no chords) since second track is empty
            Assert.That(result[0].Tones[0].ObertonFrequencies, Has.Length.EqualTo(1)); // '7'
            Assert.That(result[0].Tones[1].ObertonFrequencies, Has.Length.EqualTo(1)); // 'B'
        }

        [Test]
        public void Integration_BinaryConversion_WithPolyphonic()
        {
            // Arrange
            var processor = new MultiTrackProcessor();
            
            // Act - Convert decimal to binary with polyphonic input
            var result = processor.Process("10+15", NumberFormats.Bin, NumberFormats.Dec);
            
            // Assert - "10" -> "1010", "15" -> "1111"
            Assert.That(result[0].Tones, Has.Count.EqualTo(4));
            
            // All positions should have chords since both binary results have same length
            for (int i = 0; i < 4; i++)
            {
                Assert.That(result[0].Tones[i].ObertonFrequencies, Has.Length.EqualTo(2));
            }
        }

        [Test]
        public void Integration_SingleTrackProcessor_StillWorksForNonPolyphonic()
        {
            // Arrange
            var singleProcessor = new SingleTrackProcessor();
            var multiProcessor = new MultiTrackProcessor();
            
            // Act
            var singleResult = ((ITonesProcessor)singleProcessor).Process("123", NumberFormats.Dec, NumberFormats.Dec);
            var multiResult = multiProcessor.Process("123", NumberFormats.Dec, NumberFormats.Dec);
            
            // Assert - Both should produce identical results for non-polyphonic input
            Assert.That(singleResult[0].Tones, Has.Count.EqualTo(multiResult[0].Tones.Count));
            for (int i = 0; i < singleResult[0].Tones.Count; i++)
            {
                Assert.That(singleResult[0].Tones[i].ObertonFrequencies[0], 
                           Is.EqualTo(multiResult[0].Tones[i].ObertonFrequencies[0]));
                Assert.That(singleResult[0].Tones[i].Duration, 
                           Is.EqualTo(multiResult[0].Tones[i].Duration));
            }
        }

        [Test]
        public void Integration_ProgramChoosesCorrectProcessor()
        {
            // Test that ExpressionParser.IsPolyphonic works correctly
            Assert.That(ExpressionParser.IsPolyphonic("123+456"), Is.True);
            Assert.That(ExpressionParser.IsPolyphonic("123"), Is.False);
            Assert.That(ExpressionParser.IsPolyphonic("ABC+DEF"), Is.True);
            Assert.That(ExpressionParser.IsPolyphonic("ABCDEF"), Is.False);
        }
    }
}