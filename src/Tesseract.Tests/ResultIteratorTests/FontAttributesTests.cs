namespace Tesseract.Tests.ResultIteratorTests
{
    using Abstractions;
    using Interop;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;

    [TestFixture]
    public class FontAttributesTests : TesseractTestBase
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
        public void GetWordFontAttributesWorks()
        {
            // Arrange
            var engineFactory = (this.provider ?? throw new InvalidOperationException()).GetRequiredService<TesseractEngineFactory>();
            var pixFactory = this.provider.GetRequiredService<IPixFactory>();

            using ITesseractEngine engine = engineFactory(new TesseractEngineOptionBuilder(DataPath, mode: EngineMode.TesseractOnly).Build());
            using Pix testImage = pixFactory.LoadFromFile(MakeAbsoluteTestFilePath("Ocr/Fonts.tif"));

            using Page page = engine.Process(testImage) ?? throw new ArgumentNullException("this.Engine?.Process(this.TestImage)");
            using ResultIterator iter = page.GetIterator();
            // font attributes come in this order in the test image:
            // bold, italic, monospace, serif, smallcaps
            //
            // there is no test for underline because in 3.04 IsUnderlined is
            // hard-coded to "false".  See: https://github.com/tesseract-ocr/tesseract/blob/3.04/ccmain/ltrresultiterator.cpp#182
            // Note: GetWordFontAttributes returns null if font failed to be resolved (https://github.com/charlesw/tesseract/issues/607)

            FontAttributes fontAttrs = iter.GetWordFontAttributes()!;
            Assert.NotNull(fontAttrs);
            Assert.That(fontAttrs.FontInfo.IsBold, Is.True);
            Assert.That(iter.GetWordRecognitionLanguage(), Is.EqualTo("eng"));
            //Assert.That(iter.GetWordIsFromDictionary(), Is.True);
            iter.Next(PageIteratorLevel.TextLine, PageIteratorLevel.Word);

            fontAttrs = iter.GetWordFontAttributes()!;
            Assert.That(fontAttrs.FontInfo.IsItalic, Is.True);
            iter.Next(PageIteratorLevel.TextLine, PageIteratorLevel.Word);

            fontAttrs = iter.GetWordFontAttributes()!;
            Assert.That(fontAttrs.FontInfo.IsFixedPitch, Is.True);
            iter.Next(PageIteratorLevel.TextLine, PageIteratorLevel.Word);

            fontAttrs = iter.GetWordFontAttributes()!;
            Assert.That(fontAttrs.FontInfo.IsSerif, Is.True);
            iter.Next(PageIteratorLevel.TextLine, PageIteratorLevel.Word);

            fontAttrs = iter.GetWordFontAttributes()!;
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