namespace Tesseract.ImageProcessing.Abstractions
{
    using Tesseract.Interop;

    public interface IImageRotator
    {
        /// <summary>
        ///     Creates a new image by rotating this image about it's centre.
        /// </summary>
        /// <remarks>
        ///     Please note the following:
        ///     <list type="bullet">
        ///         <item>
        ///             Rotation will bring in either white or black pixels, as specified by <paramref name="fillColor" /> from
        ///             the outside as required.
        ///         </item>
        ///         <item>Above 20 degrees, sampling rotation will be used if shear was requested.</item>
        ///         <item>Colormaps are removed for rotation by area map and shear.</item>
        ///         <item>
        ///             The resulting image can be expanded so that no image pixels are lost. To invoke expansion,
        ///             input the original width and height. For repeated rotation, use of the original width and heigh allows
        ///             expansion to stop at the maximum required size which is a square of side = sqrt(w*w + h*h).
        ///         </item>
        ///     </list>
        ///     <para>
        ///         Please note there is an implicit assumption about RGB component ordering.
        ///     </para>
        /// </remarks>
        /// <param name="source">The source image to rotate.</param>
        /// <param name="angleInRadians">The angle to rotate by, in radians; clockwise is positive.</param>
        /// <param name="method">The rotation method to use.</param>
        /// <param name="fillColor">The fill color to use for pixels that are brought in from the outside.</param>
        /// <param name="width">The original width; use 0 to avoid embedding</param>
        /// <param name="height">The original height; use 0 to avoid embedding</param>
        /// <returns>The image rotated around it's centre.</returns>
        Pix RotateImage(Pix source, float angleInRadians, RotationMethod method = RotationMethod.AreaMap, RotationFill fillColor = RotationFill.White, int? width = null, int? height = null);

        /// <summary>
        ///     90 degree rotation.
        /// </summary>
        /// <param name="source">The source image to rotate.</param>
        /// <param name="direction">1 = clockwise,  -1 = counter-clockwise</param>
        /// <returns>rotated image</returns>
        Pix RotateImage90(Pix source, RotateDirection direction);
    }
}