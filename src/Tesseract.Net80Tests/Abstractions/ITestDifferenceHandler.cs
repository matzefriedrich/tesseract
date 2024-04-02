namespace Tesseract.Tests
{
    /// <summary>
    ///     Determines what action is taken when the test result doesn't match the expected (reference) result.
    /// </summary>
    public interface ITestDifferenceHandler
    {
        void Execute(string actualResultFilename, string expectedResultFilename);
    }
}