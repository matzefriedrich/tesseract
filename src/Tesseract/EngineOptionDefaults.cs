namespace Tesseract
{
    using System;
    using System.ComponentModel;
    using Abstractions;

    public sealed class EngineOptionDefaults
    {
        public string DataPath { get; }
        public string Language { get; }
        public EngineMode Mode { get; }

        public EngineOptionDefaults(string dataPath, string language = "eng", EngineMode mode = EngineMode.Default)
        {
            if (string.IsNullOrWhiteSpace(dataPath)) throw new ArgumentException(Resources.Resources.Value_cannot_be_null_or_whitespace, nameof(dataPath));
            if (string.IsNullOrWhiteSpace(language)) throw new ArgumentException(Resources.Resources.Value_cannot_be_null_or_whitespace, nameof(language));
            if (!Enum.IsDefined(typeof(EngineMode), mode)) throw new InvalidEnumArgumentException(nameof(mode), (int)mode, typeof(EngineMode));

            this.DataPath = dataPath;
            this.Language = language;
            this.Mode = mode;
        }
    }
}