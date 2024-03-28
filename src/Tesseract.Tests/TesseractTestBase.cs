namespace Tesseract.Tests
{
    using System.IO;
    using NUnit.Framework;

    public abstract class TesseractTestBase
    {
        /// <summary>
        ///     Determines how test differences are handled
        /// </summary>
        private static readonly ITestDifferenceHandler testDifferenceHandler = new FailTestDifferenceHandler();

        protected static string DataPath => AbsolutePath("tessdata");

        protected static TesseractEngine CreateEngine(string lang = "eng", EngineMode mode = EngineMode.Default)
        {
            string datapath = DataPath;
            return new TesseractEngine(datapath, lang, mode);
        }

        protected static string AbsolutePath(string relativePath)
        {
            return Path.Combine(TestContext.CurrentContext.WorkDirectory, relativePath);
        }

        #region File Helpers

        protected static string TestFilePath(string path)
        {
            string basePath = AbsolutePath("Data");

            return Path.GetFullPath(Path.Combine(basePath, path));
        }

        protected static string TestResultPath(string path)
        {
            // Assumes test executable is running in .\bin\$config\$platform
            string basePath = AbsolutePath("../../../../Tesseract.Tests/Results");

            return Path.GetFullPath(Path.Combine(basePath, path));
        }

        protected static string TestResultRunDirectory(string path)
        {
            string runPath = AbsolutePath(
                string.Format("Runs/{0:yyyyMMddTHHmmss}", TestRun.Current.StartedAt)
            );
            string testResultRunDirectory = Path.Combine(runPath, path);
            Directory.CreateDirectory(testResultRunDirectory);

            return testResultRunDirectory;
        }

        protected static string TestResultRunFile(string path)
        {
            string testRunDirectory = TestResultRunDirectory(Path.GetDirectoryName(path));
            string testFileName = Path.GetFileName(path);

            return Path.GetFullPath(Path.Combine(testRunDirectory, testFileName));
        }

        protected static Pix LoadTestPix(string filename)
        {
            string testFilename = TestFilePath(filename);
            return Pix.LoadFromFile(testFilename);
        }

        protected static void CheckResult(string resultFilename)
        {
            string actualResultFilename = TestResultRunFile(resultFilename);
            string expectedResultFilename = TestResultPath(resultFilename);

            testDifferenceHandler.Execute(actualResultFilename, expectedResultFilename);
        }

        #endregion File Helpers
    }
}