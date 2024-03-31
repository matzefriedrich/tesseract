namespace Tesseract.Rendering
{
    using System;
    using System.Runtime.InteropServices;

    using Abstractions;

    using Interop.Abstractions;

    using JetBrains.Annotations;using Tesseract.Abstractions;

    /// <summary>
    ///     Represents a native result renderer (e.g. text, pdf, etc).
    /// </summary>
    /// <remarks>
    ///     Note that the ResultRenderer is explicitly responsible for managing the renderer hierarchy. This gets around a number of difficult issues such as keeping track of what the next renderer is and how to manage the memory.
    /// </remarks>
    public abstract class ResultRenderer : DisposableBase, IResultRenderer
    {
        private readonly ITessApiSignatures native;
        protected HandleRef handle;
        
        protected ResultRenderer([NotNull] ITessApiSignatures native)
        {
            this.native = native ?? throw new ArgumentNullException(nameof(native));
        }

        protected void AssignHandle(IntPtr handle)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(handle);
            this.handle = new HandleRef(this, handle);
        }
        
        /// <summary>
        ///     Begins a new document with the specified <paramref name="title" />.
        /// </summary>
        /// <param name="title">The (ANSI) title of the new document.</param>
        /// <param name="rendererHandle">The handle of the renderer that owns the new document.</param>
        /// <returns>A handle that when disposed of ends the current document.</returns>
        public UnmanagedDocument BeginDocument(string title)
        {
            if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(title));
            
            var document = new ResultRendererDocument(this.native, this.handle, title);
            
            return document;
        }

        protected override void Dispose(bool disposing)
        {
            if (this.IsDisposed == false && disposing)
            {
                if (this.handle.Handle == IntPtr.Zero) return;
                this.native.DeleteResultRenderer(this.handle);
                this.handle = new HandleRef(null, IntPtr.Zero);
            }
        }
    }
}