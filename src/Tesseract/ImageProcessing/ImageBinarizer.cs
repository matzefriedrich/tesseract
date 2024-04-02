namespace Tesseract.ImageProcessing
{
    using System;
    using Abstractions;
    using Interop.Abstractions;
    using JetBrains.Annotations;

    public class ImageBinarizer
    {
        private const int resultError = 1;
        private readonly ILeptonicaApiSignatures leptonicaApi;

        public ImageBinarizer([NotNull] ILeptonicaApiSignatures leptonicaApi)
        {
            this.leptonicaApi = leptonicaApi ?? throw new ArgumentNullException(nameof(leptonicaApi));
        }

        /// <summary>
        ///     Binarization of the input image based on the passed parameters and the Otsu method
        /// </summary>
        /// <param name="sx"> sizeX Desired tile X dimension; actual size may vary.</param>
        /// <param name="sy"> sizeY Desired tile Y dimension; actual size may vary.</param>
        /// <param name="smoothX"> smoothX Half-width of convolution kernel applied to threshold array: use 0 for no smoothing.</param>
        /// <param name="smoothY"> smoothY Half-height of convolution kernel applied to threshold array: use 0 for no smoothing.</param>
        /// <param name="scoreFraction"> scoreFraction Fraction of the max Otsu score; typ. 0.1 (use 0.0 for standard Otsu).</param>
        /// <returns>The binarized image.</returns>
        public Pix BinarizeOtsuAdaptiveThreshold([NotNull] Pix bpp8Source, int sx, int sy, int smoothX, int smoothY, float scoreFraction)
        {
            if (bpp8Source == null) throw new ArgumentNullException(nameof(bpp8Source));
            if (bpp8Source.Depth != 8) throw new InvalidOperationException("Image must have a depth of 8 bits per pixel to be binarized using Otsu.");
            if (sx < 16) throw new ArgumentException("The sx parameter must be greater than or equal to 16", nameof(sx));
            if (sy < 16) throw new ArgumentException("The sy parameter must be greater than or equal to 16", nameof(sy));

            int result = this.leptonicaApi.pixOtsuAdaptiveThreshold(bpp8Source.Handle, sx, sy, smoothX, smoothY, scoreFraction, out IntPtr arrThresholdValues, out IntPtr thresholdedInputPixs);

            if (arrThresholdValues != IntPtr.Zero)
                // free memory held by ppixth, an array of threshold values found for each tile
                this.leptonicaApi.pixDestroy(ref arrThresholdValues);

            if (result == resultError)
                throw new TesseractException("Failed to binarize image.");

            return new Pix(this.leptonicaApi, thresholdedInputPixs);
        }

        /// <summary>
        ///     Binarization of the input image using the Sauvola local thresholding method.
        ///     Note: The source image must be 8 bpp grayscale; not colormapped.
        /// </summary>
        /// <remarks>
        ///     <list type="number">
        ///         <listheader>Notes</listheader>
        ///         <item>
        ///             The window width and height are 2 * <paramref name="whSize" /> + 1. The minimum value for
        ///             <paramref name="whSize" /> is 2; typically it is >= 7.
        ///         </item>
        ///         <item>The local statistics, measured over the window, are the average and standard deviation.</item>
        ///         <item>
        ///             The measurements of the mean and standard deviation are performed inside a border of (
        ///             <paramref name="whSize" /> + 1) pixels.
        ///             If source pix does not have these added border pixels, use <paramref name="addBorder" /> = <c>True</c> to
        ///             add it here; otherwise use
        ///             <paramref name="addBorder" /> = <c>False</c>.
        ///         </item>
        ///         <item>
        ///             The Sauvola threshold is determined from the formula:  t = m * (1 - k * (1 - s / 128)) where t = local
        ///             threshold, m = local mean,
        ///             k = <paramref name="factor" />, and s = local standard deviation which is maximised at 127.5 when half the
        ///             samples are 0 and the other
        ///             half are 255.
        ///         </item>
        ///         <item>
        ///             The basic idea of Niblack and Sauvola binarization is that the local threshold should be less than the
        ///             median value,
        ///             and the larger the variance, the closer to the median it should be chosen. Typical values for k are between
        ///             0.2 and 0.5.
        ///         </item>
        ///     </list>
        /// </remarks>
        /// <param name="whSize">the window half-width for measuring local statistics.</param>
        /// <param name="factor">
        ///     The factor for reducing threshold due to variances greater than or equal to zero (0). Typically
        ///     around 0.35.
        /// </param>
        /// <param name="addBorder">If <c>True</c> add a border of width (<paramref name="whSize" /> + 1) on all sides.</param>
        /// <returns>The binarized image.</returns>
        public Pix BinarizeSauvola([NotNull] Pix source, int whSize, float factor, bool addBorder)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (source.Depth != 8) throw new ArgumentException("Source image must be 8bpp");
            if (source.Colormap != null) throw new ArgumentException("Source image must not be color mapped.");
            if (whSize < 2) throw new ArgumentException("The window half-width (whsize) must be greater than 2.", nameof(whSize));

            int maxWhSize = Math.Min((source.Width - 3) / 2, (source.Height - 3) / 2);
            if (whSize >= maxWhSize) throw new ArgumentException($"The window half-width (whsize) must be less than {maxWhSize} for this image.", nameof(whSize));
            if (factor < 0) throw new ArgumentException("Factor must be greater than zero (0).", nameof(factor));

            int result = this.leptonicaApi.pixSauvolaBinarize(source.Handle, whSize, factor, addBorder ? 1 : 0, out IntPtr ppixm, out IntPtr ppixsd, out IntPtr ppixth, out IntPtr ppixd);

            // Free memory held by other unused pix's

            if (ppixm != IntPtr.Zero) this.leptonicaApi.pixDestroy(ref ppixm);

            if (ppixsd != IntPtr.Zero) this.leptonicaApi.pixDestroy(ref ppixsd);

            if (ppixth != IntPtr.Zero) this.leptonicaApi.pixDestroy(ref ppixth);

            if (result == resultError) throw new TesseractException("Failed to binarize image.");

            return new Pix(this.leptonicaApi, ppixd);
        }

        /// <summary>
        ///     Binarization of the input image using the Sauvola local thresholding method on tiles
        ///     of the source image.
        ///     Note: The source image must be 8 bpp grayscale; not colormapped.
        /// </summary>
        /// <remarks>
        ///     A tiled version of Sauvola can become neccisary for large source images (over 16M pixels) because:
        ///     * The mean value accumulator is a uint32, overflow can occur for an image with more than 16M pixels.
        ///     * The mean value accumulator array for 16M pixels is 64 MB. While the mean square accumulator array for 16M pixels
        ///     is 128 MB.
        ///     Using tiles reduces the size of these arrays.
        ///     * Each tile can be processed independently, in parallel, on a multicore processor.
        /// </remarks>
        /// <param name="whSize">The window half-width for measuring local statistics</param>
        /// <param name="factor">
        ///     The factor for reducing threshold due to variances greater than or equal to zero (0). Typically
        ///     around 0.35.
        /// </param>
        /// <param name="nx">The number of tiles to subdivide the source image into on the x-axis.</param>
        /// <param name="ny">The number of tiles to subdivide the source image into on the y-axis.</param>
        /// <returns>THe binarized image.</returns>
        public Pix BinarizeSauvolaTiled([NotNull] Pix source, int whSize, float factor, int nx, int ny)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (source.Depth != 8) throw new InvalidOperationException("Source image must be 8bpp");
            if (source.Colormap != null) throw new InvalidOperationException("Source image must not be color mapped.");
            if (whSize < 2) throw new ArgumentException("The window half-width (whsize) must be greater than 2.", nameof(whSize));

            int maxWhSize = Math.Min((source.Width - 3) / 2, (source.Height - 3) / 2);
            if (whSize >= maxWhSize) throw new ArgumentException($"The window half-width (whsize) must be less than {maxWhSize} for this image.", nameof(whSize));
            if (factor < 0) throw new ArgumentException("Factor must be greater than zero (0).");

            int result = this.leptonicaApi.pixSauvolaBinarizeTiled(source.Handle, whSize, factor, nx, ny, out IntPtr ppixth, out IntPtr ppixd);

            // Free memory held by other unused pix's
            if (ppixth != IntPtr.Zero) this.leptonicaApi.pixDestroy(ref ppixth);

            if (result == resultError) throw new TesseractException("Failed to binarize image.");

            return new Pix(this.leptonicaApi, ppixd);
        }
    }
}