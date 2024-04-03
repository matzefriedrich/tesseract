namespace Tesseract.ImageProcessing.Abstractions
{
    public interface IGrayscaleConverter
    {
        /// <summary>
        ///     Conversion from RBG to 8bpp grayscale using the specified weights. Note red, green, blue weights should add up to
        ///     1.0.
        /// </summary>
        /// <param name="source">The source image to grayscale.</param>
        /// <param name="weightRed">Red weight</param>
        /// <param name="weightGreen">Green weight</param>
        /// <param name="weightBlue">Blue weight</param>
        /// <returns>The Grayscale pix.</returns>
        Pix ConvertRgbToGray(Pix source, float weightRed = 0, float weightGreen = 0, float weightBlue = 0);
    }
}