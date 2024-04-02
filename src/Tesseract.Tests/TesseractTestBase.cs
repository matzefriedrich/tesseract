namespace Tesseract.Tests
{
    using NUnit.Framework;

    public abstract class TesseractTestBase
    {
        /// <summary>
        ///     Determines how test differences are handled
        /// </summary>
        private static readonly ITestDifferenceHandler testDifferenceHandler = new FailTestDifferenceHandler();

        protected static string DataPath => AbsolutePath("tessdata");

        protected static string AbsolutePath(string relativePath)
        {
            return Path.Combine(TestContext.CurrentContext.WorkDirectory, relativePath);
        }

        protected static string MakeAbsoluteTestFilePath(string path)
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
            string runPath = AbsolutePath($"Runs/{TestRun.Current.StartedAt:yyyyMMddTHHmmss}");
            string testResultRunDirectory = Path.Combine(runPath, path);
            Directory.CreateDirectory(testResultRunDirectory);

            return testResultRunDirectory;
        }

        protected internal string TestResultRunFile(string path)
        {
            string? directoryName = Path.GetDirectoryName(path) ?? throw new ArgumentNullException("Path.GetDirectoryName(path)");
            string testRunDirectory = TestResultRunDirectory(directoryName);
            string testFileName = Path.GetFileName(path);

            return Path.GetFullPath(Path.Combine(testRunDirectory, testFileName));
        }

        protected void CheckResult(string resultFilename)
        {
            string actualResultFilename = this.TestResultRunFile(resultFilename);
            string expectedResultFilename = TestResultPath(resultFilename);

            testDifferenceHandler.Execute(actualResultFilename, expectedResultFilename);
        }
    }
}