namespace Tesseract.Tests.Leptonica.PixTests
{
    using Abstractions;
    using ImageProcessing;
    using Interop.Abstractions;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;

    [TestFixture]
    public class ImageRotatorTest : TesseractTestBase
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
        [TestCase(45)]
        [TestCase(80)]
        [TestCase(90)]
        [TestCase(180)]
        [TestCase(270)]
        public void Rotate_ShouldBeAbleToRotateImageByXDegrees(float angle)
        {
            // Arrange
            var pixFactory = (this.provider ?? throw new InvalidOperationException()).GetRequiredService<IPixFactory>();
            var api = this.provider.GetRequiredService<ILeptonicaApiSignatures>();
            var writer = this.provider.GetRequiredService<IPixFileWriter>();

            const string FileNameFormat = "rotation_{0}degrees.jpg";
            float angleAsRadians = MathHelper.ToRadians(angle);

            string sourcePixFilename = MakeAbsoluteTestFilePath(@"Conversion/photo_rgb_32bpp.tif");
            using Pix sourcePix = pixFactory.LoadFromFile(sourcePixFilename);

            var sut = new ImageRotator(api, pixFactory);

            // Act
            using Pix result = sut.RotateImage(sourcePix, angleAsRadians);

            // Assert
            // TODO: Visually confirm successful rotation and then setup an assertion to compare that result is the same.
            string filename = string.Format(FileNameFormat, angle);
            this.SaveResult(writer, result, ResultsDirectory, filename);
        }
    }
}