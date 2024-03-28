namespace Tesseract.Tests
{
    using System.Collections.Generic;
    using System.IO;
    using NUnit.Framework;

    [TestFixture]
    public class ResultRendererTests : TesseractTestBase
    {
        [SetUp]
        public void Inititialse()
        {
            this._engine = CreateEngine();
        }

        [TearDown]
        public void Dispose()
        {
            if (this._engine != null)
            {
                this._engine.Dispose();
                this._engine = null;
            }
        }

        private TesseractEngine _engine;

        [Test]
        public void CanRenderResultsIntoTextFile()
        {
            string resultPath = TestResultRunFile(@"ResultRenderers/Text/phototest");
            using (IResultRenderer renderer = ResultRenderer.CreateTextRenderer(resultPath))
            {
                string examplePixPath = TestFilePath("Ocr/phototest.tif");
                this.ProcessFile(renderer, examplePixPath);
            }

            string expectedOutputFilename = Path.ChangeExtension(resultPath, "txt");
            Assert.That(File.Exists(expectedOutputFilename), $"Expected a Text file \"{expectedOutputFilename}\" to have been created; but none was found.");
        }

        [Test]
        public void CanRenderResultsIntoPdfFile()
        {
            string resultPath = TestResultRunFile(@"ResultRenderers/PDF/phototest");
            using (IResultRenderer renderer = ResultRenderer.CreatePdfRenderer(resultPath, DataPath, false))
            {
                string examplePixPath = TestFilePath("Ocr/phototest.tif");
                this.ProcessFile(renderer, examplePixPath);
            }

            string expectedOutputFilename = Path.ChangeExtension(resultPath, "pdf");
            using (IResultRenderer renderer = ResultRenderer.CreatePdfRenderer(resultPath, DataPath, false))
            {
                string examplePixPath = TestFilePath("Ocr/phototest.tif");
                this.ProcessImageFile(renderer, examplePixPath);
            }

            Assert.That(File.Exists(expectedOutputFilename), $"Expected a PDF file \"{expectedOutputFilename}\" to have been created; but none was found.");
        }

        [Test]
        public void CanRenderResultsIntoPdfFile1()
        {
            string resultPath = TestResultRunFile(@"ResultRenderers/PDF/phototest");
            using (IResultRenderer renderer = ResultRenderer.CreatePdfRenderer(resultPath, DataPath, false))
            {
                string examplePixPath = TestFilePath("Ocr/phototest.tif");
                this.ProcessImageFile(renderer, examplePixPath);
            }

            string expectedOutputFilename = Path.ChangeExtension(resultPath, "pdf");
            Assert.That(File.Exists(expectedOutputFilename), $"Expected a PDF file \"{expectedOutputFilename}\" to have been created; but none was found.");
        }

        [Test]
        public void CanRenderMultiplePageDocumentToPdfFile()
        {
            string resultPath = TestResultRunFile(@"ResultRenderers/PDF/multi-page");
            using (IResultRenderer renderer = ResultRenderer.CreatePdfRenderer(resultPath, DataPath, false))
            {
                string examplePixPath = TestFilePath("processing/multi-page.tif");
                this.ProcessMultipageTiff(renderer, examplePixPath);
            }

            string expectedOutputFilename = Path.ChangeExtension(resultPath, "pdf");
            using (IResultRenderer renderer = ResultRenderer.CreatePdfRenderer(resultPath, DataPath, false))
            {
                string examplePixPath = TestFilePath("processing/multi-page.tif");
                this.ProcessImageFile(renderer, examplePixPath);
            }

            Assert.That(File.Exists(expectedOutputFilename), $"Expected a PDF file \"{expectedOutputFilename}\" to have been created; but none was found.");
        }

        [Test]
        public void CanRenderMultiplePageDocumentToPdfFile1()
        {
            string resultPath = TestResultRunFile(@"ResultRenderers/PDF/multi-page");
            using (IResultRenderer renderer = ResultRenderer.CreatePdfRenderer(resultPath, DataPath, false))
            {
                string examplePixPath = TestFilePath("processing/multi-page.tif");
                this.ProcessImageFile(renderer, examplePixPath);
            }

            string expectedOutputFilename = Path.ChangeExtension(resultPath, "pdf");
            Assert.That(File.Exists(expectedOutputFilename), $"Expected a PDF file \"{expectedOutputFilename}\" to have been created; but none was found.");
        }

        [Test]
        public void CanRenderResultsIntoHOcrFile()
        {
            string resultPath = TestResultRunFile(@"ResultRenderers/HOCR/phototest");
            using (IResultRenderer renderer = ResultRenderer.CreateHOcrRenderer(resultPath))
            {
                string examplePixPath = TestFilePath("Ocr/phototest.tif");
                this.ProcessFile(renderer, examplePixPath);
            }

            string expectedOutputFilename = Path.ChangeExtension(resultPath, "hocr");
            Assert.That(File.Exists(expectedOutputFilename), $"Expected a HOCR file \"{expectedOutputFilename}\" to have been created; but none was found.");
        }

        [Test]
        public void CanRenderResultsIntoUnlvFile()
        {
            string resultPath = TestResultRunFile(@"ResultRenderers/UNLV/phototest");
            using (IResultRenderer renderer = ResultRenderer.CreateUnlvRenderer(resultPath))
            {
                string examplePixPath = TestFilePath("Ocr/phototest.tif");
                this.ProcessFile(renderer, examplePixPath);
            }

            string expectedOutputFilename = Path.ChangeExtension(resultPath, "unlv");
            Assert.That(File.Exists(expectedOutputFilename), $"Expected a Unlv file \"{expectedOutputFilename}\" to have been created; but none was found.");
        }

        [Test]
        public void CanRenderResultsIntoAltoFile()
        {
            string resultPath = TestResultRunFile(@"ResultRenderers/Alto/phototest");
            using (IResultRenderer renderer = ResultRenderer.CreateAltoRenderer(resultPath))
            {
                string examplePixPath = TestFilePath("Ocr/phototest.tif");
                this.ProcessFile(renderer, examplePixPath);
            }

            string expectedOutputFilename = Path.ChangeExtension(resultPath, "xml");
            Assert.That(File.Exists(expectedOutputFilename), $"Expected an xml file \"{expectedOutputFilename}\" to have been created; but none was found.");
        }


        [Test]
        public void CanRenderResultsIntoTsvFile()
        {
            string resultPath = TestResultRunFile(@"ResultRenderers/Tsv/phototest");
            using (IResultRenderer renderer = ResultRenderer.CreateTsvRenderer(resultPath))
            {
                string examplePixPath = TestFilePath("Ocr/phototest.tif");
                this.ProcessFile(renderer, examplePixPath);
            }

            string expectedOutputFilename = Path.ChangeExtension(resultPath, "tsv");
            Assert.That(File.Exists(expectedOutputFilename), $"Expected a Tsv file \"{expectedOutputFilename}\" to have been created; but none was found.");
        }

        [Test]
        public void CanRenderResultsIntoLSTMBoxFile()
        {
            string resultPath = TestResultRunFile(@"ResultRenderers/LSTMBox/phototest");
            using (IResultRenderer renderer = ResultRenderer.CreateLSTMBoxRenderer(resultPath))
            {
                string examplePixPath = TestFilePath("Ocr/phototest.tif");
                this.ProcessFile(renderer, examplePixPath);
            }

            string expectedOutputFilename = Path.ChangeExtension(resultPath, "box");
            Assert.That(File.Exists(expectedOutputFilename), $"Expected a box file \"{expectedOutputFilename}\" to have been created; but none was found.");
        }

        [Test]
        public void CanRenderResultsIntoWordStrBoxFile()
        {
            string resultPath = TestResultRunFile(@"ResultRenderers/WordStrBox/phototest");
            using (IResultRenderer renderer = ResultRenderer.CreateWordStrBoxRenderer(resultPath))
            {
                string examplePixPath = TestFilePath("Ocr/phototest.tif");
                this.ProcessFile(renderer, examplePixPath);
            }

            string expectedOutputFilename = Path.ChangeExtension(resultPath, "box");
            Assert.That(File.Exists(expectedOutputFilename), $"Expected a box file \"{expectedOutputFilename}\" to have been created; but none was found.");
        }

        [Test]
        public void CanRenderResultsIntoBoxFile()
        {
            string resultPath = TestResultRunFile(@"ResultRenderers/Box/phototest");
            using (IResultRenderer renderer = ResultRenderer.CreateBoxRenderer(resultPath))
            {
                string examplePixPath = TestFilePath("Ocr/phototest.tif");
                this.ProcessFile(renderer, examplePixPath);
            }

            string expectedOutputFilename = Path.ChangeExtension(resultPath, "box");
            Assert.That(File.Exists(expectedOutputFilename), $"Expected a Box file \"{expectedOutputFilename}\" to have been created; but none was found.");
        }

        [Test]
        public void CanRenderResultsIntoMultipleOutputFormats()
        {
            string resultPath = TestResultRunFile(@"ResultRenderers/PDF/phototest");
            var formats = new List<RenderedFormat> { RenderedFormat.HOCR, RenderedFormat.PDF_TEXTONLY, RenderedFormat.TEXT };
            using (var renderer = new AggregateResultRenderer(ResultRenderer.CreateRenderers(resultPath, DataPath, formats)))
            {
                string examplePixPath = TestFilePath("Ocr/phototest.tif");
                this.ProcessFile(renderer, examplePixPath);
            }

            string expectedOutputFilename = Path.ChangeExtension(resultPath, "pdf");
            Assert.That(File.Exists(expectedOutputFilename), $"Expected a PDF file \"{expectedOutputFilename}\" to have been created; but none was found.");
            expectedOutputFilename = Path.ChangeExtension(resultPath, "hocr");
            Assert.That(File.Exists(expectedOutputFilename), $"Expected a HOCR file \"{expectedOutputFilename}\" to have been created; but none was found.");
            expectedOutputFilename = Path.ChangeExtension(resultPath, "txt");
            Assert.That(File.Exists(expectedOutputFilename), $"Expected a TEXT file \"{expectedOutputFilename}\" to have been created; but none was found.");
        }

        [Test]
        public void CanRenderMultiplePageDocumentIntoMultipleResultRenderers()
        {
            string resultPath = TestResultRunFile(@"ResultRenderers/Aggregate/multi-page");
            using (var renderer = new AggregateResultRenderer(ResultRenderer.CreatePdfRenderer(resultPath, DataPath, false), ResultRenderer.CreateTextRenderer(resultPath)))
            {
                string examplePixPath = TestFilePath("processing/multi-page.tif");
                this.ProcessMultipageTiff(renderer, examplePixPath);
            }

            string expectedPdfOutputFilename = Path.ChangeExtension(resultPath, "pdf");
            Assert.That(File.Exists(expectedPdfOutputFilename), $"Expected a PDF file \"{expectedPdfOutputFilename}\" to have been created; but none was found.");

            string expectedTxtOutputFilename = Path.ChangeExtension(resultPath, "txt");
            Assert.That(File.Exists(expectedTxtOutputFilename), $"Expected a Text file \"{expectedTxtOutputFilename}\" to have been created; but none was found.");
        }

        private void ProcessMultipageTiff(IResultRenderer renderer, string filename)
        {
            string imageName = Path.GetFileNameWithoutExtension(filename);
            using PixArray pixA = PixArray.LoadMultiPageTiffFromFile(filename);
            int expectedPageNumber = -1;
            using (renderer.BeginDocument(imageName))
            {
                Assert.AreEqual(renderer.PageNumber, expectedPageNumber);
                foreach (Pix pix in pixA)
                {
                    using Page page = this._engine.Process(pix, imageName);
                    bool addedPage = renderer.AddPage(page);
                    expectedPageNumber++;

                    Assert.That(addedPage, Is.True);
                    Assert.That(renderer.PageNumber, Is.EqualTo(expectedPageNumber));
                }
            }

            Assert.That(renderer.PageNumber, Is.EqualTo(expectedPageNumber));
        }

        private void ProcessFile(IResultRenderer renderer, string filename)
        {
            string imageName = Path.GetFileNameWithoutExtension(filename);
            using (Pix pix = Pix.LoadFromFile(filename))
            {
                using (renderer.BeginDocument(imageName))
                {
                    Assert.AreEqual(renderer.PageNumber, -1);
                    using (Page page = this._engine.Process(pix, imageName))
                    {
                        bool addedPage = renderer.AddPage(page);

                        Assert.That(addedPage, Is.True);
                        Assert.That(renderer.PageNumber, Is.EqualTo(0));
                    }
                }

                Assert.AreEqual(renderer.PageNumber, 0);
            }
        }

        private void ProcessImageFile(IResultRenderer renderer, string filename)
        {
            string imageName = Path.GetFileNameWithoutExtension(filename);
            using (PixArray pixA = this.ReadImageFileIntoPixArray(filename))
            {
                int expectedPageNumber = -1;
                using (renderer.BeginDocument(imageName))
                {
                    Assert.AreEqual(renderer.PageNumber, expectedPageNumber);
                    foreach (Pix pix in pixA)
                        using (Page page = this._engine.Process(pix, imageName))
                        {
                            bool addedPage = renderer.AddPage(page);
                            expectedPageNumber++;

                            Assert.That(addedPage, Is.True);
                            Assert.That(renderer.PageNumber, Is.EqualTo(expectedPageNumber));
                        }
                }

                Assert.That(renderer.PageNumber, Is.EqualTo(expectedPageNumber));
            }
        }

        private PixArray ReadImageFileIntoPixArray(string filename)
        {
            if (filename.ToLower().EndsWith(".tif") || filename.ToLower().EndsWith(".tiff")) return PixArray.LoadMultiPageTiffFromFile(filename);

            var pa = PixArray.Create(0);
            pa.Add(Pix.LoadFromFile(filename));
            return pa;
        }
    }
}