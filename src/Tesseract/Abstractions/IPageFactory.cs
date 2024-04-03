namespace Tesseract.Abstractions
{
    using System;

    public interface IPageFactory
    {
        /// <summary>
        ///     Creates a new <see cref="Page" /> object representing a region within an image, and a layout analysis mode.
        /// </summary>
        /// <param name="image">The image to analyse.</param>
        /// <param name="configurePage">A function delegate representing a method that configures the analysis parameters, such as the region of interest, analysis mode, and engine options.</param>
        /// <returns></returns>
        Page CreatePage(Pix image, Action<PageBuilder>? configurePage = null);
    }
}