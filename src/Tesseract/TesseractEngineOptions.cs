namespace Tesseract
{
    using System.Collections.ObjectModel;

    using Abstractions;

    public readonly struct TesseractEngineOptions
    {
        /// <summary>
        ///     The path to the parent directory that contains the 'tessdata' directory, ignored if the <c>TESSDATA_PREFIX</c>
        ///     environment variable is defined.
        /// </summary>
        public string DataPath { get; init; }

        /// <summary>
        ///     The language to load, for example 'eng' for English.
        /// </summary>
        public string Language { get; init; }

        /// <summary>
        ///     The <see cref="EngineMode" /> value to use when initialising the tesseract engine.
        /// </summary>
        public EngineMode Mode { get; init; }

        public bool SetOnlyNonDebugVariables { get; init; }

        /// <summary>
        ///     An optional sequence of tesseract configuration files to load, encoded using UTF8 without BOM with Unix end of line
        ///     characters you can use an advanced text editor such as Notepad++ to accomplish this.
        /// </summary>
        public ReadOnlyCollection<string> ConfigurationFiles { get; init; }

        public ReadOnlyDictionary<string, object> InitialOptions { get; init; }
    }
}