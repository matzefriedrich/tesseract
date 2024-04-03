namespace Tesseract
{
    using System;
    using System.Collections.Generic;
    using Abstractions;

    public sealed class TesseractEngineOptionBuilder
    {
        private readonly List<string> configFiles = [];
        private readonly string dataPath;
        private readonly string language;
        private readonly EngineMode mode;
        private readonly IDictionary<string, object> options = new Dictionary<string, object>();
        private readonly bool setOnlyNonDebugVariables;

        public TesseractEngineOptionBuilder(string dataPath, string language = "eng", EngineMode mode = EngineMode.Default, bool setOnlyNonDebugVariables = false)
        {
            if (string.IsNullOrWhiteSpace(dataPath)) throw new ArgumentException(Resources.Resources.Value_cannot_be_null_or_whitespace, nameof(dataPath));
            if (string.IsNullOrWhiteSpace(language)) throw new ArgumentException(Resources.Resources.Value_cannot_be_null_or_whitespace, nameof(language));

            this.dataPath = dataPath;
            this.language = language;
            this.mode = mode;
            this.setOnlyNonDebugVariables = setOnlyNonDebugVariables;
        }

        public TesseractEngineOptionBuilder WithConfigFile(string file)
        {
            if (string.IsNullOrWhiteSpace(file)) throw new ArgumentException(Resources.Resources.Value_cannot_be_null_or_whitespace, nameof(file));
            this.configFiles.Add(file);
            return this;
        }

        public TesseractEngineOptionBuilder WithConfigFiles(IEnumerable<string> files)
        {
            ArgumentNullException.ThrowIfNull(files);
            this.configFiles.AddRange(files);
            return this;
        }

        public TesseractEngineOptionBuilder WithInitialOptions(IDictionary<string, object> dict)
        {
            ArgumentNullException.ThrowIfNull(dict);
            foreach (KeyValuePair<string, object> pair in dict) this.options[pair.Key] = pair.Value;

            return this;
        }

        public TesseractEngineOptions Build()
        {
            return new TesseractEngineOptions
            {
                DataPath = this.dataPath,
                Language = this.language,
                Mode = this.mode,
                SetOnlyNonDebugVariables = this.setOnlyNonDebugVariables,
                ConfigurationFiles = this.configFiles.AsReadOnly(),
                InitialOptions = this.options.AsReadOnly()
            };
        }
    }
}