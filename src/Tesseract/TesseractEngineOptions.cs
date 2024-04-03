namespace Tesseract
{
    using System.Collections.ObjectModel;
    using Abstractions;

    /// <summary>
    ///     Stores the configuration properties required to create <see cref="TesseractEngine" /> objects.
    /// </summary>
    /// <remarks>
    ///     The <see cref="DataPath" /> property should point to the directory that contains the "testdata" folder. For
    ///     example, if your tesseract language data is installed in <c>C:\Tesseract\tessdata</c> the value of <see cref="DataPath" /> should
    ///     be <c>C:\Tesseract</c>. Note that tesseract will use the value of the <c>TESSDATA_PREFIX</c> environment variable
    ///     if defined, effectively ignoring the value of <see cref="DataPath" /> parameter.
    /// </remarks>
    public readonly struct TesseractEngineOptions
    {
        /// <summary>
        ///     The path to the parent directory that contains the tessdata directory, ignored if the <c>TESSDATA_PREFIX</c>
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