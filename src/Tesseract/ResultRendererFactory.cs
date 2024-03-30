namespace Tesseract
{
    using System;
    using System.Collections.Generic;

    using Abstractions;

    using Interop.Abstractions;

    using JetBrains.Annotations;

    using Rendering;

    public sealed class ResultRendererFactory : IResultRendererFactory
    {
        private readonly ITessApiSignatures native;

        public ResultRendererFactory([NotNull] ITessApiSignatures native)
        {
            this.native = native ?? throw new ArgumentNullException(nameof(native));
        }

        /// <summary>
        ///     Creates renderers for specified output formats.
        /// </summary>
        /// <param name="outputbase"></param>
        /// <param name="dataPath">The directory containing the pdf font data, normally same as your tessdata directory.</param>
        /// <param name="outputFormats"></param>
        /// <returns></returns>
        public IEnumerable<IResultRenderer> CreateRenderers(string outputbase, string dataPath, List<RenderedFormat> outputFormats)
        {
            var renderers = new List<IResultRenderer>();

            foreach (RenderedFormat format in outputFormats)
            {
                IResultRenderer renderer = null;

                switch (format)
                {
                    case RenderedFormat.TEXT:
                        renderer = this.CreateTextRenderer(outputbase);
                        break;
                    case RenderedFormat.HOCR:
                        renderer = this.CreateHOcrRenderer(outputbase);
                        break;
                    case RenderedFormat.PDF:
                    case RenderedFormat.PDF_TEXTONLY:
                        bool textonly = format == RenderedFormat.PDF_TEXTONLY;
                        renderer = this.CreatePdfRenderer(outputbase, dataPath, textonly);
                        break;
                    case RenderedFormat.BOX:
                        renderer = this.CreateBoxRenderer(outputbase);
                        break;
                    case RenderedFormat.UNLV:
                        renderer = this.CreateUnlvRenderer(outputbase);
                        break;
                    case RenderedFormat.ALTO:
                        renderer = this.CreateAltoRenderer(outputbase);
                        break;
                    case RenderedFormat.TSV:
                        renderer = this.CreateTsvRenderer(outputbase);
                        break;
                    case RenderedFormat.LSTMBOX:
                        renderer = this.CreateLSTMBoxRenderer(outputbase);
                        break;
                    case RenderedFormat.WORDSTRBOX:
                        renderer = this.CreateWordStrBoxRenderer(outputbase);
                        break;
                }

                renderers.Add(renderer);
            }

            return renderers;
        }

        /// <summary>
        ///     Creates a <see cref="IResultRenderer">result renderer</see> that render that generates a searchable
        ///     pdf file from tesseract's output.
        /// </summary>
        /// <param name="outputFilename">The filename of the pdf file to be generated without the file extension.</param>
        /// <param name="fontDirectory">The directory containing the pdf font data, normally same as your tessdata directory.</param>
        /// <param name="textonly">skip images if set</param>
        /// <returns></returns>
        public IResultRenderer CreatePdfRenderer(string outputFilename, string fontDirectory, bool textonly)
        {
            return new PdfResultRenderer(this.native, outputFilename, fontDirectory, textonly);
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
        public IResultRenderer CreateLSTMBoxRenderer(string outputFilename)
        {
            return new LSTMBoxResultRenderer(this.native, outputFilename);
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