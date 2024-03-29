namespace Tesseract.Tests.Leptonica.PixTests
{
    using Abstractions;

    using NUnit.Framework;

    [TestFixture]
    public class ImageManipulationTests : TesseractTestBase
    {
        private const string ResultsDirectory = @"Results/ImageManipulation/";

        [Test]
        public void DescewTest()
        {
            string sourcePixPath = TestFilePath(@"Scew/scewed-phototest.png");
            using Pix sourcePix = Pix.LoadFromFile(sourcePixPath);
            using Pix descewedImage = sourcePix.Deskew(new ScewSweep(range: 45), Pix.DefaultBinarySearchReduction, Pix.DefaultBinaryThreshold, out Scew scew);
            Assert.That(scew.Angle, Is.EqualTo(-9.953125F).Within(0.00001));
            Assert.That(scew.Confidence, Is.EqualTo(3.782913F).Within(0.00001));

            this.SaveResult(descewedImage, "descewedImage.png");
        }

        [Test]
        public void OtsuBinarizationTest()
        {
            string sourcePixFilename = TestFilePath(@"Binarization/neo-8bit.png");
            using Pix sourcePix = Pix.LoadFromFile(sourcePixFilename);
            using Pix binarizedImage = sourcePix.BinarizeOtsuAdaptiveThreshold(200, 200, 10, 10, 0.1F);
            Assert.That(binarizedImage, Is.Not.Null);
            Assert.That(binarizedImage.Handle, Is.Not.EqualTo(IntPtr.Zero));
            this.SaveResult(binarizedImage, "binarizedOtsuImage.png");
        }

        [Test]
        public void SauvolaBinarizationTest()
        {
            string sourcePixFilename = TestFilePath(@"Binarization/neo-8bit-grayscale.png");
            using Pix sourcePix = Pix.LoadFromFile(sourcePixFilename);
            using Pix grayscalePix = sourcePix.ConvertRGBToGray(1, 1, 1);
            using Pix binarizedImage = grayscalePix.BinarizeSauvola(10, 0.35f, false);
            Assert.That(binarizedImage, Is.Not.Null);
            Assert.That(binarizedImage.Handle, Is.Not.EqualTo(IntPtr.Zero));
            this.SaveResult(binarizedImage, "binarizedSauvolaImage.png");
        }

        [Test]
        public void SauvolaTiledBinarizationTest()
        {
            string sourcePixFilename = TestFilePath(@"Binarization/neo-8bit-grayscale.png");
            using Pix sourcePix = Pix.LoadFromFile(sourcePixFilename);
            using Pix grayscalePix = sourcePix.ConvertRGBToGray(1, 1, 1);
            using Pix binarizedImage = grayscalePix.BinarizeSauvolaTiled(10, 0.35f, 2, 2);
            Assert.That(binarizedImage, Is.Not.Null);
            Assert.That(binarizedImage.Handle, Is.Not.EqualTo(IntPtr.Zero));
            this.SaveResult(binarizedImage, "binarizedSauvolaTiledImage.png");
        }

        [Test]
        public void ConvertRGBToGrayTest()
        {
            string sourcePixFilename = TestFilePath(@"Conversion/photo_rgb_32bpp.tif");
            using Pix sourcePix = Pix.LoadFromFile(sourcePixFilename);
            using Pix grayscaleImage = sourcePix.ConvertRGBToGray();
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
            const string FileNameFormat = "rotation_{0}degrees.jpg";
            float angleAsRadians = MathHelper.ToRadians(angle);

            string sourcePixFilename = TestFilePath(@"Conversion/photo_rgb_32bpp.tif");
            using Pix sourcePix = Pix.LoadFromFile(sourcePixFilename);
            using Pix result = sourcePix.Rotate(angleAsRadians);
            // TODO: Visualy confirm successful rotation and then setup an assertion to compare that result is the same.
            string filename = string.Format(FileNameFormat, angle);
            this.SaveResult(result, filename);
        }

        [Test]
        public void RemoveLinesTest()
        {
            string sourcePixFilename = TestFilePath(@"processing/table.png");
            using Pix sourcePix = Pix.LoadFromFile(sourcePixFilename);
            // remove horizontal lines
            using Pix result = sourcePix.RemoveLines();
            // rotate 90 degrees cw
            using Pix result1 = result.Rotate90(1);
            // effectively remove vertical lines
            using Pix result2 = result1.RemoveLines();
            // rotate 90 degrees ccw
            using Pix result3 = result2.Rotate90(-1);
            // TODO: Visualy confirm successful rotation and then setup an assertion to compare that result is the same.
            this.SaveResult(result3, "tableBordersRemoved.png");
        }

        [Test]
        public void DespeckleTest()
        {
            string sourcePixFilename = TestFilePath(@"processing/w91frag.jpg");
            using Pix sourcePix = Pix.LoadFromFile(sourcePixFilename);
            // remove speckles
            using Pix result = sourcePix.Despeckle(Pix.SEL_STR2, 2);
            // TODO: Visualy confirm successful despeckle and then setup an assertion to compare that result is the same.
            this.SaveResult(result, "w91frag-despeckled.png");
        }

        [Test]
        public void Scale_RGB_ShouldBeScaledBySpecifiedFactor(
            [Values(0.25f, 0.5f, 0.75f, 1, 1.25f, 1.5f, 1.75f, 2, 4, 8)]
            float scale)
        {
            const string FileNameFormat = "scale_{0}.jpg";

            string sourcePixFilename = TestFilePath(@"Conversion/photo_rgb_32bpp.tif");
            using Pix sourcePix = Pix.LoadFromFile(sourcePixFilename);
            using Pix result = sourcePix.Scale(scale, scale);
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