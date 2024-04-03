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
            string sanitizedPath = this.GetSanitizedDataPath();

            return new TesseractEngineOptions
            {
                DataPath = sanitizedPath,
                Language = this.language,
                Mode = this.mode,
                SetOnlyNonDebugVariables = this.setOnlyNonDebugVariables,
                ConfigurationFiles = this.configFiles.AsReadOnly(),
                InitialOptions = this.options.AsReadOnly()
            };
        }

        /// <summary>
        ///     Does some minor processing on <seealso cref="dataPath" /> to fix some probable errors (this basically mirrors what tesseract does as of 3.04).
        /// </summary>
        /// <returns>Returns a value indicating a path that is safe to construct a <see cref="TesseractEngineOptions" /> object.</returns>
        private string GetSanitizedDataPath()
        {
            string path = this.dataPath;
            if (string.IsNullOrEmpty(path)) return this.dataPath;

            string trimmedPath = path.Trim();

            // remove any trialing '\' or '/' characters
            if (trimmedPath.EndsWith('\\') || trimmedPath.EndsWith('/'))
                return trimmedPath.Substring(0, trimmedPath.Length - 1);

            return trimmedPath;
        }
    }
}