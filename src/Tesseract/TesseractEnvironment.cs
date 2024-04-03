namespace Tesseract
{
    using System;
    using InteropDotNet;

    public sealed class TesseractEnvironment
    {
        private readonly LibraryLoader libraryLoader;

        public TesseractEnvironment(LibraryLoader libraryLoader)
        {
            this.libraryLoader = libraryLoader ?? throw new ArgumentNullException(nameof(libraryLoader));
        }

        /// <summary>
        ///     Gets or sets a search path that will be checked first when attempting to load the Tesseract and Leptonica dlls.
        /// </summary>
        /// <remarks>
        ///     This search path should not include the platform component as this will automatically be appended to the string
        ///     based on the detected platform.
        /// </remarks>
        public string? CustomSearchPath
        {
            get => this.libraryLoader.CustomSearchPath;
            set => this.libraryLoader.CustomSearchPath = value;
        }
    }
}