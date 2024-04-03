namespace Tesseract.ImageProcessing
{
    using System;
    using Abstractions;
    using Interop.Abstractions;
    using Resources;
    using Tesseract.Abstractions;

    public class ImageBinarizer : IImageBinarizer
    {
        private const int ResultError = 1;
        private readonly ILeptonicaApiSignatures leptonicaApi;

        public ImageBinarizer(ILeptonicaApiSignatures leptonicaApi)
        {
            this.leptonicaApi = leptonicaApi ?? throw new ArgumentNullException(nameof(leptonicaApi));
        }

        /// <inheritdoc />
        public Pix BinarizeOtsuAdaptiveThreshold(Pix bpp8Source, int sx, int sy, int smoothX, int smoothY, float scoreFraction)
        {
            if (bpp8Source == null) throw new ArgumentNullException(nameof(bpp8Source));
            if (bpp8Source.Depth != 8) throw new InvalidOperationException("Image must have a depth of 8 bits per pixel to be binarized using Otsu.");
            if (sx < 16) throw new ArgumentException(string.Format(Resources.ImageBinarizer_BinarizeOtsuAdaptiveThreshold_The__0__parameter_must_be_greater_than_or_equal_to__1__, nameof(sx), 16), nameof(sx));
            if (sy < 16) throw new ArgumentException(string.Format(Resources.ImageBinarizer_BinarizeOtsuAdaptiveThreshold_The__0__parameter_must_be_greater_than_or_equal_to__1__, nameof(sy), 16), nameof(sy));

            int result = this.leptonicaApi.pixOtsuAdaptiveThreshold(bpp8Source.Handle, sx, sy, smoothX, smoothY, scoreFraction, out IntPtr arrThresholdValues, out IntPtr thresholdedInputPixs);

            if (arrThresholdValues != IntPtr.Zero)
                this.leptonicaApi.pixDestroy(ref arrThresholdValues);

            if (result == ResultError)
                throw new TesseractException(Resources.ImageBinarizer_BinarizeOtsuAdaptiveThreshold_Failed_to_binarize_image_);

            return new Pix(this.leptonicaApi, thresholdedInputPixs);
        }

        /// <inheritdoc />
        public Pix BinarizeSauvola(Pix source, int whSize, float factor, bool addBorder)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (source.Depth != 8) throw new ArgumentException("Source image must be 8bpp");
            if (source.Colormap != null) throw new ArgumentException("Source image must not be color mapped.");
            if (whSize < 2) throw new ArgumentException("The window half-width (whsize) must be greater than 2.", nameof(whSize));

            int maxWhSize = Math.Min((source.Width - 3) / 2, (source.Height - 3) / 2);
            if (whSize >= maxWhSize) throw new ArgumentException($"The window half-width (whsize) must be less than {maxWhSize} for this image.", nameof(whSize));
            if (factor < 0) throw new ArgumentException(Resources.ImageBinarizer_BinarizeSauvola_Factor_must_be_greater_than_zero__0__, nameof(factor));

            int result = this.leptonicaApi.pixSauvolaBinarize(source.Handle, whSize, factor, addBorder ? 1 : 0, out IntPtr ppixm, out IntPtr ppixsd, out IntPtr ppixth, out IntPtr ppixd);

            // Free memory held by other unused pix's

            if (ppixm != IntPtr.Zero) this.leptonicaApi.pixDestroy(ref ppixm);

            if (ppixsd != IntPtr.Zero) this.leptonicaApi.pixDestroy(ref ppixsd);

            if (ppixth != IntPtr.Zero) this.leptonicaApi.pixDestroy(ref ppixth);

            if (result == ResultError) throw new TesseractException(Resources.ImageBinarizer_BinarizeOtsuAdaptiveThreshold_Failed_to_binarize_image_);

            return new Pix(this.leptonicaApi, ppixd);
        }

        /// <inheritdoc />
        public Pix BinarizeSauvolaTiled(Pix source, int whSize, float factor, int nx, int ny)
        {
            ArgumentNullException.ThrowIfNull(source);

            if (source.Depth != 8) throw new InvalidOperationException("The source image must have a depth of 8 bits per pixel.");
            if (source.Colormap != null) throw new InvalidOperationException("The source image must not be color mapped.");
            if (whSize < 2) throw new ArgumentException("The window half-width (whsize) must be greater than 2.", nameof(whSize));

            int maxWhSize = Math.Min((source.Width - 3) / 2, (source.Height - 3) / 2);
            if (whSize >= maxWhSize) throw new ArgumentException($"The window half-width (whsize) must be less than {maxWhSize} for this image.", nameof(whSize));
            if (factor < 0) throw new ArgumentException(Resources.ImageBinarizer_BinarizeSauvola_Factor_must_be_greater_than_zero__0__);

            int result = this.leptonicaApi.pixSauvolaBinarizeTiled(source.Handle, whSize, factor, nx, ny, out IntPtr ppixth, out IntPtr ppixd);

            // Free memory held by other unused pix's
            if (ppixth != IntPtr.Zero) this.leptonicaApi.pixDestroy(ref ppixth);

            if (result == ResultError) throw new TesseractException(Resources.ImageBinarizer_BinarizeOtsuAdaptiveThreshold_Failed_to_binarize_image_);

            return new Pix(this.leptonicaApi, ppixd);
        }
    }
}