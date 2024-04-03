namespace Tesseract.Tests.Leptonica
{
    using System.Drawing;
    using System.Drawing.Imaging;
    using Abstractions;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using ImageFormat = Interop.Abstractions.ImageFormat;

    public class ConvertBitmapToPixTests : TesseractTestBase
    {
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

        // Test for [Issue #166](https://github.com/charlesw/tesseract/issues/166)
        [Test]
        public void Convert_ScaledBitmapToPix()
        {
            // Arrange
            var sut = (this.provider ?? throw new InvalidOperationException()).GetRequiredService<IBitmapToPixConverter>();
            var writer = this.provider.GetRequiredService<IPixFileWriter>();

            string sourceFilePath = MakeAbsoluteTestFilePath("Conversion/photo_rgb_32bpp.tif");

            using var source = new Bitmap(sourceFilePath);
            using var scaledSource = new Bitmap(source, new Size(source.Width * 2, source.Height * 2));

            // Act
            using Pix dest = sut.Convert(scaledSource);
            writer.Save(dest, this.TestResultRunFile("Conversion/ScaledBitmapToPix_rgb_32bpp.tif"), ImageFormat.Tiff);

            // Assert
            Assert.That(scaledSource.GetBpp(), Is.EqualTo(32));
            this.AssertAreEquivalent(scaledSource, dest, true);
        }

        [Test]
        [TestCase(PixelFormat.Format1bppIndexed)] // Note: 1bpp will not save pixmap when writing out the result, this is a limitation of leptonica (see pixWriteToTiffStream)
        [TestCase(PixelFormat.Format4bppIndexed, Ignore = "4bpp images not supported.")]
        [TestCase(PixelFormat.Format8bppIndexed)]
        [TestCase(PixelFormat.Format32bppRgb)]
        [TestCase(PixelFormat.Format32bppArgb)]
        public void Convert_BitmapToPix(PixelFormat pixelFormat)
        {
            // Arrange
            var sut = (this.provider ?? throw new InvalidOperationException()).GetRequiredService<IBitmapToPixConverter>();
            var writer = this.provider.GetRequiredService<IPixFileWriter>();

            int depth = Image.GetPixelFormatSize(pixelFormat);

            string pixType;
            if (depth < 16) pixType = "palette";
            else if (depth == 16) pixType = "grayscale";
            else pixType = Image.IsAlphaPixelFormat(pixelFormat) ? "argb" : "rgb";

            var sourceFile = $"Conversion/photo_{pixType}_{depth}bpp.tif";
            string sourceFilePath = MakeAbsoluteTestFilePath(sourceFile);

            using var source = new Bitmap(sourceFilePath);

            // Act
            using Pix dest = sut.Convert(source);
            var destFilename = $"Conversion/BitmapToPix_{pixType}_{depth}bpp.tif";
            writer.Save(dest, this.TestResultRunFile(destFilename), ImageFormat.Tiff);

            // Assert
            Assert.That(source.PixelFormat, Is.EqualTo(pixelFormat));
            Assert.That(source.GetBpp(), Is.EqualTo(depth));
            this.AssertAreEquivalent(source, dest, true);
        }

        /// <summary>
        ///     Test case for https://github.com/charlesw/tesseract/issues/180
        /// </summary>
        [Test]
        public void Convert_BitmapToPix_Format8bppIndexed()
        {
            // Arrange
            var sut = (this.provider ?? throw new InvalidOperationException()).GetRequiredService<IBitmapToPixConverter>();
            var pixFileWriter = this.provider.GetRequiredService<IPixFileWriter>();

            string sourceFile = MakeAbsoluteTestFilePath("Conversion/photo_palette_8bpp.png");
            using var source = new Bitmap(sourceFile);

            // Act
            using Pix dest = sut.Convert(source);
            string destFilename = this.TestResultRunFile("Conversion/BitmapToPix_palette_8bpp.png");
            pixFileWriter.Save(dest, destFilename, ImageFormat.Png);

            // Assert
            Assert.That(source.GetBpp(), Is.EqualTo(8));
            Assert.That(source.PixelFormat, Is.EqualTo(PixelFormat.Format8bppIndexed));
            this.AssertAreEquivalent(source, dest, true);
        }

        [Test]
        [TestCase(1, true, false)]
        [TestCase(1, false, false, Ignore = "1bpp images with colormap are not supported")]
        [TestCase(4, false, false, Ignore = "4bpp images not supported.")]
        [TestCase(4, true, false, Ignore = "4bpp images not supported.")]
        [TestCase(8, false, false)]
        [TestCase(8, true, false, Ignore = "Haven't yet created a 8bpp grayscale test image.")]
        [TestCase(32, false, true)]
        [TestCase(32, false, false)]
        public void Convert_PixToBitmap(int depth, bool isGrayscale, bool includeAlpha)
        {
            // Arrange
            var pixFactory = (this.provider ?? throw new InvalidOperationException()).GetRequiredService<IPixFactory>();

            bool hasPalette = depth < 16 && !isGrayscale;

            string pixType;
            if (isGrayscale) pixType = "grayscale";
            else if (hasPalette) pixType = "palette";
            else pixType = "rgb";

            string sourceFile = MakeAbsoluteTestFilePath($"Conversion/photo_{pixType}_{depth}bpp.tif");
            var converter = new PixToBitmapConverter();
            using Pix source = pixFactory.LoadFromFile(sourceFile);

            // Act
            using Bitmap dest = converter.Convert(source, includeAlpha);
            string destFilename = this.TestResultRunFile($"Conversion/PixToBitmap_{pixType}_{depth}bpp.tif");
            dest.Save(destFilename, System.Drawing.Imaging.ImageFormat.Tiff);

            // Assert
            Assert.That(source.Depth, Is.EqualTo(depth));
            if (hasPalette)
                Assert.That(source.Colormap, Is.Not.Null, "Expected source image to have color map\\palette.");
            else
                Assert.That(source.Colormap, Is.Null, "Expected source image to be grayscale.");

            this.AssertAreEquivalent(dest, source, includeAlpha);
        }

        private void AssertAreEquivalent(Bitmap bmp, Pix pix, bool checkAlpha)
        {
            // verify img metadata
            Assert.That(pix.Width, Is.EqualTo(bmp.Width));
            Assert.That(pix.Height, Is.EqualTo(bmp.Height));
            //Assert.That(pix.Resolution.X, Is.EqualTo(bmp.HorizontalResolution));
            //Assert.That(pix.Resolution.Y, Is.EqualTo(bmp.VerticalResolution));

            // do some random sampling over image
            int height = pix.Height;
            int width = pix.Width;
            for (var y = 0; y < height; y += height)
            for (var x = 0; x < width; x += width)
            {
                var sourcePixel = bmp.GetPixel(x, y).ToPixColor();
                PixColor destPixel = this.GetPixel(pix, x, y);
                if (checkAlpha)
                    Assert.That(destPixel, Is.EqualTo(sourcePixel), "Expected pixel at <{0},{1}> to be same in both source and dest.", x, y);
                else
                    Assert.That(destPixel, Is.EqualTo(sourcePixel).Using<PixColor>((c1, c2) => c1.Red == c2.Red && c1.Blue == c2.Blue && c1.Green == c2.Green ? 0 : 1), "Expected pixel at <{0},{1}> to be same in both source and dest.", x, y);
            }
        }

        private unsafe PixColor GetPixel(Pix pix, int x, int y)
        {
            int pixDepth = pix.Depth;
            PixData pixData = pix.GetData();
            uint* pixLine = (uint*)pixData.Data + pixData.WordsPerLine * y;
            uint pixValue;
            if (pixDepth == 1)
                pixValue = PixData.GetDataBit(pixLine, x);
            else if (pixDepth == 4)
                pixValue = PixData.GetDataQBit(pixLine, x);
            else if (pixDepth == 8)
                pixValue = PixData.GetDataByte(pixLine, x);
            else if (pixDepth == 32)
                pixValue = PixData.GetDataFourByte(pixLine, x);
            else
                throw new ArgumentException($"Bit depth of {pix.Depth} is not supported.", nameof(pix));

            if (pix.Colormap != null) return pix.Colormap[(int)pixValue];

            if (pixDepth == 32) return PixColor.FromRgba(pixValue);

            var grayscale = (byte)(pixValue * 255 / ((1 << 16) - 1));
            return new PixColor(grayscale, grayscale, grayscale);
        }
    }
}