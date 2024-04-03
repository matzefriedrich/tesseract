namespace Tesseract.Abstractions
{
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class FontAttributes(
        FontInfo fontInfo,
        bool isUnderlined,
        bool isSmallCaps,
        int pointSize)
    {
        public FontInfo FontInfo { get; private set; } = fontInfo ?? throw new ArgumentNullException(nameof(fontInfo));
        public bool IsUnderlined { get; private set; } = isUnderlined;
        public bool IsSmallCaps { get; private set; } = isSmallCaps;
        public int PointSize { get; private set; } = pointSize;
    }
}