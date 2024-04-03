namespace Tesseract.Abstractions
{
    using System.Diagnostics.CodeAnalysis;

    /// <remarks>
    ///     The .NET equivalent of the ccstruct/fontinfo.h FontInfo struct. It's missing spacing info since we don't have
    ///     any way of getting it (and it's probably not all that useful anyway)
    /// </remarks>
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class FontInfo
    {
        public FontInfo(string name, int id, bool isItalic, bool isBold, bool isFixedPitch, bool isSerif, bool isBlackLetterFont = false)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
            this.Name = name;
            this.Id = id;

            this.IsItalic = isItalic;
            this.IsBold = isBold;
            this.IsFixedPitch = isFixedPitch;
            this.IsSerif = isSerif;
            this.IsBlackLetterFont = isBlackLetterFont;
        }

        public string Name { get; private set; }

        public int Id { get; private set; }
        public bool IsItalic { get; private set; }
        public bool IsBold { get; private set; }
        public bool IsFixedPitch { get; private set; }
        public bool IsSerif { get; private set; }
        public bool IsBlackLetterFont { get; private set; }
    }
}