namespace Tesseract
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Abstractions;
    using Microsoft.Extensions.Options;

    public sealed class EngineOptionBuilder
    {
        private readonly List<string> configFiles = [];
        private string dataPath;
        private string language;
        private EngineMode mode;
        private readonly IDictionary<string, object> options = new Dictionary<string, object>();
        private readonly bool setOnlyNonDebugVariables = false;

        public EngineOptionBuilder(IOptions<EngineOptionDefaults> defaults)
        {
            ArgumentNullException.ThrowIfNull(defaults);

            this.dataPath = defaults.Value.DataPath;
            this.language = defaults.Value.Language;
            this.mode = defaults.Value.Mode;
        }

        public EngineOptionBuilder WithConfigFile(string file)
        {
            if (string.IsNullOrWhiteSpace(file)) throw new ArgumentException(Resources.Resources.Value_cannot_be_null_or_whitespace, nameof(file));
            this.configFiles.Add(file);
            return this;
        }

        public EngineOptionBuilder WithConfigFiles(IEnumerable<string> files)
        {
            ArgumentNullException.ThrowIfNull(files);
            this.configFiles.AddRange(files);
            return this;
        }

        public EngineOptionBuilder WithOption(string variableName, object value)
        {
            ArgumentNullException.ThrowIfNull(value);
            if (string.IsNullOrWhiteSpace(variableName)) throw new ArgumentException(Resources.Resources.Value_cannot_be_null_or_whitespace, nameof(variableName));

            this.options[variableName] = variableName;
            return this;
        }

        public EngineOptionBuilder WithOptions(params (string, object)[] tuples)
        {
            ArgumentNullException.ThrowIfNull(tuples);
            foreach ((string variableName, object value) in tuples)
            {
                this.options[variableName] = value;
            }

            return this;
        }
        
        public EngineOptionBuilder WithOptions(IDictionary<string, object> dict)
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

        public EngineOptionBuilder WithMode(EngineMode engineMode)
        {
            if (!Enum.IsDefined(typeof(EngineMode), engineMode)) throw new InvalidEnumArgumentException(nameof(engineMode), (int)engineMode, typeof(EngineMode));
            this.mode = engineMode;
            return this;
        }

        public EngineOptionBuilder Language(string language)
        {
            if (string.IsNullOrWhiteSpace(language)) throw new ArgumentException(Resources.Resources.Value_cannot_be_null_or_whitespace, nameof(language));
            this.language = language;
            return this;
        }

        public EngineOptionBuilder OverrideDataPath(string dataPath)
        {
            if (string.IsNullOrWhiteSpace(dataPath)) throw new ArgumentException(Resources.Resources.Value_cannot_be_null_or_whitespace, nameof(dataPath));
            this.dataPath = dataPath;
            return this;
        }
    }
}