namespace Tesseract.ImageProcessing.Abstractions
{
    public interface ILineRemover
    {
        /// <summary>
        ///     Removes horizontal lines from a grayscale image. The algorithm is based on Leptonica <code>lineremoval.c</code>
        ///     example. See <a href="http://www.leptonica.com/line-removal.html">line-removal</a>.
        /// </summary>
        /// <returns>image with lines removed</returns>
        Pix RemoveHorizontalLines(Pix source);
    }
}