namespace Tesseract.Tests
{
    using System;
    using System.IO;
    using Interop;
    using Interop.Abstractions;
    using NUnit.Framework;

    [TestFixture]
    public class AnalyseResultTests : TesseractTestBase
    {
        [TearDown]
        public void Dispose()
        {
            if (this.engine != null)
            {
                this.engine.Dispose();
                this.engine = null;
            }
        }

        [SetUp]
        public void Init()
        {
            if (!Directory.Exists(this.ResultsDirectory)) Directory.CreateDirectory(this.ResultsDirectory);

            this.engine = CreateEngine("osd");
        }

        private string ResultsDirectory => TestResultPath(@"Analysis/");

        private const string ExampleImagePath = @"Ocr/phototest.tif";

        private TesseractEngine engine;

        [Test]
        [TestCase(null)]
        [TestCase(90f)]
        [TestCase(180f)]
        public void AnalyseLayout_RotatedImage(float? angle)
        {
            string exampleImagePath = TestFilePath("Ocr/phototest.tif");
            using (Pix img = this.LoadTestImage(ExampleImagePath))
            {
                using (Pix rotatedImage = angle.HasValue ? img.Rotate(MathHelper.ToRadians(angle.Value)) : img.Clone())
                {
                    rotatedImage.Save(TestResultRunFile(string.Format(@"AnalyseResult/AnalyseLayout_RotateImage_{0}.png", angle)));

                    this.engine.DefaultPageSegMode = PageSegMode.AutoOsd;
                    using (Page page = this.engine.Process(rotatedImage))
                    {
                        using (ResultIterator pageLayout = page.GetIterator())
                        {
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
                    }
                }
            }
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
            using (Pix img = this.LoadTestImage(ExampleImagePath))
            {
                using (Pix rotatedPix = img.Rotate((float)Math.PI))
                {
                    using (Page page = this.engine.Process(rotatedPix, pageSegMode))
                    {
                        int orientation;
                        float confidence;
                        string scriptName;
                        float scriptConfidence;

                        page.DetectBestOrientationAndScript(out orientation, out confidence, out scriptName, out scriptConfidence);

                        Assert.That(orientation, Is.EqualTo(180));
                        Assert.That(scriptName, Is.EqualTo("Latin"));
                    }
                }
            }
        }

        [Test]
        [TestCase(0)]
        [TestCase(90)]
        [TestCase(180)]
        [TestCase(270)]
        public void DetectOrientation_Degrees_RotatedImage(int expectedOrientation)
        {
            using (Pix img = this.LoadTestImage(ExampleImagePath))
            {
                using (Pix rotatedPix = img.Rotate((float)expectedOrientation / 360 * (float)Math.PI * 2))
                {
                    using (Page page = this.engine.Process(rotatedPix, PageSegMode.OsdOnly))
                    {
                        int orientation;
                        float confidence;
                        string scriptName;
                        float scriptConfidence;

                        page.DetectBestOrientationAndScript(out orientation, out confidence, out scriptName, out scriptConfidence);

                        Assert.That(orientation, Is.EqualTo(expectedOrientation));
                        Assert.That(scriptName, Is.EqualTo("Latin"));
                    }
                }
            }
        }

        [Test]
        [TestCase(0)]
        [TestCase(90)]
        [TestCase(180)]
        [TestCase(270)]
        public void DetectOrientation_Legacy_RotatedImage(int expectedOrientationDegrees)
        {
            using Pix img = this.LoadTestImage(ExampleImagePath);
            using Pix rotatedPix = img.Rotate((float)expectedOrientationDegrees / 360 * (float)Math.PI * 2);
            using Page page = this.engine.Process(rotatedPix, PageSegMode.OsdOnly);
            page.DetectBestOrientation(out int orientation, out float _);

            Assert.That(orientation, Is.EqualTo(expectedOrientationDegrees));
        }


        [Test]
        public void GetImage(
            [Values(PageIteratorLevel.Block, PageIteratorLevel.Para, PageIteratorLevel.TextLine, PageIteratorLevel.Word, PageIteratorLevel.Symbol)]
            PageIteratorLevel level,
            [Values(0, 3)] int padding)
        {
            using Pix img = this.LoadTestImage(ExampleImagePath);
            using Page page = this.engine.Process(img);
            using ResultIterator pageLayout = page.GetIterator();
            pageLayout.Begin();
            // get symbol
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
            string fullExampleImagePath = TestFilePath(path);
            return Pix.LoadFromFile(fullExampleImagePath);
        }
    }
}