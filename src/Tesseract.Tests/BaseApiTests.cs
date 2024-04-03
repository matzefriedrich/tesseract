namespace Tesseract.Tests
{
    using Interop.Abstractions;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;

    [TestFixture]
    public class BaseApiTests
    {
        [Test]
        public void CanGetVersion()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddTesseract();

            using ServiceProvider provider = services.BuildServiceProvider();
            var sut = provider.GetRequiredService<IManagedTesseractApi>();

            // Act
            string? version = sut.GetVersion();

            // Assert
            Assert.That(version, Does.StartWith("5.0.0"));
        }
    }
}