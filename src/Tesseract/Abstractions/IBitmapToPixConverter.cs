namespace Tesseract.Abstractions
{
    using System.Drawing;

    public interface IBitmapToPixConverter
    {
        /// <summary>
        ///     Converts the specified <paramref name="img" /> to a <see cref="Pix" />.
        /// </summary>
        /// <param name="img">The source image to be converted.</param>
        /// <returns>The converted pix.</returns>
        Pix Convert(Bitmap img);
    }
}