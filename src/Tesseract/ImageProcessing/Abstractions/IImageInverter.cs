namespace Tesseract.ImageProcessing.Abstractions
{
    public interface IImageInverter
    {
        /// <summary>
        ///     Inverts pix for all pixel depths.
        /// </summary>
        /// <returns></returns>
        Pix InvertImage(Pix source);
    }
}