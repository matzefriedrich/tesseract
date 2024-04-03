namespace Tesseract.Abstractions
{
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using Interop;

    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public static class TesseractEngineBitmapProcessingExtensions
    {
        /// <summary>
        ///     Process the specified bitmap image.
        /// </summary>
        /// <remarks>
        ///     Please consider <see cref="System.Diagnostics.Process" /> instead. This is because
        ///     this method must convert the bitmap to a pix for processing which will add additional overhead.
        ///     Leptonica also supports a large number of image pre-processing functions as well.
        /// </remarks>
        /// <param name="engine">The <see cref="ITesseractEngine" /> object that processes the image.</param>
        /// <param name="converter">An <see cref="IPixConverter" /> object that is used convert <see cref="Bitmap" /> objects into <see cref="Pix" /> objects.</param>
        /// <param name="image">The image to process.</param>
        /// <param name="inputName">Sets the input file's name, only needed for training or loading a uzn file.</param>
        /// <param name="pageSegMode">The page segmentation mode.</param>
        /// <returns></returns>
        public static Page Process(this ITesseractEngine engine, IPixConverter converter, Bitmap image, string inputName, PageSegMode? pageSegMode = null)
        {
            var region = new Rect(0, 0, image.Width, image.Height);
            return engine.Process(converter, image, inputName, region, pageSegMode);
        }

        /// <summary>
        ///     Process the specified bitmap image.
        /// </summary>
        /// <remarks>
        ///     Please consider <see cref="System.Diagnostics.Process" /> instead. This is because this method must convert the bitmap to a pix for processing which will add additional overhead. Leptonica also supports a large number of image
        ///     pre-processing functions as well.
        /// </remarks>
        /// <param name="engine">The <see cref="ITesseractEngine" /> object that processes the image.</param>
        /// <param name="converter">An <see cref="IPixConverter" /> object that is used convert <see cref="Bitmap" /> objects into <see cref="Pix" /> objects.</param>
        /// <param name="image">The image to process.</param>
        /// <param name="pageSegMode">The page segmentation mode.</param>
        /// <returns></returns>
        public static Page Process(this ITesseractEngine engine, IPixConverter converter, Bitmap image, PageSegMode? pageSegMode = null)
        {
            var region = new Rect(0, 0, image.Width, image.Height);
            return engine.Process(converter, image, region, pageSegMode);
        }

        /// <summary>
        ///     Process the specified bitmap image.
        /// </summary>
        /// <remarks>
        ///     Please consider <see cref="TesseractEngine.Process(Pix, Rect, PageSegMode?)" /> instead. This is because
        ///     this method must convert the bitmap to a pix for processing which will add additional overhead.
        ///     Leptonica also supports a large number of image pre-processing functions as well.
        /// </remarks>
        /// <param name="engine">The <see cref="ITesseractEngine" /> object that processes the image.</param>
        /// <param name="converter">An <see cref="IPixConverter" /> object that is used convert <see cref="Bitmap" /> objects into <see cref="Pix" /> objects.</param>
        /// <param name="image">The image to process.</param>
        /// <param name="region">The region of the image to process.</param>
        /// <param name="pageSegMode">The page segmentation mode.</param>
        /// <returns></returns>
        public static Page Process(this ITesseractEngine engine, IPixConverter converter, Bitmap image, Rect region, PageSegMode? pageSegMode = null)
        {
            return engine.Process(converter, image, null, region, pageSegMode);
        }

        /// <summary>
        ///     Process the specified bitmap image.
        /// </summary>
        /// <remarks>
        ///     Please consider <see cref="TesseractEngine.Process(Pix, string, Rect, PageSegMode?)" /> instead. This is because this method must convert the bitmap to a pix for processing which will add additional overhead. Leptonica also supports a
        ///     large number of image pre-processing functions as well.
        /// </remarks>
        /// <param name="engine">The <see cref="ITesseractEngine" /> object that processes the image.</param>
        /// <param name="converter">An <see cref="IPixConverter" /> object that is used convert <see cref="Bitmap" /> objects into <see cref="Pix" /> objects.</param>
        /// <param name="image">The image to process.</param>
        /// <param name="inputName">Sets the input file's name, only needed for training or loading a uzn file.</param>
        /// <param name="region">The region of the image to process.</param>
        /// <param name="pageSegMode">The page segmentation mode.</param>
        /// <returns></returns>
        public static Page Process(this ITesseractEngine engine, IPixConverter converter, Bitmap image, string? inputName, Rect region, PageSegMode? pageSegMode = null)
        {
            var pix = converter.ToPix(image);
            Page page = engine.Process(pix, inputName, region, pageSegMode);
            var _ = new TesseractEngine.PageDisposalHandle(page, pix);
            return page;
        }
    }
}