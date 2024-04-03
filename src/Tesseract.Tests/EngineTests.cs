namespace Tesseract.Tests
{
    using System.Drawing;
    using Abstractions;
    using Interop;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;

    [TestFixture]
    public class EngineTests : TesseractTestBase
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

        private const string TestImagePath = "Ocr/phototest.tif";
        private readonly ServiceCollection services = new();
        private ServiceProvider? provider;

        [Test]
        public void CanGetVersion()
        {
            // Arrange
            var factory = (this.provider ?? throw new InvalidOperationException()).GetRequiredService<ITesseractEngineFactory>();

            // Act
            using ITesseractEngine engine = factory.CreateEngine();

            // Assert
            const string expectedEngineVersion = "5.0.0";
            Assert.That(engine.Version, Does.StartWith(expectedEngineVersion));
        }

        [Test]
        public void CanParseMultiPageTiff()
        {
            // Arrange
            var pixArrayFactory = this.provider.GetRequiredService<IPixArrayFactory>();
            var pageFactory = this.provider.GetRequiredService<IPageFactory>();

            string filename = MakeAbsoluteTestFilePath("./processing/multi-page.tif");

            // Act
            using PixArray pixA = pixArrayFactory.LoadMultiPageTiffFromFile(filename);
            var i = 0;
            foreach (Pix pix in pixA)
            {
                using Page page = pageFactory.CreatePage(pix);
                string? text = page.GetText()?.Trim();

                var expectedText = $"Page {++i}";

                // Assert
                Assert.That(text, Is.EqualTo(expectedText));
            }
        }

        [Test]
        public void CanParseMultiPageTiffOneByOne()
        {
            // Arrange
            var pixFactory = this.provider.GetRequiredService<IPixFactory>();
            var pageFactory = this.provider.GetRequiredService<IPageFactory>();

            string filename = MakeAbsoluteTestFilePath("./processing/multi-page.tif");

            // Act
            var offset = 0;
            var i = 0;
            do
            {
                // read pages one at a time
                using Pix img = pixFactory.ReadFromMultiPageTiff(filename, ref offset);
                using Page page = pageFactory.CreatePage(img);
                string? text = page.GetText()?.Trim();
                var expectedText = $"Page {++i}";

                // Assert
                Assert.That(text, Is.EqualTo(expectedText));
            } while (offset != 0);
        }

        [Test]
        [TestCase(PageSegMode.SingleBlock, "This is a lot of 12 point text to test the\nocr code and see if it works on all types\nof file format.")]
        [TestCase(PageSegMode.SingleColumn, "This is a lot of 12 point text to test the")]
        [TestCase(PageSegMode.SingleLine, "This is a lot of 12 point text to test the")]
        [TestCase(PageSegMode.SingleWord, "This")]
        [TestCase(PageSegMode.SingleChar, "T")]
        [TestCase(PageSegMode.SingleBlockVertText, "A line of text", Ignore = "#490")]
        public void CanParseText_UsingMode(PageSegMode mode, string expectedText)
        {
            // Arrange
            var pixFactory = this.provider.GetRequiredService<IPixFactory>();
            var pageFactory = this.provider.GetRequiredService<IPageFactory>();

            var demoFilename = $"./Ocr/PSM_{mode}.png";
            using Pix pix = pixFactory.LoadFromFile(MakeAbsoluteTestFilePath(demoFilename));

            // Act
            using Page page = pageFactory.CreatePage(pix, builder => builder.WithAnalysisMode(mode));
            string? text = page.GetText()?.Trim();

            // Assert
            Assert.That(text, Is.EqualTo(expectedText));
        }

        [Test]
        public void CanParseText()
        {
            // Arrange
            var pixFactory = this.provider.GetRequiredService<IPixFactory>();
            var pageFactory = this.provider.GetRequiredService<IPageFactory>();

            string filename = MakeAbsoluteTestFilePath(TestImagePath);
            using Pix img = pixFactory.LoadFromFile(filename);
            using Page page = pageFactory.CreatePage(img);

            const string expectedText = "This is a lot of 12 point text to test the\nocr code and see if it works on all types\nof file format.\n\nThe quick brown dog jumped over the\nlazy fox. The quick brown dog jumped\nover the lazy fox. The quick brown dog\njumped over the lazy fox. The quick\nbrown dog jumped over the lazy fox.\n";

            // Act
            string? text = page.GetText();

            // Assert
            Assert.That(text, Is.EqualTo(expectedText));
        }

        [Test]
        [Ignore("See #594")]
        public void CanParseUznFile()
        {
            // Arrange
            var pixFactory = this.provider.GetRequiredService<IPixFactory>();
            var pageFactory = this.provider.GetRequiredService<IPageFactory>();

            string inputFilename = MakeAbsoluteTestFilePath(@"Ocr/uzn-test.png");
            using Pix img = pixFactory.LoadFromFile(inputFilename);

            const string expectedText = "This is another test\n";

            // Act
            using Page page = pageFactory.CreatePage(img, ConfigurePage);
            string? text = page.GetText();

            // Assert
            Assert.That(text, Is.EqualTo(expectedText));
            return;

            void ConfigurePage(PageBuilder builder)
            {
                builder
                    .WithInputName(inputFilename)
                    .WithAnalysisMode(PageSegMode.SingleLine);
            }
        }

        [Test]
        public void CanProcessBitmap()
        {
            // Arrange
            var pixConverter = this.provider.GetRequiredService<IPixConverter>();
            var pageFactory = this.provider.GetRequiredService<IPageFactory>();

            string testImgFilename = MakeAbsoluteTestFilePath("Ocr/phototest.tif");
            using var img = new Bitmap(testImgFilename);

            const string expectedText = "This is a lot of 12 point text to test the\nocr code and see if it works on all types\nof file format.\n\nThe quick brown dog jumped over the\nlazy fox. The quick brown dog jumped\nover the lazy fox. The quick brown dog\njumped over the lazy fox. The quick\nbrown dog jumped over the lazy fox.\n";

            // Act
            using Page page = pageFactory.CreatePageFromBitmap(pixConverter, img);
            string? text = page.GetText();

            // Assert
            Assert.That(text, Is.EqualTo(expectedText));
        }

        [Test]
        [Ignore("#489")]
        public void CanProcessSpecifiedRegionInImage()
        {
            // Arrange
            var pixFactory = this.provider.GetRequiredService<IPixFactory>();
            var pageFactory = this.provider.GetRequiredService<IPageFactory>();

            using Pix img = pixFactory.LoadFromFile(MakeAbsoluteTestFilePath(TestImagePath));
            Rect region = Rect.FromCoords(0, 0, img.Width, 188);

            //  Act
            using Page page = pageFactory.CreatePage(img, builder => builder.Region(region));
            string? text = page.GetText();

            const string expectedTextRegion1 = "This is a lot of 12 point text to test the\nocr code and see if it works on all types\nof file format.\n";

            // Assert
            Assert.That(text, Is.EqualTo(expectedTextRegion1));
        }

        [Test]
        [Ignore("#489")]
        public void CanProcessDifferentRegionsInSameImage()
        {
            // Arrange
            var pixFactory = this.provider.GetRequiredService<IPixFactory>();
            var pageFactory = this.provider.GetRequiredService<IPageFactory>();

            using Pix img = pixFactory.LoadFromFile(MakeAbsoluteTestFilePath(TestImagePath));
            using Page page = pageFactory.CreatePage(img, builder => builder.Region(Rect.FromCoords(0, 0, img.Width, 188)));
            string? region1Text = page.GetText();

            const string expectedTextRegion1 = "This is a lot of 12 point text to test the\ncor code and see if it works on all types\nof file format.\n";
            const string expectedTextRegion2 = "The quick brown dog jumped over the\nlazy fox. The quick brown dog jumped\nover the lazy fox. The quick brown dog\njumped over the lazy fox. The quick\nbrown dog jumped over the lazy fox.\n";

            // Act
            page.RegionOfInterest = Rect.FromCoords(0, 188, img.Width, img.Height);
            string? region2Text = page.GetText();

            // Assert
            Assert.That(region1Text, Is.EqualTo(expectedTextRegion1));
            Assert.That(region2Text, Is.EqualTo(expectedTextRegion2));
        }

        [Test]
        public void CanGetSegmentedRegions()
        {
            // Arrange
            var pixFactory = this.provider.GetRequiredService<IPixFactory>();
            var pageFactory = this.provider.GetRequiredService<IPageFactory>();

            const int expectedCount = 8; // number of text lines in test image

            string imgPath = MakeAbsoluteTestFilePath(TestImagePath);
            using Pix img = pixFactory.LoadFromFile(imgPath);

            // Act
            using Page page = pageFactory.CreatePage(img);
            List<Rectangle> boxes = page.GetSegmentedRegions(PageIteratorLevel.TextLine);

            for (var i = 0; i < boxes.Count; i++)
            {
                Rectangle box = boxes[i];
                Console.WriteLine("Box[{0}]: x={1}, y={2}, w={3}, h={4}", i, box.X, box.Y, box.Width, box.Height);
            }

            // Assert
            Assert.AreEqual(boxes.Count, expectedCount);
        }

        [Test]
        public void CanProcessEmptyPxUsingResultIterator()
        {
            // Arrange
            var pixFactory = this.provider.GetRequiredService<IPixFactory>();
            var pageFactory = this.provider.GetRequiredService<IPageFactory>();

            using Pix img = pixFactory.LoadFromFile(MakeAbsoluteTestFilePath("Ocr/empty.png"));
            using Page page = pageFactory.CreatePage(img);

            // Act
            string? actualResult = PageSerializer.Serialize(page, false);

            // Assert
            Assert.That(actualResult, Is.EqualTo(TestUtils.NormaliseNewLine(@"</word></line>
</para>
</block>
")));
        }

        [Test]
        public void CanProcessMultiplePixs()
        {
            // Arrange
            var pixFactory = this.provider.GetRequiredService<IPixFactory>();
            var pageFactory = this.provider.GetRequiredService<IPageFactory>();

            const string expectedText = "This is a lot of 12 point text to test the\nocr code and see if it works on all types\nof file format.\n\nThe quick brown dog jumped over the\nlazy fox. The quick brown dog jumped\nover the lazy fox. The quick brown dog\njumped over the lazy fox. The quick\nbrown dog jumped over the lazy fox.\n";

            // Act
            for (var i = 0; i < 3; i++)
            {
                using Pix img = pixFactory.LoadFromFile(MakeAbsoluteTestFilePath(TestImagePath));
                using Page page = pageFactory.CreatePage(img);
                string? text = page.GetText();

                // Assert
                Assert.That(text, Is.EqualTo(expectedText));
            }
        }

        [Test]
        public void CanProcessPixUsingResultIterator()
        {
            // Arrange
            var pixFactory = this.provider.GetRequiredService<IPixFactory>();
            var pageFactory = this.provider.GetRequiredService<IPageFactory>();

            const string resultPath = "EngineTests/CanProcessPixUsingResultIterator.txt";
            string actualResultPath = this.TestResultRunFile(resultPath);

            using Pix img = pixFactory.LoadFromFile(MakeAbsoluteTestFilePath(TestImagePath));
            using Page page = pageFactory.CreatePage(img);

            // Act
            string? pageString = PageSerializer.Serialize(page, false);
            File.WriteAllText(actualResultPath, pageString);

            // Assert
            this.CheckResult(resultPath);
        }

        // Test for [Issue #166](https://github.com/charlesw/tesseract/issues/166)
        [Test]
        public void CanProcessScaledBitmap()
        {
            // Arrange
            var converter = this.provider.GetRequiredService<IPixConverter>();
            var pageFactory = this.provider.GetRequiredService<IPageFactory>();

            string imagePath = MakeAbsoluteTestFilePath(TestImagePath);
            using Image img = Image.FromFile(imagePath);
            using var scaledImg = new Bitmap(img, new Size(img.Width * 2, img.Height * 2));

            const string expectedText = "This is a lot of 12 point text to test the\nocr code and see if it works on all types\nof file format.\n\nThe quick brown dog jumped over the\nlazy fox. The quick brown dog jumped\nover the lazy fox. The quick brown dog\njumped over the lazy fox. The quick\nbrown dog jumped over the lazy fox.";

            // Act
            using Page page = pageFactory.CreatePageFromBitmap(converter, scaledImg);
            string? text = page.GetText()?.Trim();

            // Assert
            Assert.That(text, Is.EqualTo(expectedText));
        }

        [Test]
        public void CanGenerateHOCROutput([Values(true, false)] bool useXHtml)
        {
            // Arrange
            var pixFactory = this.provider.GetRequiredService<IPixFactory>();
            var pageFactory = this.provider.GetRequiredService<IPageFactory>();

            var resultFilename = $"EngineTests/CanGenerateHOCROutput_{useXHtml}.txt";

            using Pix img = pixFactory.LoadFromFile(MakeAbsoluteTestFilePath(TestImagePath));
            using Page page = pageFactory.CreatePage(img);

            // Act
            string? hocrText = page.GetHocrText(1, useXHtml);
            File.WriteAllText(this.TestResultRunFile(resultFilename), hocrText);

            // Assert
            this.AssertXDocumentsAreEqual(resultFilename);
        }

        [Test]
        public void CanGenerateAltoOutput()
        {
            // Arrange
            var pixFactory = this.provider.GetRequiredService<IPixFactory>();
            var pageFactory = this.provider.GetRequiredService<IPageFactory>();

            const string resultFilename = "EngineTests/CanGenerateAltoOutput.txt";

            using Pix img = pixFactory.LoadFromFile(MakeAbsoluteTestFilePath(TestImagePath));
            using Page page = pageFactory.CreatePage(img);

            // Act
            string? altoText = page.GetAltoText(1);
            string? actualResult = TestUtils.NormaliseNewLine(altoText);
            File.WriteAllText(this.TestResultRunFile(resultFilename), actualResult);

            // Assert
            this.CheckResult(resultFilename);
        }

        [Test]
        public void CanGenerateTsvOutput()
        {
            // Arrange
            var pixFactory = this.provider.GetRequiredService<IPixFactory>();
            var pageFactory = this.provider.GetRequiredService<IPageFactory>();

            const string resultFilename = "EngineTests/CanGenerateTsvOutput.txt";

            using Pix img = pixFactory.LoadFromFile(MakeAbsoluteTestFilePath(TestImagePath));
            using Page page = pageFactory.CreatePage(img);

            // Act
            string? tsvText = page.GetTsvText(1);
            string? actualResult = TestUtils.NormaliseNewLine(tsvText);
            File.WriteAllText(this.TestResultRunFile(resultFilename), actualResult);

            // Assert
            this.CheckResult(resultFilename);
        }

        [Test]
        public void CanGenerateBoxOutput()
        {
            // Arrange
            var pixFactory = this.provider.GetRequiredService<IPixFactory>();
            var pageFactory = this.provider.GetRequiredService<IPageFactory>();

            const string resultFilename = "EngineTests/CanGenerateBoxOutput.txt";
            using Pix img = pixFactory.LoadFromFile(MakeAbsoluteTestFilePath(TestImagePath));
            using Page page = pageFactory.CreatePage(img);

            // Act
            string? boxText = page.GetBoxText(1);
            string? actualResult = TestUtils.NormaliseNewLine(boxText);
            File.WriteAllText(this.TestResultRunFile(resultFilename), actualResult);

            // Assert
            this.CheckResult(resultFilename);
        }

        [Test]
        public void CanGenerateLSTMBoxOutput()
        {
            // Arrange
            var pixFactory = this.provider.GetRequiredService<IPixFactory>();
            var pageFactory = this.provider.GetRequiredService<IPageFactory>();

            const string resultFilename = "EngineTests/CanGenerateLSTMBoxOutput.txt";

            using Pix img = pixFactory.LoadFromFile(MakeAbsoluteTestFilePath(TestImagePath));
            using Page page = pageFactory.CreatePage(img);

            // Act
            string? lstmBoxText = page.GetLstmBoxText(1);
            string? actualResult = TestUtils.NormaliseNewLine(lstmBoxText);
            File.WriteAllText(this.TestResultRunFile(resultFilename), actualResult);

            // Assert
            this.CheckResult(resultFilename);
        }

        [Test]
        public void CanGenerateWordStrBoxOutput()
        {
            // Arrange
            var pixFactory = this.provider.GetRequiredService<IPixFactory>();
            var pageFactory = this.provider.GetRequiredService<IPageFactory>();

            const string resultFilename = "EngineTests/CanGenerateWordStrBoxOutput.txt";

            using Pix img = pixFactory.LoadFromFile(MakeAbsoluteTestFilePath(TestImagePath));
            using Page page = pageFactory.CreatePage(img);

            // Act
            string? wordStrBoxText = page.GetWordStrBoxText(1);
            string? actualResult = TestUtils.NormaliseNewLine(wordStrBoxText);
            File.WriteAllText(this.TestResultRunFile(resultFilename), actualResult);

            // Assert
            this.CheckResult(resultFilename);
        }

        [Test]
        public void CanGenerateUNLVOutput()
        {
            // Arrange
            var pixFactory = this.provider.GetRequiredService<IPixFactory>();
            var pageFactory = this.provider.GetRequiredService<IPageFactory>();

            const string resultFilename = "EngineTests/CanGenerateUNLVOutput.txt";

            using Pix img = pixFactory.LoadFromFile(MakeAbsoluteTestFilePath(TestImagePath));
            using Page page = pageFactory.CreatePage(img);

            // Act
            string? unlvText = page.GetUnlvText();
            string? actualResult = TestUtils.NormaliseNewLine(unlvText);
            File.WriteAllText(this.TestResultRunFile(resultFilename), actualResult);

            // Assert
            this.CheckResult(resultFilename);
        }

        [Test]
        public void CanProcessPixUsingResultIteratorAndChoiceIterator()
        {
            // Arrange
            var pixFactory = this.provider.GetRequiredService<IPixFactory>();
            var pageFactory = this.provider.GetRequiredService<IPageFactory>();

            const string resultFilename = @"EngineTests/CanProcessPixUsingResultIteratorAndChoiceIterator.txt";

            using Pix img = pixFactory.LoadFromFile(MakeAbsoluteTestFilePath(TestImagePath));
            using Page page = pageFactory.CreatePage(img);

            // Act
            string? pageString = PageSerializer.Serialize(page, true);
            File.WriteAllText(this.TestResultRunFile(resultFilename), pageString);

            // Assert
            this.CheckResult(resultFilename);
        }

        [Test]
        public void Initialise_CanLoadConfigFile()
        {
            // Arrange
            const string configFile = "bazzar";

            var pixFactory = (this.provider ?? throw new InvalidOperationException()).GetRequiredService<IPixFactory>();
            using Pix img = pixFactory.LoadFromFile(MakeAbsoluteTestFilePath(TestImagePath));

            var pageFactory = this.provider.GetRequiredService<IPageFactory>();

            // Act
            using Page page = pageFactory.CreatePage(img, ConfigurePage);
            string? text = page.GetText();

            const string expectedText = "This is a lot of 12 point text to test the\nocr code and see if it works on all types\nof file format.\n\nThe quick brown dog jumped over the\nlazy fox. The quick brown dog jumped\nover the lazy fox. The quick brown dog\njumped over the lazy fox. The quick\nbrown dog jumped over the lazy fox.\n";

            // Assert
            Assert.That(text, Is.EqualTo(expectedText));
            return;

            void ConfigureEngine(EngineOptionBuilder optionBuilder)
            {
                optionBuilder.WithConfigFile(configFile);
            }

            void ConfigurePage(PageBuilder builder)
            {
                builder.WithEngineOptions(ConfigureEngine);
            }
        }

        [Test]
        public void TesseractEngine_TryGetStringVariable_reflects_variables_from_config_file()
        {
            // Arrange
            const string configFile = "bazzar";
            const string variableName = "user_words_suffix";
            const string expectedVariableValue = "user-words";

            var engineFactory = (this.provider ?? throw new InvalidOperationException()).GetRequiredService<ITesseractEngineFactory>();

            using ITesseractEngine engine = engineFactory.CreateEngine(builder => builder.WithConfigFile(configFile));

            // Act
            string? user_patterns_suffix;
            bool actual = engine.TryGetStringVariable(variableName, out user_patterns_suffix);

            // Assert
            Assert.IsTrue(actual);
            Assert.That(user_patterns_suffix, Is.EqualTo(expectedVariableValue));
        }

        [Test]
        public void Initialise_CanPassInitVariables()
        {
            // Arrange
            var engineFactory = (this.provider ?? throw new InvalidOperationException()).GetRequiredService<ITesseractEngineFactory>();

            var initVars = new Dictionary<string, object>
            {
                { "load_system_dawg", false }
            };

            //Act
            using ITesseractEngine engine = engineFactory.CreateEngine(builder => builder.WithOptions(initVars));

            // Assert
            if (!engine.TryGetBoolVariable("load_system_dawg", out bool loadSystemDawg)) Assert.Fail("Failed to get 'load_system_dawg'.");
            Assert.That(loadSystemDawg, Is.False);
        }

        [Test]
        [Ignore("Missing russian language data")]
        public void Initialise_Rus_ShouldStartEngine()
        {
            // Arrange
            var engineFactory = (this.provider ?? throw new InvalidOperationException()).GetRequiredService<ITesseractEngineFactory>();

            const string language = "rus";

            //Act
            using ITesseractEngine? engine = engineFactory.CreateEngine(builder => builder.Language(language));

            // Assert
            Assert.NotNull(engine);
        }

        [Test]
        public void Initialise_ShouldStartEngine([ValueSource("DataPaths")] string datapath)
        {
            // Arrange
            var engineFactory = (this.provider ?? throw new InvalidOperationException()).GetRequiredService<ITesseractEngineFactory>();

            // Act
            using ITesseractEngine engine = engineFactory.CreateEngine(builder => builder.OverrideDataPath(DataPath));

            // Assert
            Assert.IsNotNull(engine);
        }

        [Test]
        public void Initialise_ShouldThrowErrorIfDataPathNotCorrect()
        {
            // Arrange
            var engineFactory = (this.provider ?? throw new InvalidOperationException()).GetRequiredService<ITesseractEngineFactory>();
            const string invalidPath = "./this-path-does-not-exist";

            // Act, Assert
            Assert.That(() =>
            {
                using ITesseractEngine engine = engineFactory.CreateEngine(builder => builder.OverrideDataPath(invalidPath));
            }, Throws.InstanceOf(typeof(TesseractException)));
        }

        private static IEnumerable<string> DataPaths()
        {
            return new[]
            {
                AbsolutePath(@"./tessdata"),
                AbsolutePath(@"./tessdata/"),
                AbsolutePath(@".\tessdata\")
            };
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void CanSetBooleanVariable(bool variableValue)
        {
            // Arrange
            var engineFactory = (this.provider ?? throw new InvalidOperationException()).GetRequiredService<ITesseractEngineFactory>();

            using ITesseractEngine engine = engineFactory.CreateEngine();

            const string VariableName = "classify_enable_learning";

            // Act
            bool variableWasSet = engine.SetVariable(VariableName, variableValue);

            // Assert
            Assert.That(variableWasSet, Is.True, "Failed to set variable '{0}'.", VariableName);
            if (engine.TryGetBoolVariable(VariableName, out bool result))
                Assert.That(result, Is.EqualTo(variableValue));
            else
                Assert.Fail("Failed to retrieve value for '{0}'.", VariableName);
        }

        /// <summary>
        ///     As per Bug #52 setting 'classify_bln_numeric_mode' variable to '1' causes the engine to fail on processing.
        /// </summary>
        [Test]
        public void CanSetClassifyBlnNumericModeVariable()
        {
            // Arrange
            var pixFactory = (this.provider ?? throw new InvalidOperationException()).GetRequiredService<IPixFactory>();
            var pageFactory = this.provider.GetRequiredService<IPageFactory>();

            string filename = MakeAbsoluteTestFilePath("./processing/numbers.png");
            using Pix img = pixFactory.LoadFromFile(filename);

            const string expectedText = "1234567890\n";

            using Page page = pageFactory.CreatePage(img, ConfigurePage);

            // Act
            string? text = page.GetText();

            // Assert
            Assert.That(text, Is.EqualTo(expectedText));
            return;

            void ConfigurePage(PageBuilder builder)
            {
                builder.WithEngineOptions(ConfigureEngine);
            }

            void ConfigureEngine(EngineOptionBuilder builder)
            {
                builder.WithOption("classify_bln_numeric_mode", 1);
            }
        }

        [Test]
        [TestCase("edges_boxarea", 0.875)]
        [TestCase("edges_boxarea", 0.9)]
        [TestCase("edges_boxarea", -0.9)]
        public void CanSetDoubleVariable(string variableName, double variableValue)
        {
            // Arrange
            var engineFactory = (this.provider ?? throw new InvalidOperationException()).GetRequiredService<ITesseractEngineFactory>();
            using ITesseractEngine engine = engineFactory.CreateEngine();

            // Act
            bool variableWasSet = engine.SetVariable(variableName, variableValue);

            // Assert
            Assert.That(variableWasSet, Is.True, "Failed to set variable '{0}'.", variableName);
            if (engine.TryGetDoubleVariable(variableName, out double result))
                Assert.That(result, Is.EqualTo(variableValue));
            else
                Assert.Fail("Failed to retrieve value for '{0}'.", variableName);
        }

        [Test]
        [TestCase("edges_children_count_limit", 45)]
        [TestCase("edges_children_count_limit", 20)]
        [TestCase("textord_testregion_left", 20)]
        [TestCase("textord_testregion_left", -20)]
        public void CanSetIntegerVariable(string variableName, int variableValue)
        {
            // Arrange
            var engineFactory = (this.provider ?? throw new InvalidOperationException()).GetRequiredService<ITesseractEngineFactory>();
            using ITesseractEngine engine = engineFactory.CreateEngine();

            // Act
            bool variableWasSet = engine.SetVariable(variableName, variableValue);

            // Assert
            Assert.That(variableWasSet, Is.True, "Failed to set variable '{0}'.", variableName);
            if (engine.TryGetIntVariable(variableName, out int result))
                Assert.That(result, Is.EqualTo(variableValue));
            else
                Assert.Fail("Failed to retrieve value for '{0}'.", variableName);
        }

        [Test]
        [TestCase("tessedit_char_whitelist", "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ")]
        [TestCase("tessedit_char_whitelist", "")]
        [TestCase("tessedit_char_whitelist", "Test")]
        [TestCase("tessedit_char_whitelist", "chinese 漢字")] // Issue 68
        public void CanSetStringVariable(string variableName, string variableValue)
        {
            // Arrange
            var engineFactory = (this.provider ?? throw new InvalidOperationException()).GetRequiredService<ITesseractEngineFactory>();
            using ITesseractEngine engine = engineFactory.CreateEngine();

            // Act
            bool variableWasSet = engine.SetVariable(variableName, variableValue);

            // Assert
            Assert.That(variableWasSet, Is.True, "Failed to set variable '{0}'.", variableName);
            if (engine.TryGetStringVariable(variableName, out string? result))
                Assert.That(result, Is.EqualTo(variableValue));
            else
                Assert.Fail("Failed to retrieve value for '{0}'.", variableName);
        }

        [Test]
        public void CanGetStringVariableThatDoesNotExist()
        {
            // Arrange
            var engineFactory = (this.provider ?? throw new InvalidOperationException()).GetRequiredService<ITesseractEngineFactory>();
            using ITesseractEngine engine = engineFactory.CreateEngine();

            // Act
            bool success = engine.TryGetStringVariable("illegal-variable", out string? result);

            // Assert
            Assert.That(success, Is.False);
            Assert.That(result, Is.Null);
        }

        [Test]
        public void CanPrintVariables()
        {
            // Arrange
            var engineFactory = (this.provider ?? throw new InvalidOperationException()).GetRequiredService<ITesseractEngineFactory>();
            using ITesseractEngine engine = engineFactory.CreateEngine();

            const string ResultFilename = @"EngineTests/CanPrintVariables.txt";
            string actualResultsFilename = this.TestResultRunFile(ResultFilename);

            // Act
            bool actual = engine.TryPrintVariablesToFile(actualResultsFilename);

            // Assert
            Assert.That(actual, Is.True);

            // Load the expected results and verify that they match
            this.CheckResult(ResultFilename);
        }
    }
}