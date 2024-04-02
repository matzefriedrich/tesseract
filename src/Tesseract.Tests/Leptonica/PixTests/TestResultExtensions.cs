namespace Tesseract.Tests.Leptonica.PixTests
{
    using Abstractions;

    internal static class TestResultExtensions
    {
        public static void SaveResult(this TesseractTestBase t, IPixFileWriter writer, Pix result, string resultsDirectory, string filename)
        {
            if (t == null) throw new ArgumentNullException(nameof(t));
            if (writer == null) throw new ArgumentNullException(nameof(writer));
            if (result == null) throw new ArgumentNullException(nameof(result));

            if (string.IsNullOrWhiteSpace(resultsDirectory)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(resultsDirectory));
            if (string.IsNullOrWhiteSpace(filename)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(filename));

            string runFilename = t.TestResultRunFile(Path.Combine(resultsDirectory, filename));
            writer.Save(result, runFilename);
        }
    }
}