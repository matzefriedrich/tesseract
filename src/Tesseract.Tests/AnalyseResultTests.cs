namespace Tesseract.Tests
{
    using Abstractions;

    using Interop;
    using Interop.Abstractions;

    using Microsoft.Extensions.DependencyInjection;

    using NUnit.Framework;

    [TestFixture]
    public class AnalyseResultTests : TesseractTestBase
    {
        [TearDown]
        public void Dispose()
        {
            this.provider?.Dispose();
        }

        [SetUp]
        public void Init()
        {
            if (!Directory.Exists(this.ResultsDirectory))
                Directory.CreateDirectory(this.ResultsDirectory);

            this.services.AddTesseract();
            this.provider = this.services.BuildServiceProvider();
        }

        private readonly ServiceCollection services = new();

        private ITesseractEngine CreateEngine()
        {
            var engineFactory = (this.provider ?? throw new InvalidOperationException()).GetRequiredService<TesseractEngineFactory>();
            TesseractEngineOptions engineOptions = new TesseractEngineOptionBuilder(DataPath, "osd")
                .Build();

            return engineFactory(engineOptions);
        }

        private string ResultsDirectory => TestResultPath(@"Analysis/");

        private const string ExampleImagePath = @"Ocr/phototest.tif";

        private ServiceProvider? provider;

        [Test]
        [TestCase(null)]
        [TestCase(90f)]
        [TestCase(180f)]
        public void AnalyseLayout_RotatedImage(float? angle)
        {
            // Arrange
            using Pix img = this.LoadTestImage(ExampleImagePath);
            using Pix rotatedImage = angle.HasValue ? img.Rotate(MathHelper.ToRadians(angle.Value)) : img.Clone();
            rotatedImage.Save(TestResultRunFile($@"AnalyseResult/AnalyseLayout_RotateImage_{angle}.png"));

            using ITesseractEngine engine = this.CreateEngine();
            if (engine != null) engine.DefaultPageSegMode = PageSegMode.AutoOsd;

            // Act
            using Page page = engine?.Process(rotatedImage) ?? throw new ArgumentNullException("tesseractEngine?.Process(rotatedImage)");
            using ResultIterator pageLayout = page.GetIterator();
            pageLayout.Begin();
            do
            {
                ElementProperties result = pageLayout.GetProperties();

                float rotation = angle ?? 0;
                this.ExpectedOrientation(rotation, out Orientation orient, out float _);
                Assert.That(result.Orientation, Is.EqualTo(orient));

                if (angle.HasValue)
                {
                    if (angle == 180f)
                    {
                        // This isn't correct...
                        Assert.That(result.WritingDirection, Is.EqualTo(WritingDirection.LeftToRight));
                        Assert.That(result.TextLineOrder, Is.EqualTo(TextLineOrder.TopToBottom));
                    }
                    else if (angle == 90f)
                    {
                        Assert.That(result.WritingDirection, Is.EqualTo(WritingDirection.LeftToRight));
                        Assert.That(result.TextLineOrder, Is.EqualTo(TextLineOrder.TopToBottom));
                    }
                    else
                    {
                        Assert.Fail("Angle not supported.");
                    }
                }
                else
                {
                    Assert.That(result.WritingDirection, Is.EqualTo(WritingDirection.LeftToRight));
                    Assert.That(result.TextLineOrder, Is.EqualTo(TextLineOrder.TopToBottom));
                }
            } while (pageLayout.Next(PageIteratorLevel.Block));
        }

        [Test]
        public void CanDetectOrientationForMode(
            [Values(PageSegMode.Auto,
                PageSegMode.AutoOnly,
                PageSegMode.AutoOsd,
                PageSegMode.CircleWord,
                PageSegMode.OsdOnly,
                PageSegMode.SingleBlock,
                PageSegMode.SingleBlockVertText,
                PageSegMode.SingleChar,
                PageSegMode.SingleColumn,
                PageSegMode.SingleLine,
                PageSegMode.SingleWord)]
            PageSegMode pageSegMode)
        {
            using ITesseractEngine engine = this.CreateEngine();

            using Pix img = this.LoadTestImage(ExampleImagePath);
            using Pix rotatedPix = img.Rotate((float)Math.PI);
            using Page page = engine.Process(rotatedPix, pageSegMode);

            page.DetectBestOrientationAndScript(out int orientation, out float _, out string scriptName, out float _);

            // Assert
            Assert.That(orientation, Is.EqualTo(180));
            Assert.That(scriptName, Is.EqualTo("Latin"));
        }

        [Test]
        [TestCase(0)]
        [TestCase(90)]
        [TestCase(180)]
        [TestCase(270)]
        public void DetectOrientation_Degrees_RotatedImage(int expectedOrientation)
        {
            // Arrange
            using ITesseractEngine engine = this.CreateEngine();

            using Pix img = this.LoadTestImage(ExampleImagePath);
            using Pix rotatedPix = img.Rotate((float)expectedOrientation / 360 * (float)Math.PI * 2);

            // Act
            using Page page = engine.Process(rotatedPix, PageSegMode.OsdOnly);

            page.DetectBestOrientationAndScript(out int orientation, out float _, out string scriptName, out float _);

            // Assert
            Assert.That(orientation, Is.EqualTo(expectedOrientation));
            Assert.That(scriptName, Is.EqualTo("Latin"));
        }

        [Test]
        [TestCase(0)]
        [TestCase(90)]
        [TestCase(180)]
        [TestCase(270)]
        public void DetectOrientation_Legacy_RotatedImage(int expectedOrientationDegrees)
        {
            // Arrange
            using ITesseractEngine engine = this.CreateEngine();

            using Pix img = this.LoadTestImage(ExampleImagePath);
            using Pix rotatedPix = img.Rotate((float)expectedOrientationDegrees / 360 * (float)Math.PI * 2);

            // Act
            using Page page = engine.Process(rotatedPix, PageSegMode.OsdOnly);
            page.DetectBestOrientation(out int orientation, out float _);

            // Assert
            Assert.That(orientation, Is.EqualTo(expectedOrientationDegrees));
        }

        [Test]
        public void GetImage(
            [Values(PageIteratorLevel.Block, PageIteratorLevel.Para, PageIteratorLevel.TextLine, PageIteratorLevel.Word, PageIteratorLevel.Symbol)]
            PageIteratorLevel level,
            [Values(0, 3)] int padding)
        {
            // Arrange
            using ITesseractEngine engine = this.CreateEngine();

            using Pix img = this.LoadTestImage(ExampleImagePath);
            using Page? page = engine.Process(img);
            using ResultIterator pageLayout = page?.GetIterator() ?? throw new ArgumentNullException("page?.GetIterator()");
            pageLayout.Begin();

            // Act
            using Pix elementImg = pageLayout.GetImage(level, padding, out int x, out int y);
            var elementImgFilename = $@"AnalyseResult/GetImage/ResultIterator_Image_{level}_{padding}_at_({x},{y}).png";

            // TODO: Ensure generated pix is equal to expected pix, only saving it if it's not.
            string destFilename = TestResultRunFile(elementImgFilename);
            elementImg.Save(destFilename, ImageFormat.Png);
        }

        private void ExpectedOrientation(float rotation, out Orientation orientation, out float deskew)
        {
            rotation = rotation % 360f;
            rotation = rotation < 0 ? rotation + 360 : rotation;

            if (rotation >= 315 || rotation < 45)
            {
                orientation = Orientation.PageUp;
                deskew = -rotation;
            }
            else if (rotation >= 45 && rotation < 135)
            {
                orientation = Orientation.PageRight;
                deskew = 90 - rotation;
            }
            else if (rotation >= 135 && rotation < 225)
            {
                orientation = Orientation.PageDown;
                deskew = 180 - rotation;
            }
            else if (rotation >= 225 && rotation < 315)
            {
                orientation = Orientation.PageLeft;
                deskew = 270 - rotation;
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(rotation));
            }
        }

        private Pix LoadTestImage(string path)
        {
            var pixFactory = (this.provider ?? throw new InvalidOperationException()).GetRequiredService<IPixFactory>();
            string fullExampleImagePath = MakeAbsoluteTestFilePath(path);
            return pixFactory.LoadFromFile(fullExampleImagePath);
        }
    }
}