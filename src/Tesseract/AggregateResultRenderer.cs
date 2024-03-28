namespace Tesseract
{
    using System;
    using System.Collections.Generic;
    using Internal;

    /// <summary>
    ///     Aggregate result renderer.
    /// </summary>
    public class AggregateResultRenderer : DisposableBase, IResultRenderer
    {
        private IDisposable _currentDocumentHandle;

        private List<IResultRenderer> _resultRenderers;

        /// <summary>
        ///     Create a new aggregate result renderer with the specified child result renderers.
        /// </summary>
        /// <param name="resultRenderers">The child result renderers.</param>
        public AggregateResultRenderer(params IResultRenderer[] resultRenderers)
            : this((IEnumerable<IResultRenderer>)resultRenderers)
        {
        }

        /// <summary>
        ///     Create a new aggregate result renderer with the specified child result renderers.
        /// </summary>
        /// <param name="resultRenderers">The child result renderers.</param>
        public AggregateResultRenderer(IEnumerable<IResultRenderer> resultRenderers)
        {
            if (resultRenderers == null) throw new ArgumentNullException(nameof(resultRenderers));
            this._resultRenderers = new List<IResultRenderer>(resultRenderers);
        }

        /// <summary>
        ///     Get's the child result renderers.
        /// </summary>
        public IEnumerable<IResultRenderer> ResultRenderers => this._resultRenderers;

        /// <summary>
        ///     Get's the current page number.
        /// </summary>
        public int PageNumber { get; private set; } = -1;

        /// <summary>
        ///     Adds a page to each of the child result renderers.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public bool AddPage(Page page)
        {
            if (page == null) throw new ArgumentNullException(nameof(page));
            this.VerifyNotDisposed();

            this.PageNumber++;
            foreach (IResultRenderer renderer in this.ResultRenderers)
                if (!renderer.AddPage(page))
                    return false;

            return true;
        }

        /// <summary>
        ///     Begins a new document with the specified title.
        /// </summary>
        /// <param name="title">The title of the document.</param>
        /// <returns></returns>
        public IDisposable BeginDocument(string title)
        {
            if (title == null) throw new ArgumentException("Value cannot be null or whitespace.", nameof(title));

            this.VerifyNotDisposed();
            if (this._currentDocumentHandle != null) throw new InvalidOperationException($"Cannot begin document \"{title}\" as another document is currently being processed which must be dispose off first.");

            // Reset the page numer
            this.PageNumber = -1;

            // Begin the document on each child renderer.
            var children = new List<IDisposable>();
            try
            {
                foreach (IResultRenderer renderer in this.ResultRenderers) children.Add(renderer.BeginDocument(title));

                this._currentDocumentHandle = new EndDocumentOnDispose(this, children);
                return this._currentDocumentHandle;
            }
            catch
            {
                // Dispose of all previously created child document's iff an error occured to prevent a memory leak.
                foreach (IDisposable child in children)
                    try
                    {
                        child.Dispose();
                    }
                    catch (Exception disposalError)
                    {
                        Logger.TraceError("Failed to dispose of child document {0}: {1}", child, disposalError.Message);
                    }

                throw;
            }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                    // Ensure that if the renderer has an active document when disposed it too is disposed off.
                    if (this._currentDocumentHandle != null)
                    {
                        this._currentDocumentHandle.Dispose();
                        this._currentDocumentHandle = null;
                    }
            }
            finally
            {
                // dispose of result renderers
                foreach (IResultRenderer renderer in this.ResultRenderers) renderer.Dispose();
                this._resultRenderers = null;
            }
        }

        /// <summary>
        ///     Ensures the renderer's EndDocument when disposed off.
        /// </summary>
        private class EndDocumentOnDispose : DisposableBase
        {
            private readonly AggregateResultRenderer _renderer;
            private List<IDisposable> _children;

            public EndDocumentOnDispose(AggregateResultRenderer renderer, IEnumerable<IDisposable> children)
            {
                this._renderer = renderer;
                this._children = new List<IDisposable>(children);
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    if (this._renderer._currentDocumentHandle != this) throw new InvalidOperationException("Expected the Result Render's active document to be this document.");

                    // End the renderer
                    foreach (IDisposable child in this._children) child.Dispose();
                    this._children = null;

                    // reset current handle
                    this._renderer._currentDocumentHandle = null;
                }
            }
        }
    }
}