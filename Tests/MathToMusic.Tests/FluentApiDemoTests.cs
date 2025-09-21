using MathToMusic.Demo;
using NUnit.Framework;

namespace MathToMusic.Tests
{
    [TestFixture]
    public class FluentApiDemoTests
    {
        [Test]
        public void DemonstrateFluentApi_RunsWithoutException()
        {
            // This test ensures the demo runs without throwing exceptions
            // The actual file creation and Windows actions are tested elsewhere
            Assert.DoesNotThrow(() => FluentApiDemo.DemonstrateFluentApi());
        }
    }
}