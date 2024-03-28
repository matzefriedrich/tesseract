namespace Tesseract.Tests
{
    using Interop;
    using NUnit.Framework;

    [TestFixture]
    public class BaseApiTests
    {
        [Test]
        public void CanGetVersion()
        {
            string version = TessApi.BaseApiGetVersion();
            Assert.That(version, Does.StartWith("5.0.0"));
        }
    }
}