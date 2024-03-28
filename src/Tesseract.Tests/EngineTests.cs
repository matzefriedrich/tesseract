namespace Tesseract.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using Interop;
    using NUnit.Framework;

    [TestFixture]
    public class EngineTests : TesseractTestBase
    {
        private const string TestImagePath = "Ocr/phototest.tif";

        [Test]
        public void CanGetVersion()
        {
            using (TesseractEngine engine = CreateEngine())
            {
                Assert.That(engine.Version, Does.StartWith("5.0.0"));
            }
        }

        [Test]
        public void CanParseMultipageTif()
        {
            using (TesseractEngine engine = CreateEngine())
            {
                // load all pages at once
                using (PixArray pixA = PixArray.LoadMultiPageTiffFromFile(TestFilePath("./processing/multi-page.tif")))
                {
                    var i = 0;
                    foreach (Pix pix in pixA)
                        using (Page page = engine.Process(pix))
                        {
                            string text = page.GetText().Trim();

                            string expectedText = string.Format("Page {0}", ++i);
                            Assert.That(text, Is.EqualTo(expectedText));
                        }
                }
            }
        }

        [Test]
        public void CanParseMultipageTifOneByOne()
        {
            using (TesseractEngine engine = CreateEngine())
            {
                var offset = 0;
                var i = 0;
                do
                {
                    // read pages one at a time
                    using (var img = Pix.pixReadFromMultipageTiff(TestFilePath("./processing/multi-page.tif"), ref offset))
                    {
                        using (Page page = engine.Process(img))
                        {
                            string text = page.GetText().Trim();
                            string expectedText = string.Format("Page {0}", ++i);
                            Assert.That(text, Is.EqualTo(expectedText));
                        }
                    }
                } while (offset != 0);
            }
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
            using (TesseractEngine engine = CreateEngine(mode: EngineMode.TesseractAndLstm))
            {
                string demoFilename = string.Format("./Ocr/PSM_{0}.png", mode);
                using (Pix pix = LoadTestPix(demoFilename))
                {
                    using (Page page = engine.Process(pix, mode))
                    {
                        string text = page.GetText().Trim();

                        Assert.That(text, Is.EqualTo(expectedText));
                    }
                }
            }
        }

        [Test]
        public void CanParseText()
        {
            using (TesseractEngine engine = CreateEngine())
            {
                using (Pix img = LoadTestPix(TestImagePath))
                {
                    using (Page page = engine.Process(img))
                    {
                        string text = page.GetText();

                        const string expectedText =
                            "This is a lot of 12 point text to test the\nocr code and see if it works on all types\nof file format.\n\nThe quick brown dog jumped over the\nlazy fox. The quick brown dog jumped\nover the lazy fox. The quick brown dog\njumped over the lazy fox. The quick\nbrown dog jumped over the lazy fox.\n";

                        Assert.That(text, Is.EqualTo(expectedText));
                    }
                }
            }
        }

        [Test]
        [Ignore("See #594")]
        public void CanParseUznFile()
        {
            using (TesseractEngine engine = CreateEngine())
            {
                string inputFilename = TestFilePath(@"Ocr/uzn-test.png");
                using (Pix img = Pix.LoadFromFile(inputFilename))
                {
                    using (Page page = engine.Process(img, inputFilename, PageSegMode.SingleLine))
                    {
                        string text = page.GetText();

                        const string expectedText =
                            "This is another test\n";

                        Assert.That(text, Is.EqualTo(expectedText));
                    }
                }
            }
        }

#if NETFULL
        [Test]
        public void CanProcessBitmap()
        {
            using (TesseractEngine engine = CreateEngine())
            {
                string testImgFilename = TestFilePath(@"Ocr/phototest.tif");
                using (var img = new Bitmap(testImgFilename))
                {
                    using (Page page = engine.Process(img))
                    {
                        string text = page.GetText();

                        const string expectedText =
                            "This is a lot of 12 point text to test the\nocr code and see if it works on all types\nof file format.\n\nThe quick brown dog jumped over the\nlazy fox. The quick brown dog jumped\nover the lazy fox. The quick brown dog\njumped over the lazy fox. The quick\nbrown dog jumped over the lazy fox.\n";

                        Assert.That(text, Is.EqualTo(expectedText));
                    }
                }
            }
        }
#endif
        [Test]
        [Ignore("#489")]
        public void CanProcessSpecifiedRegionInImage()
        {
            using (TesseractEngine engine = CreateEngine(mode: EngineMode.LstmOnly))
            {
                using (Pix img = LoadTestPix(TestImagePath))
                {
                    using (Page page = engine.Process(img, Rect.FromCoords(0, 0, img.Width, 188)))
                    {
                        string region1Text = page.GetText();

                        const string expectedTextRegion1 =
                            "This is a lot of 12 point text to test the\nocr code and see if it works on all types\nof file format.\n";

                        Assert.That(region1Text, Is.EqualTo(expectedTextRegion1));
                    }
                }
            }
        }

        [Test]
        [Ignore("#489")]
        public void CanProcessDifferentRegionsInSameImage()
        {
            using (TesseractEngine engine = CreateEngine())
            {
                using (Pix img = LoadTestPix(TestImagePath))
                {
                    using (Page page = engine.Process(img, Rect.FromCoords(0, 0, img.Width, 188)))
                    {
                        string region1Text = page.GetText();

                        const string expectedTextRegion1 =
                            "This is a lot of 12 point text to test the\ncor code and see if it works on all types\nof file format.\n";

                        Assert.That(region1Text, Is.EqualTo(expectedTextRegion1));

                        page.RegionOfInterest = Rect.FromCoords(0, 188, img.Width, img.Height);

                        string region2Text = page.GetText();
                        const string expectedTextRegion2 =
                            "The quick brown dog jumped over the\nlazy fox. The quick brown dog jumped\nover the lazy fox. The quick brown dog\njumped over the lazy fox. The quick\nbrown dog jumped over the lazy fox.\n";

                        Assert.That(region2Text, Is.EqualTo(expectedTextRegion2));
                    }
                }
            }
        }

        [Test]
        public void CanGetSegmentedRegions()
        {
            var expectedCount = 8; // number of text lines in test image

            using (TesseractEngine engine = CreateEngine())
            {
                string imgPath = TestFilePath(TestImagePath);
                using (Pix img = Pix.LoadFromFile(imgPath))
                {
                    using (Page page = engine.Process(img))
                    {
                        List<Rectangle> boxes = page.GetSegmentedRegions(PageIteratorLevel.TextLine);

                        for (var i = 0; i < boxes.Count; i++)
                        {
                            Rectangle box = boxes[i];
                            Console.WriteLine("Box[{0}]: x={1}, y={2}, w={3}, h={4}", i, box.X, box.Y, box.Width, box.Height);
                        }

                        Assert.AreEqual(boxes.Count, expectedCount);
                    }
                }
            }
        }

        [Test]
        public void CanProcessEmptyPxUsingResultIterator()
        {
            string actualResult;
            using (TesseractEngine engine = CreateEngine())
            {
                using (Pix img = LoadTestPix("Ocr/empty.png"))
                {
                    using (Page page = engine.Process(img))
                    {
                        actualResult = PageSerializer.Serialize(page, false);
                    }
                }
            }

            Assert.That(actualResult, Is.EqualTo(
                TestUtils.NormaliseNewLine(@"</word></line>
</para>
</block>
")));
        }

        [Test]
        public void CanProcessMultiplePixs()
        {
            using (TesseractEngine engine = CreateEngine())
            {
                for (var i = 0; i < 3; i++)
                    using (Pix img = LoadTestPix(TestImagePath))
                    {
                        using (Page page = engine.Process(img))
                        {
                            string text = page.GetText();

                            const string expectedText =
                                "This is a lot of 12 point text to test the\nocr code and see if it works on all types\nof file format.\n\nThe quick brown dog jumped over the\nlazy fox. The quick brown dog jumped\nover the lazy fox. The quick brown dog\njumped over the lazy fox. The quick\nbrown dog jumped over the lazy fox.\n";

                            Assert.That(text, Is.EqualTo(expectedText));
                        }
                    }
            }
        }

        [Test]
        public void CanProcessPixUsingResultIterator()
        {
            const string ResultPath = @"EngineTests/CanProcessPixUsingResultIterator.txt";
            string actualResultPath = TestResultRunFile(ResultPath);

            using (TesseractEngine engine = CreateEngine())
            {
                using (Pix img = LoadTestPix(TestImagePath))
                {
                    using (Page page = engine.Process(img))
                    {
                        string pageString = PageSerializer.Serialize(page, false);
                        File.WriteAllText(actualResultPath, pageString);
                    }
                }
            }

            CheckResult(ResultPath);
        }

#if NETFULL
        // Test for [Issue #166](https://github.com/charlesw/tesseract/issues/166)
        [Test]
        public void CanProcessScaledBitmap()
        {
            using (TesseractEngine engine = CreateEngine())
            {
                string imagePath = TestFilePath(TestImagePath);
                using (Image img = Image.FromFile(imagePath))
                {
                    using (var scaledImg = new Bitmap(img, new Size(img.Width * 2, img.Height * 2)))
                    {
                        using (Page page = engine.Process(scaledImg))
                        {
                            string text = page.GetText().Trim();

                            const string expectedText =
                                "This is a lot of 12 point text to test the\nocr code and see if it works on all types\nof file format.\n\nThe quick brown dog jumped over the\nlazy fox. The quick brown dog jumped\nover the lazy fox. The quick brown dog\njumped over the lazy fox. The quick\nbrown dog jumped over the lazy fox.";

                            Assert.That(text, Is.EqualTo(expectedText));
                        }
                    }
                }
            }
        }
#endif

        [Test]
        public void CanGenerateHOCROutput(
            [Values(true, false)] bool useXHtml)
        {
            string resultFilename = string.Format("EngineTests/CanGenerateHOCROutput_{0}.txt", useXHtml);

            using (TesseractEngine engine = CreateEngine())
            {
                using (Pix img = LoadTestPix(TestImagePath))
                {
                    using (Page page = engine.Process(img))
                    {
                        string actualResult = TestUtils.NormaliseNewLine(page.GetHOCRText(1, useXHtml));
                        File.WriteAllText(TestResultRunFile(resultFilename), actualResult);
                    }
                }
            }

            CheckResult(resultFilename);
        }

        [Test]
        public void CanGenerateAltoOutput()
        {
            var resultFilename = "EngineTests/CanGenerateAltoOutput.txt";

            using (TesseractEngine engine = CreateEngine())
            {
                using (Pix img = LoadTestPix(TestImagePath))
                {
                    using (Page page = engine.Process(img))
                    {
                        string actualResult = TestUtils.NormaliseNewLine(page.GetAltoText(1));
                        File.WriteAllText(TestResultRunFile(resultFilename), actualResult);
                    }
                }
            }

            CheckResult(resultFilename);
        }

        [Test]
        public void CanGenerateTsvOutput()
        {
            var resultFilename = "EngineTests/CanGenerateTsvOutput.txt";

            using (TesseractEngine engine = CreateEngine())
            {
                using (Pix img = LoadTestPix(TestImagePath))
                {
                    using (Page page = engine.Process(img))
                    {
                        string actualResult = TestUtils.NormaliseNewLine(page.GetTsvText(1));
                        File.WriteAllText(TestResultRunFile(resultFilename), actualResult);
                    }
                }
            }

            CheckResult(resultFilename);
        }

        [Test]
        public void CanGenerateBoxOutput()
        {
            var resultFilename = "EngineTests/CanGenerateBoxOutput.txt";
            using (TesseractEngine engine = CreateEngine())
            {
                using (Pix img = LoadTestPix(TestImagePath))
                {
                    using (Page page = engine.Process(img))
                    {
                        string actualResult = TestUtils.NormaliseNewLine(page.GetBoxText(1));
                        File.WriteAllText(TestResultRunFile(resultFilename), actualResult);
                    }
                }
            }

            CheckResult(resultFilename);
        }

        [Test]
        public void CanGenerateLSTMBoxOutput()
        {
            var resultFilename = "EngineTests/CanGenerateLSTMBoxOutput.txt";

            using (TesseractEngine engine = CreateEngine())
            {
                using (Pix img = LoadTestPix(TestImagePath))
                {
                    using (Page page = engine.Process(img))
                    {
                        string actualResult = TestUtils.NormaliseNewLine(page.GetLSTMBoxText(1));
                        File.WriteAllText(TestResultRunFile(resultFilename), actualResult);
                    }
                }
            }

            CheckResult(resultFilename);
        }

        [Test]
        public void CanGenerateWordStrBoxOutput()
        {
            var resultFilename = "EngineTests/CanGenerateWordStrBoxOutput.txt";

            using (TesseractEngine engine = CreateEngine())
            {
                using (Pix img = LoadTestPix(TestImagePath))
                {
                    using (Page page = engine.Process(img))
                    {
                        string actualResult = TestUtils.NormaliseNewLine(page.GetWordStrBoxText(1));
                        File.WriteAllText(TestResultRunFile(resultFilename), actualResult);
                    }
                }
            }

            CheckResult(resultFilename);
        }

        [Test]
        public void CanGenerateUNLVOutput()
        {
            var resultFilename = "EngineTests/CanGenerateUNLVOutput.txt";

            using (TesseractEngine engine = CreateEngine())
            {
                using (Pix img = LoadTestPix(TestImagePath))
                {
                    using (Page page = engine.Process(img))
                    {
                        string actualResult = TestUtils.NormaliseNewLine(page.GetUNLVText());
                        File.WriteAllText(TestResultRunFile(resultFilename), actualResult);
                    }
                }
            }

            CheckResult(resultFilename);
        }

        [Test]
        public void CanProcessPixUsingResultIteratorAndChoiceIterator()
        {
            const string resultFilename = @"EngineTests/CanProcessPixUsingResultIteratorAndChoiceIterator.txt";

            using (TesseractEngine engine = CreateEngine())
            {
                using (Pix img = LoadTestPix(TestImagePath))
                {
                    using (Page page = engine.Process(img))
                    {
                        string pageString = PageSerializer.Serialize(page, true);
                        File.WriteAllText(TestResultRunFile(resultFilename), pageString);
                    }
                }
            }

            CheckResult(resultFilename);
        }

        [Test]
        public void Initialise_CanLoadConfigFile()
        {
            using (var engine = new TesseractEngine(DataPath, "eng", EngineMode.Default, "bazzar"))
            {
                // verify that the config file was loaded
                string user_patterns_suffix;
                if (engine.TryGetStringVariable("user_words_suffix", out user_patterns_suffix))
                    Assert.That(user_patterns_suffix, Is.EqualTo("user-words"));
                else
                    Assert.Fail("Failed to retrieve value for 'user_words_suffix'.");

                using (Pix img = LoadTestPix(TestImagePath))
                {
                    using (Page page = engine.Process(img))
                    {
                        string text = page.GetText();

                        const string expectedText =
                            "This is a lot of 12 point text to test the\nocr code and see if it works on all types\nof file format.\n\nThe quick brown dog jumped over the\nlazy fox. The quick brown dog jumped\nover the lazy fox. The quick brown dog\njumped over the lazy fox. The quick\nbrown dog jumped over the lazy fox.\n";
                        Assert.That(text, Is.EqualTo(expectedText));
                    }
                }
            }
        }

        [Test]
        public void Initialise_CanPassInitVariables()
        {
            var initVars = new Dictionary<string, object>
            {
                { "load_system_dawg", false }
            };
            using (var engine = new TesseractEngine(DataPath, "eng", EngineMode.Default, Enumerable.Empty<string>(), initVars, false))
            {
                bool loadSystemDawg;
                if (!engine.TryGetBoolVariable("load_system_dawg", out loadSystemDawg)) Assert.Fail("Failed to get 'load_system_dawg'.");
                Assert.That(loadSystemDawg, Is.False);
            }
        }

        [Test]
        [Ignore("Missing russian language data")]
        public void Initialise_Rus_ShouldStartEngine()
        {
            using (var engine = new TesseractEngine(DataPath, "rus", EngineMode.Default))
            {
            }
        }

        [Test]
        public void Initialise_ShouldStartEngine(
            [ValueSource("DataPaths")] string datapath)
        {
            using (var engine = new TesseractEngine(datapath, "eng", EngineMode.Default))
            {
            }
        }

        [Test]
        public void Initialise_ShouldThrowErrorIfDatapathNotCorrect()
        {
            Assert.That(() =>
            {
                using (var engine = new TesseractEngine(AbsolutePath(@"./IDontExist"), "eng", EngineMode.Default))
                {
                }
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
            const string VariableName = "classify_enable_learning";
            using (TesseractEngine engine = CreateEngine())
            {
                bool variableWasSet = engine.SetVariable(VariableName, variableValue);
                Assert.That(variableWasSet, Is.True, "Failed to set variable '{0}'.", VariableName);
                bool result;
                if (engine.TryGetBoolVariable(VariableName, out result))
                    Assert.That(result, Is.EqualTo(variableValue));
                else
                    Assert.Fail("Failed to retrieve value for '{0}'.", VariableName);
            }
        }

        /// <summary>
        ///     As per Bug #52 setting 'classify_bln_numeric_mode' variable to '1' causes the engine to fail on processing.
        /// </summary>
        [Test]
        public void CanSetClassifyBlnNumericModeVariable()
        {
            using (TesseractEngine engine = CreateEngine())
            {
                engine.SetVariable("classify_bln_numeric_mode", 1);

                using (Pix img = Pix.LoadFromFile(TestFilePath("./processing/numbers.png")))
                {
                    using (Page page = engine.Process(img))
                    {
                        string text = page.GetText();

                        const string expectedText = "1234567890\n";

                        Assert.That(text, Is.EqualTo(expectedText));
                    }
                }
            }
        }

        [Test]
        [TestCase("edges_boxarea", 0.875)]
        [TestCase("edges_boxarea", 0.9)]
        [TestCase("edges_boxarea", -0.9)]
        public void CanSetDoubleVariable(string variableName, double variableValue)
        {
            using (TesseractEngine engine = CreateEngine())
            {
                bool variableWasSet = engine.SetVariable(variableName, variableValue);
                Assert.That(variableWasSet, Is.True, "Failed to set variable '{0}'.", variableName);
                double result;
                if (engine.TryGetDoubleVariable(variableName, out result))
                    Assert.That(result, Is.EqualTo(variableValue));
                else
                    Assert.Fail("Failed to retrieve value for '{0}'.", variableName);
            }
        }

        [Test]
        [TestCase("edges_children_count_limit", 45)]
        [TestCase("edges_children_count_limit", 20)]
        [TestCase("textord_testregion_left", 20)]
        [TestCase("textord_testregion_left", -20)]
        public void CanSetIntegerVariable(string variableName, int variableValue)
        {
            using (TesseractEngine engine = CreateEngine())
            {
                bool variableWasSet = engine.SetVariable(variableName, variableValue);
                Assert.That(variableWasSet, Is.True, "Failed to set variable '{0}'.", variableName);
                int result;
                if (engine.TryGetIntVariable(variableName, out result))
                    Assert.That(result, Is.EqualTo(variableValue));
                else
                    Assert.Fail("Failed to retrieve value for '{0}'.", variableName);
            }
        }

        [Test]
        [TestCase("tessedit_char_whitelist", "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ")]
        [TestCase("tessedit_char_whitelist", "")]
        [TestCase("tessedit_char_whitelist", "Test")]
        [TestCase("tessedit_char_whitelist", "chinese 漢字")] // Issue 68
        public void CanSetStringVariable(string variableName, string variableValue)
        {
            using (TesseractEngine engine = CreateEngine())
            {
                bool variableWasSet = engine.SetVariable(variableName, variableValue);
                Assert.That(variableWasSet, Is.True, "Failed to set variable '{0}'.", variableName);
                string result;
                if (engine.TryGetStringVariable(variableName, out result))
                    Assert.That(result, Is.EqualTo(variableValue));
                else
                    Assert.Fail("Failed to retrieve value for '{0}'.", variableName);
            }
        }

        [Test]
        public void CanGetStringVariableThatDoesNotExist()
        {
            using (TesseractEngine engine = CreateEngine())
            {
                string result;
                bool success = engine.TryGetStringVariable("illegal-variable", out result);
                Assert.That(success, Is.False);
                Assert.That(result, Is.Null);
            }
        }

        [Test]
        public void CanPrintVariables()
        {
            const string ResultFilename = @"EngineTests/CanPrintVariables.txt";
            using (TesseractEngine engine = CreateEngine())
            {
                string actualResultsFilename = TestResultRunFile(ResultFilename);
                Assert.That(engine.TryPrintVariablesToFile(actualResultsFilename), Is.True);

                // Load the expected results and verify that they match
                CheckResult(ResultFilename);
            }
        }
    }
}