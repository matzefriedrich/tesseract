namespace Tesseract.Abstractions
{
    /// <summary>
    ///     Represents properties that describe a text block's orientation.
    /// </summary>
    public struct ElementProperties
    {
        public ElementProperties(Orientation orientation, TextLineOrder textLineOrder, WritingDirection writingDirection, float deskewAngle)
        {
            this.Orientation = orientation;
            this.TextLineOrder = textLineOrder;
            this.WritingDirection = writingDirection;
            this.DeskewAngle = deskewAngle;
        }

        /// <summary>
        ///     Gets the <see cref="Orientation" /> for corresponding text block.
        /// </summary>
        public Orientation Orientation { get; }

        /// <summary>
        ///     Gets the <see cref="TextLineOrder" /> for corresponding text block.
        /// </summary>
        public TextLineOrder TextLineOrder { get; }

        /// <summary>
        ///     Gets the <see cref="WritingDirection" /> for corresponding text block.
        /// </summary>
        public WritingDirection WritingDirection { get; }

        /// <summary>
        ///     Gets the angle the page would need to be rotated to deskew the text block.
        /// </summary>
        public float DeskewAngle { get; }
    }
}