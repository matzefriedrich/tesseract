namespace Tesseract.ImageProcessing.Abstractions
{
    public interface IColorQuantizer
    {
        /// <summary>
        ///     Top-level conversion to 8 bpp.
        /// </summary>
        /// <param name="source">The source image to convert.</param>
        /// <param name="colorMapFlag"></param>
        /// <returns></returns>
        Pix ConvertTo8Bpp(Pix source, int colorMapFlag);
    }
}