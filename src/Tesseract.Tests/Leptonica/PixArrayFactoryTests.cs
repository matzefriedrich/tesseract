namespace Tesseract.Tests.Leptonica.PixTests
{
    using Microsoft.Extensions.DependencyInjection;

    using NUnit.Framework;

    [TestFixture]
    public class PixArrayFactoryTests : TesseractTestBase
    {
        [Test]
        public void PixArrayFactory_Create_returns_empty_PixArray()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddTesseract();

            using ServiceProvider provider = services.BuildServiceProvider();
            var factory = provider.GetRequiredService<IPixArrayFactory>();

            // Act
            using PixArray pixA = factory.Create(0);

            // Assert
            Assert.That(pixA.Count, Is.EqualTo(0));
        }
    }
}