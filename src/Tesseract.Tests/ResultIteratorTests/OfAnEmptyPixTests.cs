namespace Tesseract.Tests.ResultIteratorTests
{
    using Interop;

    using NUnit.Framework;

    [TestFixture]
    public class OfAnEmptyPixTests : TesseractTestBase
    {
        [SetUp]
        public void Init()
        {
            this.Engine = CreateEngine();
            this.EmptyPix = LoadTestPix("Ocr/blank.tif");
        }

        [TearDown]
        public void Dispose()
        {
            if (this.EmptyPix != null)
            {
                this.EmptyPix.Dispose();
                this.EmptyPix = null;
            }

            if (this.Engine != null)
            {
                this.Engine.Dispose();
                this.Engine = null;
            }
        }

        private TesseractEngine? Engine { get; set; }
        private Pix? EmptyPix { get; set; }

        [Theory]
        public void GetTextReturnNullForEachLevel(PageIteratorLevel level)
        {
            using Page page = this.Engine?.Process(this.EmptyPix) ?? throw new ArgumentNullException("this.Engine?.Process(this.EmptyPix)");
            using ResultIterator iter = page.GetIterator();
            Assert.That(iter.GetText(level), Is.Null);
        }
    }
}