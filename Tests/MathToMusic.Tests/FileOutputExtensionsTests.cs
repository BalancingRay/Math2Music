using MathToMusic.Contracts;
using MathToMusic.Extensions;
using MathToMusic.Outputs;
using NUnit.Framework;

namespace MathToMusic.Tests
{
    [TestFixture]
    public class FileOutputExtensionsTests
    {
        private TestFileOutput _testOutput;
        private List<Sequiention> _testInput;
        private string _testResultsPath;

        [SetUp]
        public void Setup()
        {
            _testOutput = new TestFileOutput();
            _testResultsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestResults");

            // Create test input
            var tones = new List<Tone>
            {
                new Tone(440.0, 1000), // A4 for 1 second
                new Tone(554.37, 1000) // C#5 for 1 second
            };
            var sequence = new Sequiention
            {
                Tones = tones,
                TotalDuration = TimeSpan.FromSeconds(2),
                Title = "TestSequence"
            };
            _testInput = new List<Sequiention> { sequence };

            // Clean up any existing test files
            if (Directory.Exists(_testResultsPath))
            {
                foreach (var file in Directory.GetFiles(_testResultsPath, "*.tmp"))
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
                foreach (var file in Directory.GetFiles(_testResultsPath, "*.tmp"))
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
        public void OpenFileLocation_WrapperCreated_ReturnsCorrectType()
        {
            // Act
            var wrappedOutput = _testOutput.OpenFileLocation();

            // Assert
            Assert.That(wrappedOutput, Is.Not.Null);
            Assert.That(wrappedOutput, Is.InstanceOf<OpenFileLocationOutput>());
            Assert.That(wrappedOutput, Is.InstanceOf<ITonesFileOutput>());
        }

        [Test]
        public void OpenFile_WrapperCreated_ReturnsCorrectType()
        {
            // Act
            var wrappedOutput = _testOutput.OpenFile();

            // Assert
            Assert.That(wrappedOutput, Is.Not.Null);
            Assert.That(wrappedOutput, Is.InstanceOf<OpenFileOutput>());
            Assert.That(wrappedOutput, Is.InstanceOf<ITonesFileOutput>());
        }

        [Test]
        public void FluentApi_ChainedCalls_CreatesNestedWrappers()
        {
            // Act
            var chainedOutput = _testOutput.OpenFileLocation().OpenFile();

            // Assert
            Assert.That(chainedOutput, Is.Not.Null);
            Assert.That(chainedOutput, Is.InstanceOf<OpenFileOutput>());
            Assert.That(chainedOutput, Is.InstanceOf<ITonesFileOutput>());
        }

        [Test]
        public void OpenFileLocationOutput_Send_CallsInnerOutputAndReturnsFilePath()
        {
            // Arrange
            var wrappedOutput = _testOutput.OpenFileLocation();

            // Act
            wrappedOutput.Send(_testInput);

            // Assert
            Assert.That(_testOutput.WasCalled, Is.True);
            Assert.That(_testOutput.LastInput, Is.EqualTo(_testInput));
        }

        [Test]
        public void OpenFileLocationOutput_SendAndGetFilePath_ReturnsCorrectPath()
        {
            // Arrange
            var wrappedOutput = _testOutput.OpenFileLocation();

            // Act
            string? result = wrappedOutput.SendAndGetFilePath(_testInput);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(_testOutput.TestFilePath));
            Assert.That(_testOutput.WasCalled, Is.True);
        }

        [Test]
        public void OpenFileOutput_SendAndGetFilePath_ReturnsCorrectPath()
        {
            // Arrange
            var wrappedOutput = _testOutput.OpenFile();

            // Act
            string? result = wrappedOutput.SendAndGetFilePath(_testInput);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(_testOutput.TestFilePath));
            Assert.That(_testOutput.WasCalled, Is.True);
        }

        [Test]
        public void ChainedWrappers_Send_CallsInnerOutputOnce()
        {
            // Arrange
            var chainedOutput = _testOutput.OpenFileLocation().OpenFile();

            // Act
            chainedOutput.Send(_testInput);

            // Assert
            Assert.That(_testOutput.WasCalled, Is.True);
            Assert.That(_testOutput.CallCount, Is.EqualTo(1));
        }

        [Test]
        public void OpenFileLocationOutput_NullInput_ReturnsNull()
        {
            // Arrange
            var wrappedOutput = _testOutput.OpenFileLocation();

            // Act
            string? result = wrappedOutput.SendAndGetFilePath(null);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void OpenFileOutput_EmptyInput_ReturnsNull()
        {
            // Arrange
            var wrappedOutput = _testOutput.OpenFile();
            var emptyInput = new List<Sequiention>();

            // Act
            string? result = wrappedOutput.SendAndGetFilePath(emptyInput);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void WavFileOutput_ImplementsITonesFileOutput()
        {
            // Arrange
            var wavOutput = new WavFileOutput();

            // Assert
            Assert.That(wavOutput, Is.InstanceOf<ITonesFileOutput>());
            Assert.That(wavOutput, Is.InstanceOf<ITonesOutput>());
        }

        [Test]
        public void WavFileOutput_SendAndGetFilePath_ReturnsFilePath()
        {
            // Arrange
            var wavOutput = new WavFileOutput();

            // Act
            string? result = wavOutput.SendAndGetFilePath(_testInput);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Does.EndWith(".wav"));
            Assert.That(File.Exists(result), Is.True);

            // Clean up
            if (!string.IsNullOrEmpty(result) && File.Exists(result))
            {
                try { File.Delete(result); } catch { /* ignore */ }
            }
        }
    }

    /// <summary>
    /// Test implementation of ITonesFileOutput for testing purposes
    /// </summary>
    internal class TestFileOutput : ITonesFileOutput
    {
        public string TestFilePath { get; private set; }
        public bool WasCalled { get; private set; }
        public IList<Sequiention>? LastInput { get; private set; }
        public int CallCount { get; private set; }

        public TestFileOutput()
        {
            // Create a test file path
            string testResultsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestResults");
            Directory.CreateDirectory(testResultsPath);
            TestFilePath = Path.Combine(testResultsPath, $"test_{DateTime.Now:yyyyMMdd_HHmmss_fff}.tmp");
        }

        public void Send(IList<Sequiention> input)
        {
            SendAndGetFilePath(input);
        }

        public string? SendAndGetFilePath(IList<Sequiention> input)
        {
            WasCalled = true;
            LastInput = input;
            CallCount++;

            if (input == null || input.Count == 0)
                return null;

            // Create a dummy file for testing
            File.WriteAllText(TestFilePath, "test content");
            return TestFilePath;
        }
    }
}