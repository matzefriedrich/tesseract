namespace Tesseract.Tests.ResultIteratorTests
{
    using Interop;
    using NUnit.Framework;

    [TestFixture]
    public class FontAttributesTests : TesseractTestBase
    {
        [SetUp]
        public void Init()
        {
            this.Engine = CreateEngine(mode: EngineMode.TesseractOnly);
            this.TestImage = LoadTestPix("Ocr/Fonts.tif");
        }

        [TearDown]
        public void Dispose()
        {
            if (this.TestImage != null)
            {
                this.TestImage.Dispose();
                this.TestImage = null;
            }

            if (this.Engine != null)
            {
                this.Engine.Dispose();
                this.Engine = null;
            }
        }

        private TesseractEngine Engine { get; set; }
        private Pix TestImage { get; set; }

        [Test]
        public void GetWordFontAttributesWorks()
        {
            using (Page page = this.Engine.Process(this.TestImage))
            using (ResultIterator iter = page.GetIterator())
            {
                // font attributes come in this order in the test image:
                // bold, italic, monospace, serif, smallcaps
                //
                // there is no test for underline because in 3.04 IsUnderlined is
                // hard-coded to "false".  See: https://github.com/tesseract-ocr/tesseract/blob/3.04/ccmain/ltrresultiterator.cpp#182
                // Note: GetWordFontAttributes returns null if font failed to be resolved (https://github.com/charlesw/tesseract/issues/607)

                FontAttributes fontAttrs = iter.GetWordFontAttributes();
                Assert.That(fontAttrs.FontInfo.IsBold, Is.True);
                Assert.That(iter.GetWordRecognitionLanguage(), Is.EqualTo("eng"));
                //Assert.That(iter.GetWordIsFromDictionary(), Is.True);
                iter.Next(PageIteratorLevel.TextLine, PageIteratorLevel.Word);

                fontAttrs = iter.GetWordFontAttributes();
                Assert.That(fontAttrs.FontInfo.IsItalic, Is.True);
                iter.Next(PageIteratorLevel.TextLine, PageIteratorLevel.Word);

                fontAttrs = iter.GetWordFontAttributes();
                Assert.That(fontAttrs.FontInfo.IsFixedPitch, Is.True);
                iter.Next(PageIteratorLevel.TextLine, PageIteratorLevel.Word);

                fontAttrs = iter.GetWordFontAttributes();
                Assert.That(fontAttrs.FontInfo.IsSerif, Is.True);
                iter.Next(PageIteratorLevel.TextLine, PageIteratorLevel.Word);

                fontAttrs = iter.GetWordFontAttributes();
                Assert.That(fontAttrs.IsSmallCaps, Is.True);
                iter.Next(PageIteratorLevel.TextLine, PageIteratorLevel.Word);

                Assert.That(iter.GetWordIsNumeric(), Is.True);

                iter.Next(PageIteratorLevel.TextLine, PageIteratorLevel.Word);
                iter.Next(PageIteratorLevel.Word, PageIteratorLevel.Symbol);

                Assert.That(iter.GetSymbolIsSuperscript(), Is.True);

                iter.Next(PageIteratorLevel.TextLine, PageIteratorLevel.Word);
                iter.Next(PageIteratorLevel.Word, PageIteratorLevel.Symbol);

                Assert.That(iter.GetSymbolIsSubscript(), Is.True);
            }
        }
    }
}