namespace Tesseract.Abstractions
{
    using System.Drawing;

    public interface IPixToBitmapConverter
    {
        Bitmap Convert(Pix pix, bool includeAlpha = false);
    }
}