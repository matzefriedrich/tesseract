namespace Tesseract.Tests.Leptonica.PixTests
{
    using Abstractions;

    using Microsoft.Extensions.DependencyInjection;

    using NUnit.Framework;

    [TestFixture]
    public sealed class PixArrayTests : TesseractTestBase
    {
        [Test]
        public void PixArray_Add_adds_Pix_to_array()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddTesseract();

            using ServiceProvider provider = services.BuildServiceProvider();
            var pixArrayFactory = provider.GetRequiredService<IPixArrayFactory>();
            var pixFactory = provider.GetRequiredService<IPixFactory>();

            string sourcePixPath = MakeAbsoluteTestFilePath("Ocr/phototest.tif");
            using PixArray pixA = pixArrayFactory.Create(0);
            using Pix sourcePix = pixFactory.LoadFromFile(sourcePixPath);

            // Act
            pixA.Add(sourcePix);

            // Assert
            Assert.That(pixA.Count, Is.EqualTo(1));

            using Pix targetPix = pixA.GetPix(0);
            Assert.That(targetPix, Is.EqualTo(sourcePix));
        }

        [Test]
        public void PixArray_Remove_can_remove_Pix_from_array()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddTesseract();

            using ServiceProvider provider = services.BuildServiceProvider();
            var pixArrayFactory = provider.GetRequiredService<IPixArrayFactory>();
            var pixFactory = provider.GetRequiredService<IPixFactory>();

            string sourcePixPath = MakeAbsoluteTestFilePath("Ocr/phototest.tif");
            using PixArray pixA = pixArrayFactory.Create(0);
            using (Pix sourcePix = pixFactory.LoadFromFile(sourcePixPath))
            {
                pixA.Add(sourcePix);
            }

            // Act
            pixA.Remove(0);

            // Assert
            Assert.That(pixA.Count, Is.EqualTo(0));
        }

        [Test]
        public void PixArray_Clean_remove_all_pix_from_array()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddTesseract();

            using ServiceProvider provider = services.BuildServiceProvider();
            var pixArrayFactory = provider.GetRequiredService<IPixArrayFactory>();
            var pixFactory = provider.GetRequiredService<IPixFactory>();

            string sourcePixPath = MakeAbsoluteTestFilePath("Ocr/phototest.tif");
            using PixArray pixA = pixArrayFactory.Create(0);
            using (Pix sourcePix = pixFactory.LoadFromFile(sourcePixPath))
            {
                pixA.Add(sourcePix);
            }

            // Act
            pixA.Clear();

            // Assert
            Assert.That(pixA.Count, Is.EqualTo(0));
        }
    }
}