namespace Tesseract.Rendering
{
    using System;
    using System.Runtime.InteropServices;

    using Abstractions;

    using Interop.Abstractions;

    using JetBrains.Annotations;

    /// <summary>
    ///     Represents a native result renderer (e.g. text, pdf, etc).
    /// </summary>
    /// <remarks>
    ///     Note that the ResultRenderer is explicitly responsible for managing the renderer hierarchy. This gets around a
    ///     number of difficult issues such as keeping track of what the next renderer is and how to manage the memory.
    /// </remarks>
    public abstract class ResultRenderer : DisposableBase, IResultRenderer
    {
        private readonly ITessApiSignatures native;
        private IDisposable _currentDocumentHandle;

        private HandleRef _handle;

        protected ResultRenderer([NotNull] ITessApiSignatures native)
        {
            this.native = native ?? throw new ArgumentNullException(nameof(native));
            this._handle = new HandleRef(this, IntPtr.Zero);
        }

        protected HandleRef Handle => this._handle;

        /// <summary>
        ///     Add the page to the current document.
        /// </summary>
        /// <param name="page"></param>
        /// <returns><c>True</c> if the page was successfully added to the result renderer; otherwise false.</returns>
        public bool AddPage(Page page)
        {
            ArgumentNullException.ThrowIfNull(page);
            this.ThrowIfDisposed();

            // TODO: Force page to do a recognise run to ensure the underlying base api is full of state note if
            // your implementing your own renderer you won't need to do this since all the page operations will do it
            // implicitly if required. This is why I've only made Page.Recognise internal not public.
            page.Recognize();

            return this.native.ResultRendererAddImage(this.Handle, page.Engine.Handle) != 0;
        }

        /// <summary>
        ///     Begins a new document with the specified <paramref name="title" />.
        /// </summary>
        /// <param name="title">The (ANSI) title of the new document.</param>
        /// <returns>A handle that when disposed of ends the current document.</returns>
        public IDisposable BeginDocument(string title)
        {
            if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(title));

            this.ThrowIfDisposed();
            if (this._currentDocumentHandle != null) throw new InvalidOperationException($"Cannot begin document \"{title}\" as another document is currently being processed which must be dispose off first.");

            IntPtr titlePtr = Marshal.StringToHGlobalAnsi(title);
            if (this.native.ResultRendererBeginDocument(this.Handle, titlePtr) == 0)
            {
                // release the pointer first before throwing an error.
                Marshal.FreeHGlobal(titlePtr);

                throw new InvalidOperationException($"Failed to begin document \"{title}\".");
            }

            this._currentDocumentHandle = new EndDocumentOnDispose(this.native, this, titlePtr);
            return this._currentDocumentHandle;
        }

        public int PageNumber
        {
            get
            {
                this.ThrowIfDisposed();

                return this.native.ResultRendererImageNum(this.Handle);
            }
        }

        /// <summary>
        ///     Initialise the render to use the specified native result renderer.
        /// </summary>
        /// <param name="handle"></param>
        protected void Initialise(IntPtr handle)
        {
            if (handle == IntPtr.Zero) throw new ArgumentException("handle must be initialised.", nameof(handle));
            if (this._handle.Handle != IntPtr.Zero) throw new InvalidOperationException("Result renderer has already been initialised.");

            this._handle = new HandleRef(this, handle);
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
                if (this._handle.Handle != IntPtr.Zero)
                {
                    this.native.DeleteResultRenderer(this._handle);
                    this._handle = new HandleRef(this, IntPtr.Zero);
                }
            }
        }

        /// <summary>
        ///     Ensures the renderer's EndDocument when disposed off.
        /// </summary>
        private class EndDocumentOnDispose : DisposableBase
        {
            private readonly ResultRenderer _renderer;
            private readonly ITessApiSignatures native;
            private IntPtr _titlePtr;

            public EndDocumentOnDispose([NotNull] ITessApiSignatures native, [NotNull] ResultRenderer renderer, IntPtr titlePtr)
            {
                this.native = native ?? throw new ArgumentNullException(nameof(native));
                this._renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
                this._titlePtr = titlePtr;
            }

            protected override void Dispose(bool disposing)
            {
                try
                {
                    if (disposing)
                    {
                        if (this._renderer._currentDocumentHandle != this) throw new InvalidOperationException("Expected the Result Render's active document to be this document.");

                        // End the renderer
                        this.native.ResultRendererEndDocument(this._renderer._handle);
                        this._renderer._currentDocumentHandle = null;
                    }
                }
                finally
                {
                    // free title ptr
                    if (this._titlePtr != IntPtr.Zero)
                    {
                        Marshal.FreeHGlobal(this._titlePtr);
                        this._titlePtr = IntPtr.Zero;
                    }
                }
            }
        }
    }
}