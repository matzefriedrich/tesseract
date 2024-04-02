namespace Tesseract.ImageProcessing
{
    using System;
    using Abstractions;
    using Interop.Abstractions;
    using JetBrains.Annotations;

    public class ImageRotator
    {
        /// <summary>
        ///     A small angle, in radians, for threshold checking. Equal to about 0.06 degrees.
        /// </summary>
        private const float VerySmallAngle = 0.001F;

        private readonly ILeptonicaApiSignatures leptonicaApi;
        private readonly IPixFactory pixFactory;

        public ImageRotator([NotNull] ILeptonicaApiSignatures leptonicaApi, [NotNull] IPixFactory pixFactory)
        {
            this.leptonicaApi = leptonicaApi ?? throw new ArgumentNullException(nameof(leptonicaApi));
            this.pixFactory = pixFactory ?? throw new ArgumentNullException(nameof(pixFactory));
        }

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
        /// <param name="angleInRadians">The angle to rotate by, in radians; clockwise is positive.</param>
        /// <param name="method">The rotation method to use.</param>
        /// <param name="fillColor">The fill color to use for pixels that are brought in from the outside.</param>
        /// <param name="width">The original width; use 0 to avoid embedding</param>
        /// <param name="height">The original height; use 0 to avoid embedding</param>
        /// <returns>The image rotated around it's centre.</returns>
        public Pix RotateImage([NotNull] Pix source, float angleInRadians, RotationMethod method = RotationMethod.AreaMap, RotationFill fillColor = RotationFill.White, int? width = null, int? height = null)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (width == null) width = source.Width;
            if (height == null) height = source.Height;

            if (Math.Abs(angleInRadians) < VerySmallAngle) return this.pixFactory.Clone(source);

            IntPtr resultHandle;

            double rotations = 2 * angleInRadians / Math.PI;
            if (Math.Abs(rotations - Math.Floor(rotations)) < VerySmallAngle)
                // handle special case of orthoganal rotations (90, 180, 270)
                resultHandle = this.leptonicaApi.pixRotateOrth(source.Handle, (int)rotations);
            else
                // handle general case
                resultHandle = this.leptonicaApi.pixRotate(source.Handle, angleInRadians, method, fillColor, width.Value, height.Value);

            if (resultHandle == IntPtr.Zero) throw new LeptonicaException("Failed to rotate image around its centre.");

            return new Pix(this.leptonicaApi, resultHandle);
        }

        /// <summary>
        ///     90 degree rotation.
        /// </summary>
        /// <param name="direction">1 = clockwise,  -1 = counter-clockwise</param>
        /// <returns>rotated image</returns>
        public Pix RotateImage90([NotNull] Pix source, RotateDirection direction)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            int dir = direction switch
            {
                RotateDirection.Clockwise => 1,
                RotateDirection.CounterClockwise => -1,
                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, "Invalid direction value.")
            };
            
            IntPtr resultHandle = this.leptonicaApi.pixRotate90(source.Handle, dir);

            if (resultHandle == IntPtr.Zero) throw new LeptonicaException("Failed to rotate image.");
            return new Pix(this.leptonicaApi, resultHandle);
        }
    }
}