namespace Tesseract.Tests
{
    using Abstractions;

    using Microsoft.Extensions.DependencyInjection;

    using NUnit.Framework;

    [TestFixture]
    public class ResultRendererTests : TesseractTestBase
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

        [Test]
        public void CanRenderResultsIntoTextFile()
        {
            // Arrange
            var rendererFactory = (this.provider ?? throw new InvalidOperationException()).GetRequiredService<IResultRendererFactory>();

            string resultPath = TestResultRunFile(@"ResultRenderers/Text/phototest");

            // Act
            using (IResultRenderer renderer = rendererFactory.CreateTextRenderer(resultPath))
            {
                string examplePixPath = MakeAbsoluteTestFilePath("Ocr/phototest.tif");
                this.ProcessFileActAssertHelper(renderer, examplePixPath);
            }

            // Assert
            string expectedOutputFilename = Path.ChangeExtension(resultPath, "txt");
            Assert.That(File.Exists(expectedOutputFilename), $"Expected a Text file \"{expectedOutputFilename}\" to have been created; but none was found.");
        }

        [Test]
        public void CanRenderResultsIntoPdfFile()
        {
            // Arrange
            var rendererFactory = (this.provider ?? throw new InvalidOperationException()).GetRequiredService<IResultRendererFactory>();

            // Act
            string resultPath = TestResultRunFile(@"ResultRenderers/PDF/phototest");
            using (IResultRenderer renderer = rendererFactory.CreatePdfRenderer(resultPath, DataPath, false))
            {
                string examplePixPath = MakeAbsoluteTestFilePath("Ocr/phototest.tif");
                this.ProcessFileActAssertHelper(renderer, examplePixPath);
            }

            string expectedOutputFilename = Path.ChangeExtension(resultPath, "pdf");
            using (IResultRenderer renderer = rendererFactory.CreatePdfRenderer(resultPath, DataPath, false))
            {
                string examplePixPath = MakeAbsoluteTestFilePath("Ocr/phototest.tif");
                this.ProcessImageFileActAssertHelper(renderer, examplePixPath);
            }

            // Assert
            bool exists = File.Exists(expectedOutputFilename);
            Assert.That(exists, $"Expected a PDF file \"{expectedOutputFilename}\" to have been created; but none was found.");
        }

        [Test]
        public void CanRenderResultsIntoPdfFile1()
        {
            // Arrange
            var rendererFactory = (this.provider ?? throw new InvalidOperationException()).GetRequiredService<IResultRendererFactory>();

            string resultPath = TestResultRunFile(@"ResultRenderers/PDF/phototest");
            string examplePixPath = MakeAbsoluteTestFilePath("Ocr/phototest.tif");
            string expectedOutputFilename = Path.ChangeExtension(resultPath, "pdf");

            using (IResultRenderer renderer = rendererFactory.CreatePdfRenderer(resultPath, DataPath, false))
            {
                // Act
                this.ProcessImageFileActAssertHelper(renderer, examplePixPath);
            }

            // Assert
            Assert.That(File.Exists(expectedOutputFilename), $"Expected a PDF file \"{expectedOutputFilename}\" to have been created; but none was found.");
        }

        [Test]
        public void CanRenderMultiplePageDocumentToPdfFile()
        {
            // Arrange
            var rendererFactory = (this.provider ?? throw new InvalidOperationException()).GetRequiredService<IResultRendererFactory>();

            string resultPath = TestResultRunFile(@"ResultRenderers/PDF/multi-page");
            string expectedOutputFilename = Path.ChangeExtension(resultPath, "pdf");
            string examplePixPath = MakeAbsoluteTestFilePath("processing/multi-page.tif");

            // Act
            using (IResultRenderer renderer = rendererFactory.CreatePdfRenderer(resultPath, DataPath, false))
            {
                this.ProcessMultipageTiff(renderer, examplePixPath);
            }

            using (IResultRenderer renderer = rendererFactory.CreatePdfRenderer(resultPath, DataPath, false))
            {
                this.ProcessImageFileActAssertHelper(renderer, examplePixPath);
            }

            // Assert
            bool exists = File.Exists(expectedOutputFilename);
            Assert.That(exists, $"Expected a PDF file \"{expectedOutputFilename}\" to have been created; but none was found.");
        }

        [Test]
        public void CanRenderMultiplePageDocumentToPdfFile1()
        {
            // Arrange
            var rendererFactory = (this.provider ?? throw new InvalidOperationException()).GetRequiredService<IResultRendererFactory>();

            string resultPath = TestResultRunFile(@"ResultRenderers/PDF/multi-page");
            string examplePixPath = MakeAbsoluteTestFilePath("processing/multi-page.tif");
            string expectedOutputFilename = Path.ChangeExtension(resultPath, "pdf");

            // Act
            using (IResultRenderer renderer = rendererFactory.CreatePdfRenderer(resultPath, DataPath, false))
            {
                this.ProcessImageFileActAssertHelper(renderer, examplePixPath);
            }

            // Assert
            bool exists = File.Exists(expectedOutputFilename);
            Assert.That(exists, $"Expected a PDF file \"{expectedOutputFilename}\" to have been created; but none was found.");
        }

        [Test]
        public void CanRenderResultsIntoHOcrFile()
        {
            // Arrange
            var rendererFactory = (this.provider ?? throw new InvalidOperationException()).GetRequiredService<IResultRendererFactory>();

            string resultPath = TestResultRunFile(@"ResultRenderers/HOCR/phototest");
            string examplePixPath = MakeAbsoluteTestFilePath("Ocr/phototest.tif");
            string expectedOutputFilename = Path.ChangeExtension(resultPath, "hocr");

            // Act
            using (IResultRenderer renderer = rendererFactory.CreateHOcrRenderer(resultPath))
            {
                this.ProcessFileActAssertHelper(renderer, examplePixPath);
            }

            // Assert
            bool exists = File.Exists(expectedOutputFilename);
            Assert.That(exists, $"Expected a HOCR file \"{expectedOutputFilename}\" to have been created; but none was found.");
        }

        [Test]
        public void CanRenderResultsIntoUnlvFile()
        {
            // Arrange
            var rendererFactory = (this.provider ?? throw new InvalidOperationException()).GetRequiredService<IResultRendererFactory>();

            string resultPath = TestResultRunFile(@"ResultRenderers/UNLV/phototest");
            string examplePixPath = MakeAbsoluteTestFilePath("Ocr/phototest.tif");
            string expectedOutputFilename = Path.ChangeExtension(resultPath, "unlv");

            // Act
            using (IResultRenderer renderer = rendererFactory.CreateUnlvRenderer(resultPath))
            {
                this.ProcessFileActAssertHelper(renderer, examplePixPath);
            }

            // Assert
            bool exists = File.Exists(expectedOutputFilename);
            Assert.That(exists, $"Expected a Unlv file \"{expectedOutputFilename}\" to have been created; but none was found.");
        }

        [Test]
        public void CanRenderResultsIntoAltoFile()
        {
            // Arrange
            var rendererFactory = (this.provider ?? throw new InvalidOperationException()).GetRequiredService<IResultRendererFactory>();

            string resultPath = TestResultRunFile(@"ResultRenderers/Alto/phototest");
            string examplePixPath = MakeAbsoluteTestFilePath("Ocr/phototest.tif");
            string expectedOutputFilename = Path.ChangeExtension(resultPath, "xml");

            // Act
            using (IResultRenderer renderer = rendererFactory.CreateAltoRenderer(resultPath))
            {
                this.ProcessFileActAssertHelper(renderer, examplePixPath);
            }

            // Assert
            bool exists = File.Exists(expectedOutputFilename);
            Assert.That(exists, $"Expected an xml file \"{expectedOutputFilename}\" to have been created; but none was found.");
        }

        [Test]
        public void CanRenderResultsIntoTsvFile()
        {
            // Arrange
            var rendererFactory = (this.provider ?? throw new InvalidOperationException()).GetRequiredService<IResultRendererFactory>();

            string resultPath = TestResultRunFile(@"ResultRenderers/Tsv/phototest");
            string examplePixPath = MakeAbsoluteTestFilePath("Ocr/phototest.tif");
            string expectedOutputFilename = Path.ChangeExtension(resultPath, "tsv");

            // Act
            using (IResultRenderer renderer = rendererFactory.CreateTsvRenderer(resultPath))
            {
                this.ProcessFileActAssertHelper(renderer, examplePixPath);
            }

            // Assert
            bool exists = File.Exists(expectedOutputFilename);
            Assert.That(exists, $"Expected a Tsv file \"{expectedOutputFilename}\" to have been created; but none was found.");
        }

        [Test]
        public void CanRenderResultsIntoLSTMBoxFile()
        {
            // Arrange
            var rendererFactory = (this.provider ?? throw new InvalidOperationException()).GetRequiredService<IResultRendererFactory>();

            string resultPath = TestResultRunFile(@"ResultRenderers/LSTMBox/phototest");
            string examplePixPath = MakeAbsoluteTestFilePath("Ocr/phototest.tif");
            string expectedOutputFilename = Path.ChangeExtension(resultPath, "box");

            // Arrange
            using (IResultRenderer renderer = rendererFactory.CreateLSTMBoxRenderer(resultPath))
            {
                this.ProcessFileActAssertHelper(renderer, examplePixPath);
            }

            // Assert
            bool exists = File.Exists(expectedOutputFilename);
            Assert.That(exists, $"Expected a box file \"{expectedOutputFilename}\" to have been created; but none was found.");
        }

        [Test]
        public void CanRenderResultsIntoWordStrBoxFile()
        {
            // Arrange
            var rendererFactory = (this.provider ?? throw new InvalidOperationException()).GetRequiredService<IResultRendererFactory>();

            string resultPath = TestResultRunFile(@"ResultRenderers/WordStrBox/phototest");
            string examplePixPath = MakeAbsoluteTestFilePath("Ocr/phototest.tif");
            string expectedOutputFilename = Path.ChangeExtension(resultPath, "box");

            // Act
            using (IResultRenderer renderer = rendererFactory.CreateWordStrBoxRenderer(resultPath))
            {
                this.ProcessFileActAssertHelper(renderer, examplePixPath);
            }

            // Assert
            bool exists = File.Exists(expectedOutputFilename);
            Assert.That(exists, $"Expected a box file \"{expectedOutputFilename}\" to have been created; but none was found.");
        }

        [Test]
        public void CanRenderResultsIntoBoxFile()
        {
            // Arrange
            var rendererFactory = (this.provider ?? throw new InvalidOperationException()).GetRequiredService<IResultRendererFactory>();

            string resultPath = TestResultRunFile(@"ResultRenderers/Box/phototest");
            string examplePixPath = MakeAbsoluteTestFilePath("Ocr/phototest.tif");
            string expectedOutputFilename = Path.ChangeExtension(resultPath, "box");

            // Act
            using (IResultRenderer renderer = rendererFactory.CreateBoxRenderer(resultPath))
            {
                this.ProcessFileActAssertHelper(renderer, examplePixPath);
            }

            // Assert
            bool exists = File.Exists(expectedOutputFilename);
            Assert.That(exists, $"Expected a Box file \"{expectedOutputFilename}\" to have been created; but none was found.");
        }

        [Test]
        public void CanRenderResultsIntoMultipleOutputFormats()
        {
            // Arrange
            var rendererFactory = (this.provider ?? throw new InvalidOperationException()).GetRequiredService<IResultRendererFactory>();

            string examplePixPath = MakeAbsoluteTestFilePath("Ocr/phototest.tif");
            string resultPath = TestResultRunFile(@"ResultRenderers/PDF/phototest");
            var formats = new List<RenderedFormat> { RenderedFormat.HOCR, RenderedFormat.PDF_TEXTONLY, RenderedFormat.TEXT };

            string expectedPdfOutputFilename = Path.ChangeExtension(resultPath, "pdf");
            string expectedHocrOutputFilename = Path.ChangeExtension(resultPath, "hocr");
            string expectedTxtOutputFilename = Path.ChangeExtension(resultPath, "txt");

            IEnumerable<IResultRenderer> renderers = rendererFactory.CreateRenderers(resultPath, DataPath, formats);

            // Act
            using (var renderer = new AggregateResultRenderer(renderers))
            {
                this.ProcessFileActAssertHelper(renderer, examplePixPath);
            }

            // Assert
            Assert.That(File.Exists(expectedPdfOutputFilename), $"Expected a PDF file \"{expectedPdfOutputFilename}\" to have been created; but none was found.");
            Assert.That(File.Exists(expectedHocrOutputFilename), $"Expected a HOCR file \"{expectedHocrOutputFilename}\" to have been created; but none was found.");
            Assert.That(File.Exists(expectedTxtOutputFilename), $"Expected a TEXT file \"{expectedTxtOutputFilename}\" to have been created; but none was found.");
        }

        [Test]
        public void CanRenderMultiplePageDocumentIntoMultipleResultRenderers()
        {
            // Arrange
            var rendererFactory = (this.provider ?? throw new InvalidOperationException()).GetRequiredService<IResultRendererFactory>();

            string resultPath = TestResultRunFile(@"ResultRenderers/Aggregate/multi-page");
            string examplePixPath = MakeAbsoluteTestFilePath("processing/multi-page.tif");

            string expectedPdfOutputFilename = Path.ChangeExtension(resultPath, "pdf");
            IResultRenderer pdfRenderer = rendererFactory.CreatePdfRenderer(resultPath, DataPath, false);

            string expectedTxtOutputFilename = Path.ChangeExtension(resultPath, "txt");
            IResultRenderer textRenderer = rendererFactory.CreateTextRenderer(resultPath);

            // Act
            using (var renderer = new AggregateResultRenderer(pdfRenderer, textRenderer))
            {
                this.ProcessMultipageTiff(renderer, examplePixPath);
            }

            // Assert
            Assert.That(File.Exists(expectedPdfOutputFilename), $"Expected a PDF file \"{expectedPdfOutputFilename}\" to have been created; but none was found.");
            Assert.That(File.Exists(expectedTxtOutputFilename), $"Expected a Text file \"{expectedTxtOutputFilename}\" to have been created; but none was found.");
        }

        private void ProcessMultipageTiff(IResultRenderer renderer, string filename)
        {
            // Arrange
            var engineFactory = (this.provider ?? throw new InvalidOperationException()).GetRequiredService<TesseractEngineFactory>();
            TesseractEngineOptions engineOptions = new TesseractEngineOptionBuilder(DataPath).Build();

            using ITesseractEngine engine = engineFactory(engineOptions);
            var pixArrayFactory = this.provider.GetRequiredService<IPixArrayFactory>();

            string imageName = Path.GetFileNameWithoutExtension(filename);
            using PixArray pixA = pixArrayFactory.LoadMultiPageTiffFromFile(filename);

            // Act
            int expectedPageNumber = -1;
            using (renderer.BeginDocument(imageName))
            {
                Assert.AreEqual(renderer.PageNumber, expectedPageNumber);
                foreach (Pix pix in pixA)
                {
                    using Page? page = engine.Process(pix, imageName);
                    bool addedPage = renderer.AddPage(page);
                    expectedPageNumber++;

                    // Assert
                    Assert.That(addedPage, Is.True);
                    Assert.That(renderer.PageNumber, Is.EqualTo(expectedPageNumber));
                }
            }

            Assert.That(renderer.PageNumber, Is.EqualTo(expectedPageNumber));
        }

        private void ProcessFileActAssertHelper(IResultRenderer renderer, string filename)
        {
            var engineFactory = (this.provider ?? throw new InvalidOperationException()).GetRequiredService<TesseractEngineFactory>();
            var pixFactory = this.provider.GetRequiredService<IPixFactory>();

            TesseractEngineOptions engineOptions = new TesseractEngineOptionBuilder(DataPath).Build();
            using ITesseractEngine engine = engineFactory(engineOptions);

            string imageName = Path.GetFileNameWithoutExtension(filename);
            using Pix pix = pixFactory.LoadFromFile(filename);
            using (renderer.BeginDocument(imageName))
            {
                Assert.AreEqual(renderer.PageNumber, -1);
                using Page? page = engine.Process(pix, imageName);
                bool addedPage = renderer.AddPage(page);

                Assert.That(addedPage, Is.True);
                Assert.That(renderer.PageNumber, Is.EqualTo(0));
            }

            Assert.AreEqual(renderer.PageNumber, 0);
        }

        private void ProcessImageFileActAssertHelper(IResultRenderer renderer, string filename)
        {
            var engineFactory = (this.provider ?? throw new InvalidOperationException()).GetRequiredService<TesseractEngineFactory>();

            TesseractEngineOptions engineOptions = new TesseractEngineOptionBuilder(DataPath).Build();
            using ITesseractEngine engine = engineFactory(engineOptions);

            string imageName = Path.GetFileNameWithoutExtension(filename);
            using PixArray pixA = this.ReadImageFileIntoPixArray(filename);
            int expectedPageNumber = -1;
            using (renderer.BeginDocument(imageName))
            {
                Assert.AreEqual(renderer.PageNumber, expectedPageNumber);
                foreach (Pix pix in pixA)
                {
                    using Page? page = engine?.Process(pix, imageName);
                    bool addedPage = renderer.AddPage(page);
                    expectedPageNumber++;

                    Assert.That(addedPage, Is.True);
                    Assert.That(renderer.PageNumber, Is.EqualTo(expectedPageNumber));
                }
            }

            Assert.That(renderer.PageNumber, Is.EqualTo(expectedPageNumber));
        }

        private PixArray ReadImageFileIntoPixArray(string filename)
        {
            var pixFactory = (this.provider ?? throw new InvalidOperationException()).GetRequiredService<IPixFactory>();
            var pixArrayFactory = this.provider.GetRequiredService<IPixArrayFactory>();
            if (filename.ToLower().EndsWith(".tif") || filename.ToLower().EndsWith(".tiff")) return pixArrayFactory.LoadMultiPageTiffFromFile(filename);

            PixArray pa = pixArrayFactory.Create(0);
            Pix pix = pixFactory.LoadFromFile(filename);
            pa.Add(pix);
            return pa;
        }
    }
}