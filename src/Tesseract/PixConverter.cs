namespace Tesseract
{
    using System;
    using System.Drawing;

    using Abstractions;

    using JetBrains.Annotations;

    /// <summary>
    ///     Handles converting between different image formats supported by DotNet.
    /// </summary>
    public class PixConverter : IPixConverter
    {
        private readonly IBitmapToPixConverter bitmapConverter;
        private readonly IPixToBitmapConverter pixConverter;

        public PixConverter(
            [NotNull] IBitmapToPixConverter bitmapConverter,
            [NotNull] IPixToBitmapConverter pixConverter)
        {
            this.bitmapConverter = bitmapConverter ?? throw new ArgumentNullException(nameof(bitmapConverter));
            this.pixConverter = pixConverter ?? throw new ArgumentNullException(nameof(pixConverter));
        }

        /// <summary>
        ///     Converts the specified <paramref name="pix" /> to a Bitmap.
        /// </summary>
        /// <param name="pix">The source image to be converted.</param>
        /// <returns>The converted pix as a <see cref="Bitmap" />.</returns>
        public Bitmap ToBitmap(Pix pix)
        {
            return this.pixConverter.Convert(pix);
        }

        /// <summary>
        ///     Converts the specified <paramref name="img" /> to a Pix.
        /// </summary>
        /// <param name="img">The source image to be converted.</param>
        /// <returns>The converted bitmap image as a <see cref="Pix" />.</returns>
        public Pix ToPix(Bitmap img)
        {
            return this.bitmapConverter.Convert(img);
        }
    }
}