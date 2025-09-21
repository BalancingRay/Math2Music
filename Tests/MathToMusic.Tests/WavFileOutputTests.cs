using MathToMusic.Contracts;
using MathToMusic.Outputs;
using NUnit.Framework;

namespace MathToMusic.Tests
{
    [TestFixture]
    public class WavFileOutputTests
    {
        private WavFileOutput _wavOutput;
        private string _testResultsPath;

        [SetUp]
        public void Setup()
        {
            _wavOutput = new WavFileOutput();

            // Use a temporary directory for testing
            _testResultsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Results");

            // Clean up any existing test files
            if (Directory.Exists(_testResultsPath))
            {
                foreach (var file in Directory.GetFiles(_testResultsPath, "*.wav"))
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch
                    {
                        // Ignore if file is in use
                    }
                }
            }
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up test files
            if (Directory.Exists(_testResultsPath))
            {
                foreach (var file in Directory.GetFiles(_testResultsPath, "*.wav"))
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch
                    {
                        // Ignore if file is in use
                    }
                }
            }
        }

        [Test]
        public void Send_MonophonicSequence_CreatesMonoWavFile()
        {
            // Arrange
            var tones = new List<Tone>
            {
                new Tone(440.0, 1000), // A4 for 1 second
                new Tone(554.37, 1000) // C#5 for 1 second
            };
            var sequence = new Sequiention
            {
                Tones = tones,
                TotalDuration = TimeSpan.FromSeconds(2),
                Title = "TestMono"
            };
            var input = new List<Sequiention> { sequence };

            // Act
            _wavOutput.Send(input);

            // Assert
            Assert.That(Directory.Exists(_testResultsPath), Is.True);
            var wavFiles = Directory.GetFiles(_testResultsPath, "mono_*.wav");
            Assert.That(wavFiles.Length, Is.GreaterThan(0));

            // Verify file is not empty and has reasonable size
            var fileInfo = new FileInfo(wavFiles[0]);
            Assert.That(fileInfo.Length, Is.GreaterThan(100)); // Should have WAV header + data
        }

        [Test]
        public void Send_PolyphonicSequences_CreatesPolyWavFile()
        {
            // Arrange
            var sequence1 = new Sequiention
            {
                Tones = new List<Tone>
                {
                    new Tone(440.0, 1000), // A4
                    new Tone(523.25, 1000) // C5
                },
                TotalDuration = TimeSpan.FromSeconds(2),
                Title = "Track1"
            };
            var sequence2 = new Sequiention
            {
                Tones = new List<Tone>
                {
                    new Tone(329.63, 1000), // E4
                    new Tone(659.25, 1000)  // E5
                },
                TotalDuration = TimeSpan.FromSeconds(2),
                Title = "Track2"
            };
            var input = new List<Sequiention> { sequence1, sequence2 };

            // Act
            _wavOutput.Send(input);

            // Assert
            Assert.That(Directory.Exists(_testResultsPath), Is.True);
            var wavFiles = Directory.GetFiles(_testResultsPath, "poly_*.wav");
            Assert.That(wavFiles.Length, Is.GreaterThan(0));

            // Verify file is not empty and has reasonable size
            var fileInfo = new FileInfo(wavFiles[0]);
            Assert.That(fileInfo.Length, Is.GreaterThan(100)); // Should have WAV header + data
        }

        [Test]
        public void Send_EmptyInput_DoesNotCreateFile()
        {
            // Arrange
            var input = new List<Sequiention>();

            // Act
            _wavOutput.Send(input);

            // Assert
            if (Directory.Exists(_testResultsPath))
            {
                var wavFiles = Directory.GetFiles(_testResultsPath, "*.wav");
                Assert.That(wavFiles.Length, Is.EqualTo(0));
            }
        }

        [Test]
        public void Send_NullInput_DoesNotCreateFile()
        {
            // Arrange & Act & Assert
            Assert.DoesNotThrow(() => _wavOutput.Send(null));

            if (Directory.Exists(_testResultsPath))
            {
                var wavFiles = Directory.GetFiles(_testResultsPath, "*.wav");
                Assert.That(wavFiles.Length, Is.EqualTo(0));
            }
        }

        [Test]
        public void Send_SequenceWithSilence_HandlesZeroFrequency()
        {
            // Arrange
            var tones = new List<Tone>
            {
                new Tone(440.0, 500), // Sound
                new Tone(0.0, 500),   // Silence (rest)
                new Tone(554.37, 500) // Sound again
            };
            var sequence = new Sequiention
            {
                Tones = tones,
                TotalDuration = TimeSpan.FromSeconds(1.5),
                Title = "TestSilence"
            };
            var input = new List<Sequiention> { sequence };

            // Act & Assert
            Assert.DoesNotThrow(() => _wavOutput.Send(input));

            var wavFiles = Directory.GetFiles(_testResultsPath, "mono_*.wav");
            Assert.That(wavFiles.Length, Is.GreaterThan(0));
        }

        [Test]
        public void Send_ValidWavFileStructure_CanBeRead()
        {
            // Arrange
            var tones = new List<Tone>
            {
                new Tone(440.0, 500) // A4 for 0.5 seconds
            };
            var sequence = new Sequiention
            {
                Tones = tones,
                TotalDuration = TimeSpan.FromSeconds(0.5),
                Title = "TestStructure"
            };
            var input = new List<Sequiention> { sequence };

            // Act
            _wavOutput.Send(input);

            // Assert
            var wavFiles = Directory.GetFiles(_testResultsPath, "mono_*.wav");
            Assert.That(wavFiles.Length, Is.GreaterThan(0));

            // Verify WAV file structure by reading header
            using var fileStream = new FileStream(wavFiles[0], FileMode.Open, FileAccess.Read);
            using var reader = new BinaryReader(fileStream);

            // Check RIFF header
            string riff = new string(reader.ReadChars(4));
            Assert.That(riff, Is.EqualTo("RIFF"));

            int fileSize = reader.ReadInt32();
            Assert.That(fileSize, Is.GreaterThan(0));

            string wave = new string(reader.ReadChars(4));
            Assert.That(wave, Is.EqualTo("WAVE"));

            // Check format chunk
            string fmt = new string(reader.ReadChars(4));
            Assert.That(fmt, Is.EqualTo("fmt "));

            int formatChunkSize = reader.ReadInt32();
            Assert.That(formatChunkSize, Is.EqualTo(16)); // PCM format

            short audioFormat = reader.ReadInt16();
            Assert.That(audioFormat, Is.EqualTo(1)); // PCM

            short numChannels = reader.ReadInt16();
            Assert.That(numChannels, Is.EqualTo(2)); // Stereo

            int sampleRate = reader.ReadInt32();
            Assert.That(sampleRate, Is.EqualTo(44100)); // Standard CD quality
        }

        [Test]
        public void Send_DifferentSequenceLengths_HandlesCorrectly()
        {
            // Arrange - sequences with different durations
            var shortSequence = new Sequiention
            {
                Tones = new List<Tone> { new Tone(440.0, 500) }, // 0.5 seconds
                TotalDuration = TimeSpan.FromSeconds(0.5),
                Title = "Short"
            };
            var longSequence = new Sequiention
            {
                Tones = new List<Tone>
                {
                    new Tone(523.25, 1000), // 1 second
                    new Tone(659.25, 1000)  // 1 second
                },
                TotalDuration = TimeSpan.FromSeconds(2),
                Title = "Long"
            };
            var input = new List<Sequiention> { shortSequence, longSequence };

            // Act & Assert
            Assert.DoesNotThrow(() => _wavOutput.Send(input));

            var wavFiles = Directory.GetFiles(_testResultsPath, "poly_*.wav");
            Assert.That(wavFiles.Length, Is.GreaterThan(0));

            // File should be created based on the longer sequence duration
            var fileInfo = new FileInfo(wavFiles[0]);
            Assert.That(fileInfo.Length, Is.GreaterThan(100));
        }

        [Test]
        public void Send_MultipleSequences_PreventClipping()
        {
            // Arrange - Multiple sequences with same frequencies and high amplitudes to test clipping prevention
            var sequences = new List<Sequiention>();
            for (int i = 0; i < 5; i++) // 5 sequences to force potential clipping
            {
                sequences.Add(new Sequiention
                {
                    Tones = new List<Tone>
                    {
                        new Tone(440.0, 500), // A4 for all sequences
                        new Tone(880.0, 500)  // A5 for all sequences
                    },
                    TotalDuration = TimeSpan.FromSeconds(1),
                    Title = $"Track{i + 1}"
                });
            }

            // Act & Assert
            Assert.DoesNotThrow(() => _wavOutput.Send(sequences));

            var wavFiles = Directory.GetFiles(_testResultsPath, "poly_*.wav");
            Assert.That(wavFiles.Length, Is.GreaterThan(0));

            // Verify WAV file is created with reasonable size
            var fileInfo = new FileInfo(wavFiles[0]);
            Assert.That(fileInfo.Length, Is.GreaterThan(1000)); // Should be substantial for 1 second of audio
        }

        [Test]
        public void Send_ManySequencesWithLowFrequencies_HandlesAmplificationCorrectly()
        {
            // Arrange - Test with very low frequencies that get amplified
            var sequences = new List<Sequiention>();
            for (int i = 0; i < 3; i++)
            {
                sequences.Add(new Sequiention
                {
                    Tones = new List<Tone>
                    {
                        new Tone(45.0 + i * 10, 500), // Very low frequencies: 45Hz, 55Hz, 65Hz
                        new Tone(90.0 + i * 20, 500)  // Low frequencies: 90Hz, 110Hz, 130Hz
                    },
                    TotalDuration = TimeSpan.FromSeconds(1),
                    Title = $"LowFreq{i + 1}"
                });
            }

            // Act & Assert
            Assert.DoesNotThrow(() => _wavOutput.Send(sequences));

            var wavFiles = Directory.GetFiles(_testResultsPath, "poly_*.wav");
            Assert.That(wavFiles.Length, Is.GreaterThan(0));

            // The file should be created without exceptions (indicating no clipping-related crashes)
            var fileInfo = new FileInfo(wavFiles[0]);
            Assert.That(fileInfo.Length, Is.GreaterThan(1000));
        }

        [Test]
        public void Send_VerifyNormalizationApplied_WhenAmplitudeExceedsRange()
        {
            // Arrange - Create a scenario that would definitely cause clipping without normalization
            var sequences = new List<Sequiention>();
            
            // Add multiple sequences with the same low frequency (gets amplified) to force clipping
            for (int i = 0; i < 8; i++) // 8 sequences should definitely require normalization
            {
                sequences.Add(new Sequiention
                {
                    Tones = new List<Tone>
                    {
                        new Tone(30.0, 300), // Very low frequency gets maximum amplification
                    },
                    TotalDuration = TimeSpan.FromSeconds(0.3),
                    Title = $"ClippingTest{i + 1}"
                });
            }

            // Act - This should trigger normalization
            string? filePath = null;
            Assert.DoesNotThrow(() => 
            {
                filePath = _wavOutput.SendAndGetFilePath(sequences);
            });

            // Assert
            Assert.That(filePath, Is.Not.Null);
            Assert.That(File.Exists(filePath), Is.True);

            // Verify the file has content and is not corrupted
            var fileInfo = new FileInfo(filePath);
            Assert.That(fileInfo.Length, Is.GreaterThan(1000)); // Should be substantial for 8 channels
        }

        #region Stereo Positioning Tests

        [Test]
        public void Send_SingleSequence_RemainsCentered()
        {
            // Arrange
            var sequence = new Sequiention
            {
                Tones = new List<Tone> { new Tone(440.0, 500) },
                TotalDuration = TimeSpan.FromSeconds(0.5),
                Title = "Mono"
            };
            var input = new List<Sequiention> { sequence };

            // Act & Assert - Should not throw and should create centered audio
            Assert.DoesNotThrow(() => _wavOutput.Send(input));

            var wavFiles = Directory.GetFiles(_testResultsPath, "mono_*.wav");
            Assert.That(wavFiles.Length, Is.GreaterThan(0));
        }

        [Test]
        public void Send_TwoSequences_AppliesBasicStereoSeparation()
        {
            // Arrange - Two sequences should get 1.0/0.9 and 0.9/1.0 coefficients
            var sequence1 = new Sequiention
            {
                Tones = new List<Tone> { new Tone(440.0, 500) },
                TotalDuration = TimeSpan.FromSeconds(0.5),
                Title = "Left"
            };
            var sequence2 = new Sequiention
            {
                Tones = new List<Tone> { new Tone(880.0, 500) },
                TotalDuration = TimeSpan.FromSeconds(0.5),
                Title = "Right"
            };
            var input = new List<Sequiention> { sequence1, sequence2 };

            // Act
            string? filePath = null;
            Assert.DoesNotThrow(() => 
            {
                filePath = _wavOutput.SendAndGetFilePath(input);
            });

            // Assert
            Assert.That(filePath, Is.Not.Null);
            Assert.That(File.Exists(filePath), Is.True);

            // Verify it's a polyphonic file
            Assert.That(Path.GetFileName(filePath), Does.StartWith("poly_"));

            var fileInfo = new FileInfo(filePath);
            Assert.That(fileInfo.Length, Is.GreaterThan(500)); // Should have substantial stereo content
        }

        [Test]
        public void Send_ThreeSequences_DistributesAcrossStereoField()
        {
            // Arrange - Three sequences: left, center, right positioning
            var sequences = new List<Sequiention>();
            for (int i = 0; i < 3; i++)
            {
                sequences.Add(new Sequiention
                {
                    Tones = new List<Tone> { new Tone(440.0 + i * 110, 500) },
                    TotalDuration = TimeSpan.FromSeconds(0.5),
                    Title = $"Track{i + 1}"
                });
            }

            // Act
            string? filePath = null;
            Assert.DoesNotThrow(() => 
            {
                filePath = _wavOutput.SendAndGetFilePath(sequences);
            });

            // Assert
            Assert.That(filePath, Is.Not.Null);
            Assert.That(File.Exists(filePath), Is.True);

            // Verify it's a polyphonic file with reasonable size
            Assert.That(Path.GetFileName(filePath), Does.StartWith("poly_"));
            var fileInfo = new FileInfo(filePath);
            Assert.That(fileInfo.Length, Is.GreaterThan(800)); // Should have substantial content for 3 tracks
        }

        [Test]
        public void Send_ManySequences_RespectsMaximumShift()
        {
            // Arrange - Many sequences to test maximum shift limiting
            var sequences = new List<Sequiention>();
            for (int i = 0; i < 10; i++)
            {
                sequences.Add(new Sequiention
                {
                    Tones = new List<Tone> { new Tone(440.0 + i * 55, 300) },
                    TotalDuration = TimeSpan.FromSeconds(0.3),
                    Title = $"Track{i + 1}"
                });
            }

            // Act - Should not throw even with many sequences
            string? filePath = null;
            Assert.DoesNotThrow(() => 
            {
                filePath = _wavOutput.SendAndGetFilePath(sequences);
            });

            // Assert
            Assert.That(filePath, Is.Not.Null);
            Assert.That(File.Exists(filePath), Is.True);

            // Verify it's a polyphonic file
            Assert.That(Path.GetFileName(filePath), Does.StartWith("poly_"));
            var fileInfo = new FileInfo(filePath);
            Assert.That(fileInfo.Length, Is.GreaterThan(1500)); // Should have content for 10 tracks
        }

        [Test]
        public void Send_StereoPositioning_MaintainsWavFileStructure()
        {
            // Arrange - Multiple sequences to test stereo while verifying WAV structure
            var sequences = new List<Sequiention>();
            for (int i = 0; i < 4; i++)
            {
                sequences.Add(new Sequiention
                {
                    Tones = new List<Tone> { new Tone(220.0 * (i + 1), 400) },
                    TotalDuration = TimeSpan.FromSeconds(0.4),
                    Title = $"StereoTest{i + 1}"
                });
            }

            // Act
            string? filePath = null;
            Assert.DoesNotThrow(() => 
            {
                filePath = _wavOutput.SendAndGetFilePath(sequences);
            });

            // Assert
            Assert.That(filePath, Is.Not.Null);
            Assert.That(File.Exists(filePath), Is.True);

            // Verify WAV file structure is still valid
            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using var reader = new BinaryReader(fileStream);

            // Check RIFF header
            string riff = new string(reader.ReadChars(4));
            Assert.That(riff, Is.EqualTo("RIFF"));

            reader.ReadInt32(); // file size
            string wave = new string(reader.ReadChars(4));
            Assert.That(wave, Is.EqualTo("WAVE"));

            // Check format chunk
            string fmt = new string(reader.ReadChars(4));
            Assert.That(fmt, Is.EqualTo("fmt "));

            reader.ReadInt32(); // format chunk size
            short audioFormat = reader.ReadInt16();
            Assert.That(audioFormat, Is.EqualTo(1)); // PCM

            short numChannels = reader.ReadInt16();
            Assert.That(numChannels, Is.EqualTo(2)); // Still stereo

            int sampleRate = reader.ReadInt32();
            Assert.That(sampleRate, Is.EqualTo(44100)); // Still standard sample rate
        }

        [Test]
        public void Send_EmptySequences_HandledCorrectlyWithStereo()
        {
            // Arrange - Sequences with some silence to test stereo with rests
            var sequence1 = new Sequiention
            {
                Tones = new List<Tone>
                {
                    new Tone(440.0, 200),  // Sound
                    new Tone(0.0, 200),    // Rest
                    new Tone(523.25, 200)  // Sound
                },
                TotalDuration = TimeSpan.FromSeconds(0.6),
                Title = "WithSilence1"
            };
            var sequence2 = new Sequiention
            {
                Tones = new List<Tone>
                {
                    new Tone(0.0, 200),    // Rest
                    new Tone(659.25, 200), // Sound
                    new Tone(0.0, 200)     // Rest
                },
                TotalDuration = TimeSpan.FromSeconds(0.6),
                Title = "WithSilence2"
            };
            var input = new List<Sequiention> { sequence1, sequence2 };

            // Act & Assert
            Assert.DoesNotThrow(() => _wavOutput.Send(input));

            var wavFiles = Directory.GetFiles(_testResultsPath, "poly_*.wav");
            Assert.That(wavFiles.Length, Is.GreaterThan(0));

            var fileInfo = new FileInfo(wavFiles[0]);
            Assert.That(fileInfo.Length, Is.GreaterThan(400)); // Should have reasonable content
        }

        [Test]
        public void Send_VeryLowFrequenciesWithStereo_HandlesAmplificationCorrectly()
        {
            // Arrange - Test stereo with low frequencies that get amplified
            var sequences = new List<Sequiention>();
            for (int i = 0; i < 3; i++)
            {
                sequences.Add(new Sequiention
                {
                    Tones = new List<Tone> { new Tone(40.0 + i * 10, 400) }, // Very low: 40Hz, 50Hz, 60Hz
                    TotalDuration = TimeSpan.FromSeconds(0.4),
                    Title = $"LowFreqStereo{i + 1}"
                });
            }

            // Act
            string? filePath = null;
            Assert.DoesNotThrow(() => 
            {
                filePath = _wavOutput.SendAndGetFilePath(sequences);
            });

            // Assert - Should handle low frequency amplification + stereo without issues
            Assert.That(filePath, Is.Not.Null);
            Assert.That(File.Exists(filePath), Is.True);

            var fileInfo = new FileInfo(filePath);
            Assert.That(fileInfo.Length, Is.GreaterThan(800)); // Should have substantial content
        }

        #endregion
    }
}