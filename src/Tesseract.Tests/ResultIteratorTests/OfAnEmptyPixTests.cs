namespace Tesseract.Tests.ResultIteratorTests
{
    using Abstractions;

    using Interop;

    using Microsoft.Extensions.DependencyInjection;

    using NUnit.Framework;

    [TestFixture]
    public class OfAnEmptyPixTests : TesseractTestBase
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

        [Theory]
        public void ResultIterator_GetText_returns_null_for_each_level(PageIteratorLevel level)
        {
            // Arrange
            var engineFactory = (this.provider ?? throw new InvalidOperationException()).GetRequiredService<TesseractEngineFactory>();
            TesseractEngineOptions engineOptions = new TesseractEngineOptionBuilder(DataPath).Build();

            var pixFactory = this.provider.GetRequiredService<IPixFactory>();

            string filename = MakeAbsoluteTestFilePath("Ocr/blank.tif");
            using Pix emptyPix = pixFactory.LoadFromFile(filename);
            using Page page = engineFactory(engineOptions).Process(emptyPix);

            using ResultIterator sut = page.GetIterator();

            // Act
            string actual = sut.GetText(level);

            // Assert
            Assert.That(actual, Is.Null);
        }
    }
}