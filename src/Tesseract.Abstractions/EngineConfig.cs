namespace Tesseract.Abstractions
{
    public class EngineConfig
    {
        public EngineConfig(string dataPath, string language)
        {
            if (string.IsNullOrWhiteSpace(dataPath)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(dataPath));
            if (string.IsNullOrWhiteSpace(language)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(language));

            this.DataPath = dataPath;
            this.Language = language;
        }

        public string DataPath { get; }

        public string Language { get; }
    }
}