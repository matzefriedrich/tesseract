namespace Tesseract.Tests.Leptonica.PixTests
{
    using Abstractions;

    using Microsoft.Extensions.DependencyInjection;

    using NUnit.Framework;

    [TestFixture]
    public class ImageManipulationTests : TesseractTestBase
    {
        [SetUp]
        public void Init()
        {
            this.services.AddTesseract();

            this.provider = this.services.BuildServiceProvider();
        }

        [TearDown]
        public void Dispose()
        {
            this.provider?.Dispose();
        }

        private readonly ServiceCollection services = new();
        private ServiceProvider? provider;
        private const string ResultsDirectory = @"Results/ImageManipulation/";

        [Test]
        public void DescewTest()
        {
            // Arrange
            var pixFactory = (this.provider ?? throw new InvalidOperationException()).GetRequiredService<IPixFactory>();

            string sourcePixPath = MakeAbsoluteTestFilePath(@"Scew/scewed-phototest.png");
            using Pix sourcePix = pixFactory.LoadFromFile(sourcePixPath);

            // Act
            using Pix descewedImage = sourcePix.Deskew(new ScewSweep(range: 45), Pix.DefaultBinarySearchReduction, Pix.DefaultBinaryThreshold, out Scew scew);

            // Assert
            Assert.That(scew.Angle, Is.EqualTo(-9.953125F).Within(0.00001));
            Assert.That(scew.Confidence, Is.EqualTo(3.782913F).Within(0.00001));

            this.SaveResult(descewedImage, "descewedImage.png");
        }

        [Test]
        public void OtsuBinarizationTest()
        {
            // Arrange
            var pixFactory = (this.provider ?? throw new InvalidOperationException()).GetRequiredService<IPixFactory>();

            string sourcePixFilename = MakeAbsoluteTestFilePath(@"Binarization/neo-8bit.png");
            using Pix sourcePix = pixFactory.LoadFromFile(sourcePixFilename);

            // Act
            using Pix binarizedImage = sourcePix.BinarizeOtsuAdaptiveThreshold(200, 200, 10, 10, 0.1F);

            // Assert
            Assert.That(binarizedImage, Is.Not.Null);
            Assert.That(binarizedImage.Handle, Is.Not.EqualTo(IntPtr.Zero));

            this.SaveResult(binarizedImage, "binarizedOtsuImage.png");
        }

        [Test]
        public void SauvolaBinarizationTest()
        {
            // Arrange
            var pixFactory = (this.provider ?? throw new InvalidOperationException()).GetRequiredService<IPixFactory>();

            string sourcePixFilename = MakeAbsoluteTestFilePath(@"Binarization/neo-8bit-grayscale.png");
            using Pix sourcePix = pixFactory.LoadFromFile(sourcePixFilename);
            using Pix grayscalePix = sourcePix.ConvertRGBToGray(1, 1, 1);

            // Act
            using Pix binarizedImage = grayscalePix.BinarizeSauvola(10, 0.35f, false);

            // Assert
            Assert.That(binarizedImage, Is.Not.Null);
            Assert.That(binarizedImage.Handle, Is.Not.EqualTo(IntPtr.Zero));

            this.SaveResult(binarizedImage, "binarizedSauvolaImage.png");
        }

        [Test]
        public void SauvolaTiledBinarizationTest()
        {
            // Arrange
            var pixFactory = (this.provider ?? throw new InvalidOperationException()).GetRequiredService<IPixFactory>();

            string sourcePixFilename = MakeAbsoluteTestFilePath(@"Binarization/neo-8bit-grayscale.png");
            using Pix sourcePix = pixFactory.LoadFromFile(sourcePixFilename);
            using Pix grayscalePix = sourcePix.ConvertRGBToGray(1, 1, 1);

            // Act
            using Pix binarizedImage = grayscalePix.BinarizeSauvolaTiled(10, 0.35f, 2, 2);

            // Assert
            Assert.That(binarizedImage, Is.Not.Null);
            Assert.That(binarizedImage.Handle, Is.Not.EqualTo(IntPtr.Zero));

            this.SaveResult(binarizedImage, "binarizedSauvolaTiledImage.png");
        }

        [Test]
        public void ConvertRGBToGrayTest()
        {
            // Arrange
            var pixFactory = (this.provider ?? throw new InvalidOperationException()).GetRequiredService<IPixFactory>();

            string sourcePixFilename = MakeAbsoluteTestFilePath(@"Conversion/photo_rgb_32bpp.tif");
            using Pix sourcePix = pixFactory.LoadFromFile(sourcePixFilename);

            // Act
            using Pix grayscaleImage = sourcePix.ConvertRGBToGray();

            // Assert
            Assert.That(grayscaleImage.Depth, Is.EqualTo(8));

            this.SaveResult(grayscaleImage, "grayscaleImage.jpg");
        }

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

            const string FileNameFormat = "rotation_{0}degrees.jpg";
            float angleAsRadians = MathHelper.ToRadians(angle);

            string sourcePixFilename = MakeAbsoluteTestFilePath(@"Conversion/photo_rgb_32bpp.tif");
            using Pix sourcePix = pixFactory.LoadFromFile(sourcePixFilename);

            // Act
            using Pix result = sourcePix.Rotate(angleAsRadians);

            // Assert
            // TODO: Visually confirm successful rotation and then setup an assertion to compare that result is the same.
            string filename = string.Format(FileNameFormat, angle);
            this.SaveResult(result, filename);
        }

        [Test]
        public void RemoveLinesTest()
        {
            // Arrange
            var pixFactory = (this.provider ?? throw new InvalidOperationException()).GetRequiredService<IPixFactory>();

            string sourcePixFilename = MakeAbsoluteTestFilePath(@"processing/table.png");
            using Pix sourcePix = pixFactory.LoadFromFile(sourcePixFilename);

            // Act

            // remove horizontal lines
            using Pix result = sourcePix.RemoveLines();
            // rotate 90 degrees cw
            using Pix result1 = result.Rotate90(1);
            // effectively remove vertical lines
            using Pix result2 = result1.RemoveLines();
            // rotate 90 degrees ccw
            using Pix result3 = result2.Rotate90(-1);

            // Assert

            // TODO: Visualy confirm successful rotation and then setup an assertion to compare that result is the same.
            this.SaveResult(result3, "tableBordersRemoved.png");
        }

        [Test]
        public void DespeckleTest()
        {
            // Arrange
            var pixFactory = (this.provider ?? throw new InvalidOperationException()).GetRequiredService<IPixFactory>();

            string sourcePixFilename = MakeAbsoluteTestFilePath(@"processing/w91frag.jpg");
            using Pix sourcePix = pixFactory.LoadFromFile(sourcePixFilename);

            // Act
            using Pix result = sourcePix.Despeckle(Pix.SEL_STR2, 2);

            // Assert
            // TODO: Visualy confirm successful despeckle and then setup an assertion to compare that result is the same.
            this.SaveResult(result, "w91frag-despeckled.png");
        }

        [Test]
        public void Scale_RGB_ShouldBeScaledBySpecifiedFactor([Values(0.25f, 0.5f, 0.75f, 1, 1.25f, 1.5f, 1.75f, 2, 4, 8)] float scale)
        {
            // Arrange
            var pixFactory = (this.provider ?? throw new InvalidOperationException()).GetRequiredService<IPixFactory>();

            const string FileNameFormat = "scale_{0}.jpg";

            string sourcePixFilename = MakeAbsoluteTestFilePath(@"Conversion/photo_rgb_32bpp.tif");
            using Pix sourcePix = pixFactory.LoadFromFile(sourcePixFilename);

            // Act
            using Pix result = sourcePix.Scale(scale, scale);

            // Assert
            Assert.That(result.Width, Is.EqualTo((int)Math.Round(sourcePix.Width * scale)));
            Assert.That(result.Height, Is.EqualTo((int)Math.Round(sourcePix.Height * scale)));

            // TODO: Visualy confirm successful rotation and then setup an assertion to compare that result is the same.
            string filename = string.Format(FileNameFormat, scale);
            this.SaveResult(result, filename);
        }

        private void SaveResult(Pix result, string filename)
        {
            string runFilename = TestResultRunFile(Path.Combine(ResultsDirectory, filename));
            result.Save(runFilename);
        }
    }
}