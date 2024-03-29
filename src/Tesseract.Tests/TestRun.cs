namespace Tesseract.Tests
{
    /// <summary>
    ///     Represents a test run.
    /// </summary>
    public class TestRun
    {
        public static readonly TestRun Current = new();

        private TestRun()
        {
            this.StartedAt = DateTime.Now;
        }

        public DateTime StartedAt { get; private set; }
    }
}