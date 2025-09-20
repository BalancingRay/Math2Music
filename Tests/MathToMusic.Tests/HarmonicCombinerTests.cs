using NUnit.Framework;
using MathToMusic.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MathToMusic.Tests
{
    [TestFixture]
    public class HarmonicCombinerTests
    {
        [Test]
        public void CombineHarmonically_EqualLengthSequences_CreatesChords()
        {
            // Arrange
            var seq1 = new Sequiention
            {
                Tones = new List<Tone>
                {
                    new Tone(180, 300), // frequency 180, duration 300ms
                    new Tone(360, 300)  // frequency 360, duration 300ms
                },
                Title = "Track1",
                TotalDuration = TimeSpan.FromMilliseconds(600)
            };

            var seq2 = new Sequiention
            {
                Tones = new List<Tone>
                {
                    new Tone(720, 300), // frequency 720, duration 300ms
                    new Tone(900, 300)  // frequency 900, duration 300ms
                },
                Title = "Track2",
                TotalDuration = TimeSpan.FromMilliseconds(600)
            };

            var sequences = new List<Sequiention> { seq1, seq2 };

            // Act
            var result = HarmonicCombiner.CombineHarmonically(sequences);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Title, Is.EqualTo("Harmonic"));
            Assert.That(result.Tones, Has.Count.EqualTo(2));

            // First chord: (180, 720)
            Assert.That(result.Tones[0].ObertonFrequencies, Has.Length.EqualTo(2));
            Assert.That(result.Tones[0].ObertonFrequencies, Contains.Item(180.0));
            Assert.That(result.Tones[0].ObertonFrequencies, Contains.Item(720.0));
            Assert.That(result.Tones[0].Duration.TotalMilliseconds, Is.EqualTo(300));

            // Second chord: (360, 900)
            Assert.That(result.Tones[1].ObertonFrequencies, Has.Length.EqualTo(2));
            Assert.That(result.Tones[1].ObertonFrequencies, Contains.Item(360.0));
            Assert.That(result.Tones[1].ObertonFrequencies, Contains.Item(900.0));
            Assert.That(result.Tones[1].Duration.TotalMilliseconds, Is.EqualTo(300));
        }

        [Test]
        public void CombineHarmonically_DifferentLengthSequences_HandlesLengthDifference()
        {
            // Arrange - first sequence is shorter
            var seq1 = new Sequiention
            {
                Tones = new List<Tone>
                {
                    new Tone(180, 300), // frequency 180
                    new Tone(360, 300)  // frequency 360
                },
                Title = "Track1",
                TotalDuration = TimeSpan.FromMilliseconds(600)
            };

            var seq2 = new Sequiention
            {
                Tones = new List<Tone>
                {
                    new Tone(720, 300), // frequency 720
                    new Tone(900, 300), // frequency 900
                    new Tone(1080, 300), // frequency 1080
                    new Tone(1260, 300)  // frequency 1260
                },
                Title = "Track2",
                TotalDuration = TimeSpan.FromMilliseconds(1200)
            };

            var sequences = new List<Sequiention> { seq1, seq2 };

            // Act
            var result = HarmonicCombiner.CombineHarmonically(sequences);

            // Assert
            Assert.That(result.Tones, Has.Count.EqualTo(4));

            // First two positions have both tracks
            Assert.That(result.Tones[0].ObertonFrequencies, Has.Length.EqualTo(2));
            Assert.That(result.Tones[1].ObertonFrequencies, Has.Length.EqualTo(2));

            // Last two positions have only the longer sequence
            Assert.That(result.Tones[2].ObertonFrequencies, Has.Length.EqualTo(1));
            Assert.That(result.Tones[2].ObertonFrequencies[0], Is.EqualTo(1080));
            Assert.That(result.Tones[3].ObertonFrequencies, Has.Length.EqualTo(1));
            Assert.That(result.Tones[3].ObertonFrequencies[0], Is.EqualTo(1260));
        }

        [Test]
        public void CombineHarmonically_SingleSequence_ReturnsSameSequence()
        {
            // Arrange
            var seq = new Sequiention
            {
                Tones = new List<Tone>
                {
                    new Tone(180, 300),
                    new Tone(360, 300)
                },
                Title = "Single",
                TotalDuration = TimeSpan.FromMilliseconds(600)
            };

            var sequences = new List<Sequiention> { seq };

            // Act
            var result = HarmonicCombiner.CombineHarmonically(sequences);

            // Assert
            Assert.That(result.Title, Is.EqualTo("Harmonic"));
            Assert.That(result.Tones, Has.Count.EqualTo(2));
            Assert.That(result.Tones[0].ObertonFrequencies[0], Is.EqualTo(180));
            Assert.That(result.Tones[1].ObertonFrequencies[0], Is.EqualTo(360));
        }

        [Test]
        public void CombineHarmonically_EmptySequences_ReturnsEmpty()
        {
            // Arrange
            var sequences = new List<Sequiention>();

            // Act
            var result = HarmonicCombiner.CombineHarmonically(sequences);

            // Assert
            Assert.That(result.Title, Is.EqualTo("Empty"));
            Assert.That(result.Tones, Has.Count.EqualTo(0));
            Assert.That(result.TotalDuration, Is.EqualTo(TimeSpan.Zero));
        }

        [Test]
        public void CombineHarmonically_NullSequences_ReturnsEmpty()
        {
            // Act
            var result = HarmonicCombiner.CombineHarmonically(null);

            // Assert
            Assert.That(result.Title, Is.EqualTo("Empty"));
            Assert.That(result.Tones, Has.Count.EqualTo(0));
            Assert.That(result.TotalDuration, Is.EqualTo(TimeSpan.Zero));
        }

        [Test]
        public void CombineHarmonically_CalculatesTotalDurationCorrectly()
        {
            // Arrange
            var seq1 = new Sequiention
            {
                Tones = new List<Tone> { new Tone(180, 300), new Tone(360, 400) },
                Title = "Track1"
            };

            var seq2 = new Sequiention
            {
                Tones = new List<Tone> { new Tone(720, 500), new Tone(900, 200) },
                Title = "Track2"
            };

            var sequences = new List<Sequiention> { seq1, seq2 };

            // Act
            var result = HarmonicCombiner.CombineHarmonically(sequences);

            // Assert - duration should be max duration at each position: 500ms + 400ms = 900ms
            Assert.That(result.TotalDuration.TotalMilliseconds, Is.EqualTo(900));
            Assert.That(result.Tones[0].Duration.TotalMilliseconds, Is.EqualTo(500)); // max(300, 500)
            Assert.That(result.Tones[1].Duration.TotalMilliseconds, Is.EqualTo(400)); // max(400, 200)
        }

        [Test]
        public void CombineHarmonically_WithExistingChords_CombinesAllFrequencies()
        {
            // Arrange - sequence that already has chords (multiple frequencies per tone)
            var seq1 = new Sequiention
            {
                Tones = new List<Tone>
                {
                    new Tone { ObertonFrequencies = new double[] { 180, 360 }, Duration = TimeSpan.FromMilliseconds(300) }
                },
                Title = "ChordTrack"
            };

            var seq2 = new Sequiention
            {
                Tones = new List<Tone>
                {
                    new Tone(720, 300)
                },
                Title = "SingleTrack"
            };

            var sequences = new List<Sequiention> { seq1, seq2 };

            // Act
            var result = HarmonicCombiner.CombineHarmonically(sequences);

            // Assert - should combine all frequencies: 180, 360, 720
            Assert.That(result.Tones, Has.Count.EqualTo(1));
            Assert.That(result.Tones[0].ObertonFrequencies, Has.Length.EqualTo(3));
            Assert.That(result.Tones[0].ObertonFrequencies, Contains.Item(180.0));
            Assert.That(result.Tones[0].ObertonFrequencies, Contains.Item(360.0));
            Assert.That(result.Tones[0].ObertonFrequencies, Contains.Item(720.0));
        }
    }
}