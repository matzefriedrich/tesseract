namespace Tesseract.Tests
{
    using NUnit.Framework;

    /// <summary>
    ///     Fails the test if the actual result file doesn't match the expected result (ignoring line ending type(s)).
    /// </summary>
    public class FailTestDifferenceHandler : ITestDifferenceHandler
    {
        public void Execute(string actualResultFilename, string expectedResultFilename)
        {
            if (File.Exists(expectedResultFilename))
            {
                string? actualResult = TestUtils.NormaliseNewLine(File.ReadAllText(actualResultFilename));
                string? expectedResult = TestUtils.NormaliseNewLine(File.ReadAllText(expectedResultFilename));
                if (expectedResult != actualResult) Assert.Fail("Expected results to be \"{0}\" but was \"{1}\".", expectedResultFilename, actualResultFilename);
            }
            else
            {
                File.Copy(actualResultFilename, expectedResultFilename);
                Console.WriteLine($"Expected result did not exist, the file \"{actualResultFilename}\" was used as a reference. Please check the file");
            }
        }
    }
}