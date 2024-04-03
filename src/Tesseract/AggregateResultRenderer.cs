namespace Tesseract
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Abstractions;
    using Internal;
    using Rendering;
    using Rendering.Abstractions;

    /// <summary>
    ///     Aggregate result renderer.
    /// </summary>
    public class AggregateResultRenderer : DisposableBase
    {
        private readonly IEnumerable<IResultRenderer> resultRenderers;

        /// <summary>
        ///     Create a new aggregate result renderer with the specified child result renderers.
        /// </summary>
        /// <param name="resultRenderers">The child result renderers.</param>
        public AggregateResultRenderer(IEnumerable<IResultRenderer> resultRenderers)
        {
            if (resultRenderers == null) throw new ArgumentNullException(nameof(resultRenderers));

            this.resultRenderers = new ReadOnlyCollection<IResultRenderer>(resultRenderers.ToList());
        }

        /// <summary>
        ///     Begins a new document with the specified title.
        /// </summary>
        /// <param name="title">The title of the document.</param>
        /// <returns></returns>
        public Document BeginDocument(string title)
        {
            if (title == null) throw new ArgumentException(Resources.Resources.Value_cannot_be_null_or_whitespace, nameof(title));

            this.ThrowIfDisposed();

            var document = new Document();

            try
            {
                foreach (IResultRenderer renderer in this.resultRenderers)
                {
                    UnmanagedDocument child = renderer.BeginDocument(title);
                    document.AddChild(child);
                }
            }
            catch
            {
                try
                {
                    document.Dispose();
                }
                catch (Exception disposalError)
                {
                    Logger.TraceError("Failed to dispose the document {0}", document, disposalError.Message);
                }

                throw;
            }

            return document;
        }
    }
}