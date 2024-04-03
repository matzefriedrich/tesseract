namespace Tesseract.Tests.Leptonica.PixTests
{
    using Abstractions;
    using ImageProcessing;
    using Interop.Abstractions;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;

    [TestFixture]
    public class GrayscaleConverterTest : TesseractTestBase
    {
        [SetUp]
        public void Init()
        {
            this.services.AddTesseract();
            this.provider = this.services.BuildServiceProvider();
        }

        [TearDown]
        public void Teardown()
        {
            this.provider?.Dispose();
        }

        private const string ResultsDirectory = @"Results/ImageManipulation/";
        private readonly ServiceCollection services = new();
        private ServiceProvider? provider;

        [Test]
        public void ConvertRGBToGrayTest()
        {
            // Arrange
            var pixFactory = (this.provider ?? throw new InvalidOperationException()).GetRequiredService<IPixFactory>();
            var api = this.provider.GetRequiredService<ILeptonicaApiSignatures>();
            var writer = this.provider.GetRequiredService<IPixFileWriter>();

            string sourcePixFilename = MakeAbsoluteTestFilePath(@"Conversion/photo_rgb_32bpp.tif");
            using Pix sourcePix = pixFactory.LoadFromFile(sourcePixFilename);

            var sut = new GrayscaleConverter(api);

            // Act
            using Pix grayscaleImage = sut.ConvertRgbToGray(sourcePix);

            // Assert
            Assert.That(grayscaleImage.Depth, Is.EqualTo(8));

            this.SaveResult(writer, grayscaleImage, ResultsDirectory, "grayscaleImage.jpg");
        }
    }
}