namespace Tesseract.Abstractions
{
    using System.Collections.Generic;

    using Rendering.Abstractions;

    public interface IResultRendererFactory
    {
        /// <summary>
        ///     Creates renderers for specified output formats.
        /// </summary>
        /// <param name="outputbase"></param>
        /// <param name="dataPath">The directory containing the pdf font data, normally same as your tessdata directory.</param>
        /// <param name="outputFormats"></param>
        /// <returns></returns>
        IEnumerable<IResultRenderer> CreateRenderers(string outputbase, string dataPath, List<RenderedFormat> outputFormats);

        /// <summary>
        ///     Creates a <see cref="IResultRenderer">result renderer</see> that render that generates a searchable
        ///     pdf file from tesseract's output.
        /// </summary>
        /// <param name="outputFilename">The filename of the pdf file to be generated without the file extension.</param>
        /// <param name="fontDirectory">The directory containing the pdf font data, normally same as your tessdata directory.</param>
        /// <param name="textonly">skip images if set</param>
        /// <returns></returns>
        IResultRenderer CreatePdfRenderer(string outputFilename, string fontDirectory, bool textonly);

        /// <summary>
        ///     Creates a <see cref="IResultRenderer">result renderer</see> that render that generates UTF-8 encoded text
        ///     file from tesseract's output.
        /// </summary>
        /// <param name="outputFilename">The path to the text file to be generated without the file extension.</param>
        /// <returns></returns>
        IResultRenderer CreateTextRenderer(string outputFilename);

        /// <summary>
        ///     Creates a <see cref="IResultRenderer">result renderer</see> that render that generates a HOCR
        ///     file from tesseract's output.
        /// </summary>
        /// <param name="outputFilename">The path to the hocr file to be generated without the file extension.</param>
        /// <param name="fontInfo">Determines if the generated HOCR file includes font information or not.</param>
        /// <returns></returns>
        IResultRenderer CreateHOcrRenderer(string outputFilename, bool fontInfo = false);

        /// <summary>
        ///     Creates a <see cref="IResultRenderer">result renderer</see> that render that generates a unlv
        ///     file from tesseract's output.
        /// </summary>
        /// <param name="outputFilename">The path to the unlv file to be created without the file extension.</param>
        /// <returns></returns>
        IResultRenderer CreateUnlvRenderer(string outputFilename);

        /// <summary>
        ///     Creates a <see cref="IResultRenderer">result renderer</see> that render that generates an Alto
        ///     file from tesseract's output.
        /// </summary>
        /// <param name="outputFilename">The path to the Alto file to be created without the file extension.</param>
        /// <returns></returns>
        IResultRenderer CreateAltoRenderer(string outputFilename);

        /// <summary>
        ///     Creates a <see cref="IResultRenderer">result renderer</see> that render that generates a Tsv
        ///     file from tesseract's output.
        /// </summary>
        /// <param name="outputFilename">The path to the Tsv file to be created without the file extension.</param>
        /// <returns></returns>
        IResultRenderer CreateTsvRenderer(string outputFilename);

        /// <summary>
        ///     Creates a <see cref="IResultRenderer">result renderer</see> that render that generates a unlv
        ///     file from tesseract's output.
        /// </summary>
        /// <param name="outputFilename">The path to the unlv file to be created without the file extension.</param>
        /// <returns></returns>
        IResultRenderer CreateLSTMBoxRenderer(string outputFilename);

        /// <summary>
        ///     Creates a <see cref="IResultRenderer">result renderer</see> that render that generates a unlv
        ///     file from tesseract's output.
        /// </summary>
        /// <param name="outputFilename">The path to the unlv file to be created without the file extension.</param>
        /// <returns></returns>
        IResultRenderer CreateWordStrBoxRenderer(string outputFilename);

        /// <summary>
        ///     Creates a <see cref="IResultRenderer">result renderer</see> that render that generates a box text file from
        ///     tesseract's output.
        /// </summary>
        /// <param name="outputFilename">The path to the box file to be created without the file extension.</param>
        /// <returns></returns>
        IResultRenderer CreateBoxRenderer(string outputFilename);
    }
}