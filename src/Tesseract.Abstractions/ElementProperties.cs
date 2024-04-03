namespace Tesseract.Abstractions
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    ///     Represents properties that describe a text block's orientation.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public readonly struct ElementProperties(
        Orientation orientation,
        TextLineOrder textLineOrder,
        WritingDirection writingDirection,
        float deskewAngle)
    {
        /// <summary>
        ///     Gets the <see cref="Orientation" /> for corresponding text block.
        /// </summary>
        public Orientation Orientation { get; } = orientation;

        /// <summary>
        ///     Gets the <see cref="TextLineOrder" /> for corresponding text block.
        /// </summary>
        public TextLineOrder TextLineOrder { get; } = textLineOrder;

        /// <summary>
        ///     Gets the <see cref="WritingDirection" /> for corresponding text block.
        /// </summary>
        public WritingDirection WritingDirection { get; } = writingDirection;

        /// <summary>
        ///     Gets the angle the page would need to be rotated to deskew the text block.
        /// </summary>
        public float DeskewAngle { get; } = deskewAngle;
    }
}