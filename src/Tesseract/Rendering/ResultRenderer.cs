namespace Tesseract.Rendering
{
    using System;
    using System.Runtime.InteropServices;
    using Abstractions;
    using Interop.Abstractions;
    using Resources;
    using Tesseract.Abstractions;

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
        private HandleRef handleRef;

        protected ResultRenderer(ITessApiSignatures native)
        {
            this.native = native ?? throw new ArgumentNullException(nameof(native));
        }

        /// <summary>
        ///     Begins a new document with the specified <paramref name="title" />.
        /// </summary>
        /// <param name="title">The (ANSI) title of the new document.</param>
        /// <returns>A handle that when disposed of ends the current document.</returns>
        public UnmanagedDocument BeginDocument(string title)
        {
            if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException(Resources.Value_cannot_be_null_or_whitespace, nameof(title));

            var document = new ResultRendererDocument(this.native, this.handleRef, title);

            return document;
        }

        /// <summary>
        ///     Assigns a renderer handle to the current <see cref="ResultRenderer" /> object.
        /// </summary>
        /// <param name="handle">The handle of the renderer that owns the new document.</param>
        protected void AssignHandle(IntPtr handle)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(handle);
            this.handleRef = new HandleRef(this, handle);
        }

        protected override void Dispose(bool disposing)
        {
            if (this.IsDisposed == false && disposing)
            {
                if (this.handleRef.Handle == IntPtr.Zero) return;
                this.native.DeleteResultRenderer(this.handleRef);
                this.handleRef = new HandleRef(null, IntPtr.Zero);
            }

            base.Dispose(disposing);
        }
    }
}