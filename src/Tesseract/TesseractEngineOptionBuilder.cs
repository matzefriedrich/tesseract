namespace Tesseract
{
    using System;
    using System.Collections.Generic;

    using Abstractions;

    using JetBrains.Annotations;

    public sealed class TesseractEngineOptionBuilder
    {
        private readonly List<string> configFiles = new();
        private readonly IDictionary<string, object> options = new Dictionary<string, object>();
        private readonly string dataPath;
        private readonly string language;
        private readonly EngineMode mode;
        private readonly bool setOnlyNonDebugVariables;
        
        public TesseractEngineOptionBuilder([NotNull] string dataPath, [NotNull] string language = "eng", EngineMode mode = EngineMode.Default, bool setOnlyNonDebugVariables = false)
        {
            if (string.IsNullOrWhiteSpace(dataPath)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(dataPath));
            if (string.IsNullOrWhiteSpace(language)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(language));

            this.dataPath = dataPath;
            this.language = language;
            this.mode = mode;
            this.setOnlyNonDebugVariables = setOnlyNonDebugVariables;
        }

        public TesseractEngineOptionBuilder WithConfigFile([NotNull] string file)
        {
            if (string.IsNullOrWhiteSpace(file)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(file));
            this.configFiles.Add(file);
            return this;
        }        
        
        public TesseractEngineOptionBuilder WithConfigFiles([NotNull] IEnumerable<string> files)
        {
            if (files == null) throw new ArgumentNullException(nameof(files));
            this.configFiles.AddRange(files);
            return this;
        }

        public TesseractEngineOptionBuilder WithInitialOptions([NotNull] IDictionary<string, object> dict)
        {
            if (dict == null) throw new ArgumentNullException(nameof(dict));
            foreach (KeyValuePair<string,object> pair in dict)
            {
                this.options[pair.Key] = pair.Value;
            }
            
            return this;
        }

        public TesseractEngineOptions Build()
        {
            return new TesseractEngineOptions
            {
                DataPath = this.dataPath,
                Language = this.language,
                Mode = this.mode,
                SetOnlyNonDebugVariables = setOnlyNonDebugVariables,
                ConfigurationFiles = this.configFiles.AsReadOnly(),
                InitialOptions = this.options.AsReadOnly(),
            };
        }
    }
}