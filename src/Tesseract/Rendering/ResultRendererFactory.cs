namespace Tesseract.Rendering
{
    using System;
    using System.Collections.Generic;
    using Tesseract.Abstractions;
    using Tesseract.Interop.Abstractions;
    using Tesseract.Rendering.Abstractions;

    public sealed class ResultRendererFactory : IResultRendererFactory
    {
        private readonly ITessApiSignatures native;

        public ResultRendererFactory(ITessApiSignatures native)
        {
            this.native = native ?? throw new ArgumentNullException(nameof(native));
        }

        /// <summary>
        ///     Creates renderers for specified output formats.
        /// </summary>
        /// <param name="outputBase"></param>
        /// <param name="dataPath">The directory containing the pdf font data, normally same as your tessdata directory.</param>
        /// <param name="outputFormats"></param>
        /// <returns></returns>
        public IEnumerable<IResultRenderer> CreateRenderers(string outputBase, string dataPath, List<RenderedFormat> outputFormats)
        {
            var renderers = new List<IResultRenderer>();

            foreach (RenderedFormat format in outputFormats)
            {
                IResultRenderer renderer;

                switch (format)
                {
                    case RenderedFormat.Text:
                        renderer = this.CreateTextRenderer(outputBase);
                        break;
                    case RenderedFormat.Hocr:
                        renderer = this.CreateHOcrRenderer(outputBase);
                        break;
                    case RenderedFormat.Pdf:
                    case RenderedFormat.PdfTextOnly:
                        bool textOnly = format == RenderedFormat.PdfTextOnly;
                        renderer = this.CreatePdfRenderer(outputBase, dataPath, textOnly);
                        break;
                    case RenderedFormat.Box:
                        renderer = this.CreateBoxRenderer(outputBase);
                        break;
                    case RenderedFormat.Unlv:
                        renderer = this.CreateUnlvRenderer(outputBase);
                        break;
                    case RenderedFormat.Alto:
                        renderer = this.CreateAltoRenderer(outputBase);
                        break;
                    case RenderedFormat.Tsv:
                        renderer = this.CreateTsvRenderer(outputBase);
                        break;
                    case RenderedFormat.LstmBox:
                        renderer = this.CreateLstmBoxRenderer(outputBase);
                        break;
                    case RenderedFormat.WordStrBox:
                        renderer = this.CreateWordStrBoxRenderer(outputBase);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(outputFormats));
                }

                renderers.Add(renderer);
            }

            return renderers;
        }

        /// <summary>
        ///     Creates a <see cref="IResultRenderer">result renderer</see> that render that generates a searchable
        ///     pdf file from tesseract output.
        /// </summary>
        /// <param name="outputFilename">The filename of the pdf file to be generated without the file extension.</param>
        /// <param name="fontDirectory">The directory containing the pdf font data, normally same as your tessdata directory.</param>
        /// <param name="textOnly">skip images if set</param>
        /// <returns></returns>
        public IResultRenderer CreatePdfRenderer(string outputFilename, string fontDirectory, bool textOnly)
        {
            return new PdfResultRenderer(this.native, outputFilename, fontDirectory, textOnly);
        }

        /// <summary>
        ///     Creates a <see cref="IResultRenderer">result renderer</see> that render that generates UTF-8 encoded text
        ///     file from tesseract's output.
        /// </summary>
        /// <param name="outputFilename">The path to the text file to be generated without the file extension.</param>
        /// <returns></returns>
        public IResultRenderer CreateTextRenderer(string outputFilename)
        {
            return new TextResultRenderer(this.native, outputFilename);
        }

        /// <summary>
        ///     Creates a <see cref="IResultRenderer">result renderer</see> that render that generates a HOCR
        ///     file from tesseract's output.
        /// </summary>
        /// <param name="outputFilename">The path to the hocr file to be generated without the file extension.</param>
        /// <param name="fontInfo">Determines if the generated HOCR file includes font information or not.</param>
        /// <returns></returns>
        public IResultRenderer CreateHOcrRenderer(string outputFilename, bool fontInfo = false)
        {
            return new HOcrResultRenderer(this.native, outputFilename, fontInfo);
        }

        /// <summary>
        ///     Creates a <see cref="IResultRenderer">result renderer</see> that render that generates a unlv
        ///     file from tesseract's output.
        /// </summary>
        /// <param name="outputFilename">The path to the unlv file to be created without the file extension.</param>
        /// <returns></returns>
        public IResultRenderer CreateUnlvRenderer(string outputFilename)
        {
            return new UnlvResultRenderer(this.native, outputFilename);
        }

        /// <summary>
        ///     Creates a <see cref="IResultRenderer">result renderer</see> that render that generates an Alto
        ///     file from tesseract's output.
        /// </summary>
        /// <param name="outputFilename">The path to the Alto file to be created without the file extension.</param>
        /// <returns></returns>
        public IResultRenderer CreateAltoRenderer(string outputFilename)
        {
            return new AltoResultRenderer(this.native, outputFilename);
        }

        /// <summary>
        ///     Creates a <see cref="IResultRenderer">result renderer</see> that render that generates a Tsv
        ///     file from tesseract's output.
        /// </summary>
        /// <param name="outputFilename">The path to the Tsv file to be created without the file extension.</param>
        /// <returns></returns>
        public IResultRenderer CreateTsvRenderer(string outputFilename)
        {
            return new TsvResultRenderer(this.native, outputFilename);
        }

        /// <summary>
        ///     Creates a <see cref="IResultRenderer">result renderer</see> that render that generates a unlv
        ///     file from tesseract's output.
        /// </summary>
        /// <param name="outputFilename">The path to the unlv file to be created without the file extension.</param>
        /// <returns></returns>
        public IResultRenderer CreateLstmBoxRenderer(string outputFilename)
        {
            return new LstmBoxResultRenderer(this.native, outputFilename);
        }

        /// <summary>
        ///     Creates a <see cref="IResultRenderer">result renderer</see> that render that generates a unlv
        ///     file from tesseract's output.
        /// </summary>
        /// <param name="outputFilename">The path to the unlv file to be created without the file extension.</param>
        /// <returns></returns>
        public IResultRenderer CreateWordStrBoxRenderer(string outputFilename)
        {
            return new WordStrBoxResultRenderer(this.native, outputFilename);
        }

        /// <summary>
        ///     Creates a <see cref="IResultRenderer">result renderer</see> that render that generates a box text file from
        ///     tesseract's output.
        /// </summary>
        /// <param name="outputFilename">The path to the box file to be created without the file extension.</param>
        /// <returns></returns>
        public IResultRenderer CreateBoxRenderer(string outputFilename)
        {
            return new BoxResultRenderer(this.native, outputFilename);
        }
    }
}