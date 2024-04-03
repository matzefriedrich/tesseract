namespace Tesseract.ImageProcessing
{
    using System;
    using Abstractions;
    using Interop;
    using Interop.Abstractions;
    using Resources;
    using Tesseract.Abstractions;

    public class ImageRotator : IImageRotator
    {
        /// <summary>
        ///     A small angle, in radians, for threshold checking. Equal to about 0.06 degrees.
        /// </summary>
        private const float VerySmallAngle = 0.001F;

        private readonly ILeptonicaApiSignatures leptonicaApi;
        private readonly IPixFactory pixFactory;

        public ImageRotator(ILeptonicaApiSignatures leptonicaApi, IPixFactory pixFactory)
        {
            this.leptonicaApi = leptonicaApi ?? throw new ArgumentNullException(nameof(leptonicaApi));
            this.pixFactory = pixFactory ?? throw new ArgumentNullException(nameof(pixFactory));
        }

        /// <inheritdoc />
        public Pix RotateImage(Pix source, float angleInRadians, RotationMethod method = RotationMethod.AreaMap, RotationFill fillColor = RotationFill.White, int? width = null, int? height = null)
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

            if (resultHandle == IntPtr.Zero) throw new LeptonicaException(Resources.ImageRotator_RotateImage_Failed_to_rotate_image_around_its_centre_);

            return new Pix(this.leptonicaApi, resultHandle);
        }

        /// <inheritdoc />
        public Pix RotateImage90(Pix source, RotateDirection direction)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            int dir = direction switch
            {
                RotateDirection.Clockwise => 1,
                RotateDirection.CounterClockwise => -1,
                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, Resources.ImageRotator_RotateImage90_Invalid_direction_value_)
            };

            IntPtr resultHandle = this.leptonicaApi.pixRotate90(source.Handle, dir);

            if (resultHandle == IntPtr.Zero) throw new LeptonicaException(Resources.ImageRotator_RotateImage90_Failed_to_rotate_image_);
            return new Pix(this.leptonicaApi, resultHandle);
        }
    }
}