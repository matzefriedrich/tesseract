namespace Tesseract.Tests.Leptonica.PixTests
{
    using Abstractions;
    using ImageProcessing;
    using Interop.Abstractions;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;

    [TestFixture]
    public class ImageBinarizerTest : TesseractTestBase
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

        private readonly ServiceCollection services = new();
        private ServiceProvider? provider;
        private const string ResultsDirectory = @"Results/ImageManipulation/";

        [Test]
        public void OtsuBinarizationTest()
        {
            // Arrange
            var pixFactory = (this.provider ?? throw new InvalidOperationException()).GetRequiredService<IPixFactory>();
            var api = this.provider.GetRequiredService<ILeptonicaApiSignatures>();
            var writer = this.provider.GetRequiredService<IPixFileWriter>();

            string sourcePixFilename = MakeAbsoluteTestFilePath(@"Binarization/neo-8bit.png");
            using Pix sourcePix = pixFactory.LoadFromFile(sourcePixFilename);

            var sut = new ImageBinarizer(api);

            // Act
            using Pix binarizedImage = sut.BinarizeOtsuAdaptiveThreshold(sourcePix, 200, 200, 10, 10, 0.1F);

            // Assert
            Assert.That(binarizedImage, Is.Not.Null);
            Assert.That(binarizedImage.Handle, Is.Not.EqualTo(IntPtr.Zero));

            this.SaveResult(writer, binarizedImage, ResultsDirectory, "binarizedOtsuImage.png");
        }

        [Test]
        public void SauvolaBinarizationTest()
        {
            // Arrange
            var pixFactory = (this.provider ?? throw new InvalidOperationException()).GetRequiredService<IPixFactory>();
            var api = this.provider.GetRequiredService<ILeptonicaApiSignatures>();
            var writer = this.provider.GetRequiredService<IPixFileWriter>();

            string sourcePixFilename = MakeAbsoluteTestFilePath(@"Binarization/neo-8bit-grayscale.png");
            using Pix sourcePix = pixFactory.LoadFromFile(sourcePixFilename);

            var grayscale = new GrayscaleConverter(api);
            using Pix grayscalePix = grayscale.ConvertRgbToGray(sourcePix, 1, 1, 1);

            var sut = new ImageBinarizer(api);

            // Act
            using Pix binarizedImage = sut.BinarizeSauvola(grayscalePix, 10, 0.35f, false);

            // Assert
            Assert.That(binarizedImage, Is.Not.Null);
            Assert.That(binarizedImage.Handle, Is.Not.EqualTo(IntPtr.Zero));

            this.SaveResult(writer, binarizedImage, ResultsDirectory, "binarizedSauvolaImage.png");
        }

        [Test]
        public void SauvolaTiledBinarizationTest()
        {
            // Arrange
            var pixFactory = (this.provider ?? throw new InvalidOperationException()).GetRequiredService<IPixFactory>();
            var api = this.provider.GetRequiredService<ILeptonicaApiSignatures>();
            var writer = this.provider.GetRequiredService<IPixFileWriter>();

            string sourcePixFilename = MakeAbsoluteTestFilePath(@"Binarization/neo-8bit-grayscale.png");
            using Pix sourcePix = pixFactory.LoadFromFile(sourcePixFilename);

            var grayscale = new GrayscaleConverter(api);
            using Pix grayscalePix = grayscale.ConvertRgbToGray(sourcePix, 1, 1, 1);

            var sut = new ImageBinarizer(api);

            // Act
            using Pix result = sut.BinarizeSauvolaTiled(grayscalePix, 10, 0.35f, 2, 2);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Handle, Is.Not.EqualTo(IntPtr.Zero));

            this.SaveResult(writer, result, ResultsDirectory, "binarizedSauvolaTiledImage.png");
        }
    }
}