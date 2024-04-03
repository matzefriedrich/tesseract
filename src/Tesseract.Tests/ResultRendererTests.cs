namespace Tesseract.Tests
{
    using Abstractions;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using Rendering;
    using Rendering.Abstractions;
    using Document = Rendering.Document;

    [TestFixture]
    public class ResultRendererTests : TesseractTestBase
    {
        [SetUp]
        public void Init()
        {
            this.services.AddTesseract(new EngineOptionDefaults(DataPath));

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

            string resultPath = this.TestResultRunFile(@"ResultRenderers/Text/phototest");

            // Act
            using (IResultRenderer renderer = rendererFactory.CreateTextRenderer(resultPath))
            {
                string examplePixPath = MakeAbsoluteTestFilePath("Ocr/phototest.tif");
                this.ProcessFileActAssertHelper(renderer.AsDocumentRenderer(), examplePixPath);
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
            string resultPath = this.TestResultRunFile(@"ResultRenderers/PDF/phototest");
            using (IResultRenderer renderer = rendererFactory.CreatePdfRenderer(resultPath, DataPath, false))
            {
                string examplePixPath = MakeAbsoluteTestFilePath("Ocr/phototest.tif");
                this.ProcessFileActAssertHelper(renderer.AsDocumentRenderer(), examplePixPath);
            }

            string expectedOutputFilename = Path.ChangeExtension(resultPath, "pdf");
            using (IResultRenderer renderer = rendererFactory.CreatePdfRenderer(resultPath, DataPath, false))
            {
                string examplePixPath = MakeAbsoluteTestFilePath("Ocr/phototest.tif");
                this.ProcessImageFileActAssertHelper(renderer.AsDocumentRenderer(), examplePixPath);
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

            string resultPath = this.TestResultRunFile(@"ResultRenderers/PDF/phototest");
            string examplePixPath = MakeAbsoluteTestFilePath("Ocr/phototest.tif");
            string expectedOutputFilename = Path.ChangeExtension(resultPath, "pdf");

            using (IResultRenderer renderer = rendererFactory.CreatePdfRenderer(resultPath, DataPath, false))
            {
                // Act
                this.ProcessImageFileActAssertHelper(renderer.AsDocumentRenderer(), examplePixPath);
            }

            // Assert
            Assert.That(File.Exists(expectedOutputFilename), $"Expected a PDF file \"{expectedOutputFilename}\" to have been created; but none was found.");
        }

        [Test]
        public void CanRenderMultiplePageDocumentToPdfFile()
        {
            // Arrange
            var rendererFactory = (this.provider ?? throw new InvalidOperationException()).GetRequiredService<IResultRendererFactory>();

            string resultPath = this.TestResultRunFile(@"ResultRenderers/PDF/multi-page");
            string expectedOutputFilename = Path.ChangeExtension(resultPath, "pdf");
            string examplePixPath = MakeAbsoluteTestFilePath("processing/multi-page.tif");


            // Act
            using (IResultRenderer renderer = rendererFactory.CreatePdfRenderer(resultPath, DataPath, false))
            {
                this.ProcessMultipageTiff(renderer.AsDocumentRenderer(), examplePixPath);
            }

            using (IResultRenderer renderer = rendererFactory.CreatePdfRenderer(resultPath, DataPath, false))
            {
                this.ProcessImageFileActAssertHelper(renderer.AsDocumentRenderer(), examplePixPath);
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

            string resultPath = this.TestResultRunFile(@"ResultRenderers/PDF/multi-page");
            string examplePixPath = MakeAbsoluteTestFilePath("processing/multi-page.tif");
            string expectedOutputFilename = Path.ChangeExtension(resultPath, "pdf");

            // Act
            using (IResultRenderer renderer = rendererFactory.CreatePdfRenderer(resultPath, DataPath, false))
            {
                this.ProcessImageFileActAssertHelper(renderer.AsDocumentRenderer(), examplePixPath);
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

            string resultPath = this.TestResultRunFile(@"ResultRenderers/HOCR/phototest");
            string examplePixPath = MakeAbsoluteTestFilePath("Ocr/phototest.tif");
            string expectedOutputFilename = Path.ChangeExtension(resultPath, "hocr");

            // Act
            using (IResultRenderer renderer = rendererFactory.CreateHOcrRenderer(resultPath))
            {
                this.ProcessFileActAssertHelper(renderer.AsDocumentRenderer(), examplePixPath);
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

            string resultPath = this.TestResultRunFile(@"ResultRenderers/UNLV/phototest");
            string examplePixPath = MakeAbsoluteTestFilePath("Ocr/phototest.tif");
            string expectedOutputFilename = Path.ChangeExtension(resultPath, "unlv");

            // Act
            using (IResultRenderer renderer = rendererFactory.CreateUnlvRenderer(resultPath))
            {
                this.ProcessFileActAssertHelper(renderer.AsDocumentRenderer(), examplePixPath);
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

            string resultPath = this.TestResultRunFile(@"ResultRenderers/Alto/phototest");
            string examplePixPath = MakeAbsoluteTestFilePath("Ocr/phototest.tif");
            string expectedOutputFilename = Path.ChangeExtension(resultPath, "xml");

            // Act
            using (IResultRenderer renderer = rendererFactory.CreateAltoRenderer(resultPath))
            {
                this.ProcessFileActAssertHelper(renderer.AsDocumentRenderer(), examplePixPath);
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

            string resultPath = this.TestResultRunFile(@"ResultRenderers/Tsv/phototest");
            string examplePixPath = MakeAbsoluteTestFilePath("Ocr/phototest.tif");
            string expectedOutputFilename = Path.ChangeExtension(resultPath, "tsv");

            // Act
            using (IResultRenderer renderer = rendererFactory.CreateTsvRenderer(resultPath))
            {
                this.ProcessFileActAssertHelper(renderer.AsDocumentRenderer(), examplePixPath);
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

            string resultPath = this.TestResultRunFile(@"ResultRenderers/LSTMBox/phototest");
            string examplePixPath = MakeAbsoluteTestFilePath("Ocr/phototest.tif");
            string expectedOutputFilename = Path.ChangeExtension(resultPath, "box");

            // Arrange
            using (IResultRenderer renderer = rendererFactory.CreateLstmBoxRenderer(resultPath))
            {
                this.ProcessFileActAssertHelper(renderer.AsDocumentRenderer(), examplePixPath);
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

            string resultPath = this.TestResultRunFile(@"ResultRenderers/WordStrBox/phototest");
            string examplePixPath = MakeAbsoluteTestFilePath("Ocr/phototest.tif");
            string expectedOutputFilename = Path.ChangeExtension(resultPath, "box");

            // Act
            using (IResultRenderer renderer = rendererFactory.CreateWordStrBoxRenderer(resultPath))
            {
                this.ProcessFileActAssertHelper(renderer.AsDocumentRenderer(), examplePixPath);
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

            string resultPath = this.TestResultRunFile(@"ResultRenderers/Box/phototest");
            string examplePixPath = MakeAbsoluteTestFilePath("Ocr/phototest.tif");
            string expectedOutputFilename = Path.ChangeExtension(resultPath, "box");

            // Act
            using (IResultRenderer renderer = rendererFactory.CreateBoxRenderer(resultPath))
            {
                this.ProcessFileActAssertHelper(renderer.AsDocumentRenderer(), examplePixPath);
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
            string resultPath = this.TestResultRunFile(@"ResultRenderers/PDF/phototest");
            var formats = new List<RenderedFormat> { RenderedFormat.Hocr, RenderedFormat.PdfTextOnly, RenderedFormat.Text };

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

            string resultPath = this.TestResultRunFile(@"ResultRenderers/Aggregate/multi-page");
            string examplePixPath = MakeAbsoluteTestFilePath("processing/multi-page.tif");

            string expectedPdfOutputFilename = Path.ChangeExtension(resultPath, "pdf");
            IResultRenderer pdfRenderer = rendererFactory.CreatePdfRenderer(resultPath, DataPath, false);

            string expectedTxtOutputFilename = Path.ChangeExtension(resultPath, "txt");
            IResultRenderer textRenderer = rendererFactory.CreateTextRenderer(resultPath);

            // Act
            using (var renderer = new AggregateResultRenderer(new[] { pdfRenderer, textRenderer }))
            {
                this.ProcessMultipageTiff(renderer, examplePixPath);
            }

            // Assert
            Assert.That(File.Exists(expectedPdfOutputFilename), $"Expected a PDF file \"{expectedPdfOutputFilename}\" to have been created; but none was found.");
            Assert.That(File.Exists(expectedTxtOutputFilename), $"Expected a Text file \"{expectedTxtOutputFilename}\" to have been created; but none was found.");
        }

        private void ProcessMultipageTiff(AggregateResultRenderer renderer, string filename)
        {
            // Arrange
            var pageFactory = this.provider.GetRequiredService<IPageFactory>();
            var pixArrayFactory = this.provider.GetRequiredService<IPixArrayFactory>();

            string imageName = Path.GetFileNameWithoutExtension(filename);
            using PixArray pixA = pixArrayFactory.LoadMultiPageTiffFromFile(filename);

            // Act
            int expectedPageNumber = -1;
            using Document document = renderer.BeginDocument(imageName);

            Assert.AreEqual(document.NumPages, expectedPageNumber);
            foreach (Pix pix in pixA)
            {
                using Page? page = pageFactory.CreatePage(pix, builder => builder.WithInputName(imageName));
                bool addedPage = document.AddPage(page);
                expectedPageNumber++;

                // Assert
                Assert.That(addedPage, Is.True);
                Assert.That(document.NumPages, Is.EqualTo(expectedPageNumber));
            }

            Assert.That(document.NumPages, Is.EqualTo(expectedPageNumber));
        }

        private void ProcessFileActAssertHelper(AggregateResultRenderer renderer, string filename)
        {
            var pixFactory = this.provider.GetRequiredService<IPixFactory>();
            var pageFactory = this.provider.GetRequiredService<IPageFactory>();

            string imageName = Path.GetFileNameWithoutExtension(filename);
            using Pix pix = pixFactory.LoadFromFile(filename);
            using Document document = renderer.BeginDocument(imageName);
            Assert.AreEqual(document.NumPages, -1);
            
            using Page page = pageFactory.CreatePage(pix, builder => builder.WithInputName(imageName));
            bool addedPage = document.AddPage(page);

            // Assert
            Assert.That(addedPage, Is.True);
            Assert.That(document.NumPages, Is.EqualTo(0));
            Assert.AreEqual(document.NumPages, 0);
        }

        private void ProcessImageFileActAssertHelper(AggregateResultRenderer renderer, string filename)
        {
            var pageFactory = this.provider.GetRequiredService<IPageFactory>();
            
            string imageName = Path.GetFileNameWithoutExtension(filename);
            using PixArray pixA = this.ReadImageFileIntoPixArray(filename);

            int expectedPageNumber = -1;

            using Document document = renderer.BeginDocument(imageName);
            Assert.AreEqual(document.NumPages, expectedPageNumber);
            foreach (Pix pix in pixA)
            {
                using Page? page = pageFactory.CreatePage(pix, builder => builder.WithInputName(imageName));
                if (page == null) continue;
                bool addedPage = document.AddPage(page);
                expectedPageNumber++;

                // Assert
                Assert.That(addedPage, Is.True);
                Assert.That(document.NumPages, Is.EqualTo(expectedPageNumber));
            }

            Assert.That(document.NumPages, Is.EqualTo(expectedPageNumber));
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