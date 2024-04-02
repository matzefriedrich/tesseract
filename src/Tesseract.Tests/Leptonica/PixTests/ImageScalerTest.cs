namespace Tesseract.Tests.Leptonica.PixTests
{
    using Abstractions;
    using ImageProcessing;
    using Interop.Abstractions;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;

    [TestFixture]
    public class ImageScalerTest : TesseractTestBase
    {
        private const string ResultsDirectory = @"Results/ImageManipulation/";

        private readonly ServiceCollection services = new();
        private ServiceProvider? provider;

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

        [Test]
        public void Scale_RGB_ShouldBeScaledBySpecifiedFactor([Values(0.25f, 0.5f, 0.75f, 1, 1.25f, 1.5f, 1.75f, 2, 4, 8)] float scale)
        {
            // Arrange
            var pixFactory = (this.provider ?? throw new InvalidOperationException()).GetRequiredService<IPixFactory>();
            var api = this.provider.GetRequiredService<ILeptonicaApiSignatures>();
            var writer = this.provider.GetRequiredService<IPixFileWriter>();

            const string FileNameFormat = "scale_{0}.jpg";

            string sourcePixFilename = MakeAbsoluteTestFilePath(@"Conversion/photo_rgb_32bpp.tif");
            using Pix sourcePix = pixFactory.LoadFromFile(sourcePixFilename);

            var sut = new ImageScaler(api);

            // Act
            using Pix result = sut.ScaleImage(sourcePix, scale, scale);

            // Assert
            Assert.That(result.Width, Is.EqualTo((int)Math.Round(sourcePix.Width * scale)));
            Assert.That(result.Height, Is.EqualTo((int)Math.Round(sourcePix.Height * scale)));

            // TODO: Visually confirm successful rotation and then setup an assertion to compare that result is the same.
            string filename = string.Format(FileNameFormat, scale);
            this.SaveResult(writer, result, ResultsDirectory, filename);
        }
    }
}