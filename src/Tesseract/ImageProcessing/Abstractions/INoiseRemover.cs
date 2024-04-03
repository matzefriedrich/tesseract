namespace Tesseract.ImageProcessing.Abstractions
{
    public interface INoiseRemover
    {
        /// <summary>
        ///     Reduces speckle noise in image. The algorithm is based on Leptonica <code>speckle_reg.c</code> example
        ///     demonstrating morphological method of removing speckle.
        /// </summary>
        /// <param name="source">The noisy source image.</param>
        /// <param name="selStr">A <see cref="SelString"/> value representing the hit-miss sels in 2D layout; use <see cref="SelString.Str2"/>, or <see cref="SelString.Str3"/> as predefined values.</param>
        /// <param name="selSize">Specifies the sel size; either 2 for 2x2, 3 for 3x3.</param>
        /// <returns>Returns a new <see cref="Pix" /> object.</returns>
        Pix ReduceSpeckleNoise(Pix source, SelString selStr, int selSize);
    }
}