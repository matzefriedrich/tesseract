namespace Tesseract.ImageProcessing.Abstractions
{
    public interface IImageBinarizer
    {
        /// <summary>
        ///     Binarization of the input image based on the passed parameters and the Otsu method
        /// </summary>
        /// <param name="bpp8Source">The source image to binarize.</param>
        /// <param name="sx"> sizeX Desired tile X dimension; actual size may vary.</param>
        /// <param name="sy"> sizeY Desired tile Y dimension; actual size may vary.</param>
        /// <param name="smoothX"> smoothX Half-width of convolution kernel applied to threshold array: use 0 for no smoothing.</param>
        /// <param name="smoothY"> smoothY Half-height of convolution kernel applied to threshold array: use 0 for no smoothing.</param>
        /// <param name="scoreFraction"> scoreFraction Fraction of the max Otsu score; typ. 0.1 (use 0.0 for standard Otsu).</param>
        /// <returns>The binarized image.</returns>
        Pix BinarizeOtsuAdaptiveThreshold(Pix bpp8Source, int sx, int sy, int smoothX, int smoothY, float scoreFraction);

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
        /// <param name="source">The source image to binarize.</param>
        /// <param name="whSize">the window half-width for measuring local statistics.</param>
        /// <param name="factor">
        ///     The factor for reducing threshold due to variances greater than or equal to zero (0). Typically
        ///     around 0.35.
        /// </param>
        /// <param name="addBorder">If <c>True</c> add a border of width (<paramref name="whSize" /> + 1) on all sides.</param>
        /// <returns>The binarized image.</returns>
        Pix BinarizeSauvola(Pix source, int whSize, float factor, bool addBorder);

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
        /// <param name="source">The source image to binarize.</param>
        /// <param name="whSize">The window half-width for measuring local statistics</param>
        /// <param name="factor">
        ///     The factor for reducing threshold due to variances greater than or equal to zero (0). Typically
        ///     around 0.35.
        /// </param>
        /// <param name="nx">The number of tiles to subdivide the source image into on the x-axis.</param>
        /// <param name="ny">The number of tiles to subdivide the source image into on the y-axis.</param>
        /// <returns>THe binarized image.</returns>
        Pix BinarizeSauvolaTiled(Pix source, int whSize, float factor, int nx, int ny);
    }
}