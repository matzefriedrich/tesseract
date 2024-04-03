namespace Tesseract.Abstractions
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using Interop;

    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public static class PageFactoryBitmapExtensions
    {
        /// <summary>
        ///     Creates a new <see cref="Page" /> object from the given <see cref="Bitmap" /> object.
        /// </summary>
        /// <remarks>
        ///     Please consider <see cref="TesseractEngine.Process(Pix, string, Rect, PageSegMode?)" /> instead. This is because this method must convert the bitmap to a pix for processing which will add additional overhead. Leptonica also supports a
        ///     large number of image pre-processing functions as well.
        /// </remarks>
        /// <param name="factory">An <see cref="IPageFactory" /> object that is used to create the new <see cref="Page" /> object from the converted bitmap.</param>
        /// <param name="converter">An <see cref="IPixConverter" /> object that is used convert <see cref="Bitmap" /> objects into <see cref="Pix" /> objects.</param>
        /// <param name="image">The image to process.</param>
        /// <returns></returns>
        public static Page CreatePageFromBitmap(this IPageFactory factory, IPixConverter converter, Bitmap image, Action<PageBuilder>? pageBuilder = null)
        {
            ArgumentNullException.ThrowIfNull(factory);
            ArgumentNullException.ThrowIfNull(image);

            var pix = converter.ToPix(image);
            return factory.CreatePage(pix, pageBuilder);
        }
    }
}