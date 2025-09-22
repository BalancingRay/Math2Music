using MathToMusic.Contracts;
using MathToMusic.Models;
using MathToMusic.Processors;
using NUnit.Framework;

namespace MathToMusic.Tests
{
    [TestFixture]
    public class TimberFunctionalityTests
    {
        [Test]
        public void TimberProfiles_ContainsPredefinedProfiles()
        {
            // Verify that some key profiles exist
            Assert.That(TimberProfiles.HasProfile("Piano"), Is.True);
            Assert.That(TimberProfiles.HasProfile("Guitar"), Is.True);
            Assert.That(TimberProfiles.HasProfile("Sine"), Is.True);
            Assert.That(TimberProfiles.HasProfile("Sawtooth"), Is.True);
            Assert.That(TimberProfiles.HasProfile("NonexistentProfile"), Is.False);
        }

        [Test]
        public void TimberProfiles_GetProfile_ReturnsCorrectCoefficients()
        {
            var sineProfile = TimberProfiles.GetProfile("Sine");
            Assert.That(sineProfile, Is.Not.Null);
            Assert.That(sineProfile.Length, Is.EqualTo(1));
            Assert.That(sineProfile[0], Is.EqualTo(1.0f));

            var pianoProfile = TimberProfiles.GetProfile("Piano");
            Assert.That(pianoProfile, Is.Not.Null);
            Assert.That(pianoProfile.Length, Is.GreaterThan(1));
            Assert.That(pianoProfile[0], Is.EqualTo(1.0f)); // Fundamental should be 1.0
        }

        [Test]
        public void TimberSequenceProcessor_ApplyTimber_CreatesOvertones()
        {
            // Create a simple sequence with one tone
            var sequence = new Sequiention
            {
                TotalDuration = TimeSpan.FromSeconds(1),
                Title = "Test",
                Tones = new List<Tone>
                {
                    new Tone(440.0, 1000) // A4 for 1 second
                }
            };

            // Apply Piano timber
            var processor = new TimberSequenceProcessor(TimberProfiles.GetProfile("Piano"));
            var processedSequence = processor.Process(sequence);

            Assert.That(processedSequence.Timber, Is.Not.Null);
            Assert.That(processedSequence.Tones.Count, Is.EqualTo(1));

            var processedTone = processedSequence.Tones[0];
            Assert.That(processedTone.ObertonFrequencies.Length, Is.GreaterThan(1));

            // Check that overtones are harmonics of the fundamental
            Assert.That(processedTone.ObertonFrequencies[0], Is.EqualTo(440.0)); // Fundamental
            Assert.That(processedTone.ObertonFrequencies[1], Is.EqualTo(880.0)); // 2nd harmonic
            Assert.That(processedTone.ObertonFrequencies[2], Is.EqualTo(1320.0)); // 3rd harmonic
        }

        [Test]
        public void TimberSequenceProcessor_ApplyTimber_PreservesSequenceProperties()
        {
            var originalSequence = new Sequiention
            {
                TotalDuration = TimeSpan.FromSeconds(2),
                Title = "Original",
                Tones = new List<Tone>
                {
                    new Tone(220.0, 500),
                    new Tone(330.0, 1500)
                }
            };

            var processor = new TimberSequenceProcessor(TimberProfiles.GetProfile("Guitar"));
            var processedSequence = processor.Process(originalSequence);

            Assert.That(processedSequence.TotalDuration, Is.EqualTo(originalSequence.TotalDuration));
            Assert.That(processedSequence.Title, Is.EqualTo(originalSequence.Title));
            Assert.That(processedSequence.Tones.Count, Is.EqualTo(originalSequence.Tones.Count));

            // Check that durations are preserved
            for (int i = 0; i < processedSequence.Tones.Count; i++)
            {
                Assert.That(processedSequence.Tones[i].Duration, Is.EqualTo(originalSequence.Tones[i].Duration));
            }
        }

        [Test]
        public void TimberSequenceProcessor_HandlesSilenceTones()
        {
            var sequence = new Sequiention
            {
                TotalDuration = TimeSpan.FromSeconds(1),
                Title = "Test with Silence",
                Tones = new List<Tone>
                {
                    new Tone(0.0, 500), // Silence
                    new Tone(440.0, 500)  // A4
                }
            };

            var processor = new TimberSequenceProcessor(TimberProfiles.GetProfile("Piano"));
            var processedSequence = processor.Process(sequence);

            Assert.That(processedSequence.Tones.Count, Is.EqualTo(2));

            // Silence tone should remain silence
            var silentTone = processedSequence.Tones[0];
            Assert.That(silentTone.ObertonFrequencies[0], Is.EqualTo(0.0));

            // Regular tone should have overtones
            var audibleTone = processedSequence.Tones[1];
            Assert.That(audibleTone.ObertonFrequencies.Length, Is.GreaterThan(1));
        }

        [Test]
        public void TimberSequenceProcessor_MultipleSequences()
        {
            var sequences = new List<Sequiention>
            {
                new Sequiention
                {
                    TotalDuration = TimeSpan.FromSeconds(1),
                    Title = "Seq1",
                    Tones = new List<Tone> { new Tone(440.0, 1000) }
                },
                new Sequiention
                {
                    TotalDuration = TimeSpan.FromSeconds(1),
                    Title = "Seq2",
                    Tones = new List<Tone> { new Tone(880.0, 1000) }
                }
            };

            var processor = new TimberSequenceProcessor(TimberProfiles.GetProfile("Violin"));
            var processedSequences = processor.Process(sequences);
            Assert.That(processedSequences.Count, Is.EqualTo(2));
            foreach (var seq in processedSequences)
            {
                Assert.That(seq.Timber, Is.Not.Null);
                Assert.That(seq.Tones[0].ObertonFrequencies.Length, Is.GreaterThan(1));
            }
        }

        [Test]
        public void ISequenceProcessor_Interface_Works()
        {
            ISequenceProcessor processor = new TimberSequenceProcessor(TimberProfiles.GetProfile("Organ"));

            var sequence = new Sequiention
            {
                TotalDuration = TimeSpan.FromSeconds(1),
                Title = "Interface Test",
                Tones = new List<Tone> { new Tone(440.0, 1000) }
            };

            var processedSequence = processor.Process(sequence);

            Assert.That(processedSequence.Tones[0].ObertonFrequencies.Length, Is.GreaterThan(1));
        }

        [Test]
        public void GetAvailableProfiles_ReturnsExpectedProfiles()
        {
            var profiles = TimberProfiles.GetAvailableProfiles().ToList();

            Assert.That(profiles.Count, Is.GreaterThan(10));
            Assert.That(profiles, Contains.Item("Piano"));
            Assert.That(profiles, Contains.Item("Guitar"));
            Assert.That(profiles, Contains.Item("Sine"));
            Assert.That(profiles, Contains.Item("Sawtooth"));
            Assert.That(profiles, Contains.Item("Square"));
        }
    }
}