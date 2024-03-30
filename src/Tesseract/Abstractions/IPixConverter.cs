namespace Tesseract.Abstractions
{
    using System.Drawing;

    public interface IPixConverter
    {
        /// <summary>
        ///     Converts the specified <paramref name="pix" /> to a Bitmap.
        /// </summary>
        /// <param name="pix">The source image to be converted.</param>
        /// <returns>The converted pix as a <see cref="Bitmap" />.</returns>
        Bitmap ToBitmap(Pix pix);

        /// <summary>
        ///     Converts the specified <paramref name="img" /> to a Pix.
        /// </summary>
        /// <param name="img">The source image to be converted.</param>
        /// <returns>The converted bitmap image as a <see cref="Pix" />.</returns>
        Pix ToPix(Bitmap img);
    }
}