namespace Tesseract.Rendering
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;
    using Abstractions;
    using Interop.Abstractions;
    using Resources;

    /// <summary>
    ///     Encapsulates a renderer handle and manages all memory and state of a result-renderer document.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public sealed class ResultRendererDocument : UnmanagedDocument
    {
        private readonly ITessApiSignatures native;
        private IntPtr titlePtr;

        internal ResultRendererDocument(
            ITessApiSignatures native,
            HandleRef rendererHandle,
            string title)
        {
            if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException(Resources.Value_cannot_be_null_or_whitespace, nameof(title));
            this.native = native ?? throw new ArgumentNullException(nameof(native));
            this.Title = title;
            this.Handle = rendererHandle;
            this.titlePtr = this.GetTitlePtr(title);
        }

        public string Title { get; }

        private IntPtr GetTitlePtr(string s)
        {
            IntPtr ptr = Marshal.StringToHGlobalAnsi(s);
            if (this.native.ResultRendererBeginDocument(this.Handle, ptr) != 0) return ptr;

            // release the pointer first before throwing an error.
            Marshal.FreeHGlobal(ptr);
            throw new InvalidOperationException($"Failed to begin document \"{s}\".");
        }

        private void FreeTitlePtr()
        {
            if (this.titlePtr == IntPtr.Zero) return;
            Marshal.FreeHGlobal(this.titlePtr);
            this.titlePtr = IntPtr.Zero;
        }

        /// <summary>
        ///     Add the page to the current document.
        /// </summary>
        /// <param name="page"></param>
        /// <returns><c>True</c> if the page was successfully added to the result renderer; otherwise false.</returns>
        public override bool AddPage(Page page)
        {
            ArgumentNullException.ThrowIfNull(page);
            this.ThrowIfDisposed();

            // TODO: Force page to do a recognise run to ensure the underlying base api is full of state note if
            // your implementing your own renderer you won't need to do this since all the page operations will do it
            // implicitly if required. This is why I've only made Page.Recognise internal not public.
            page.Recognize();

            int result = this.native.ResultRendererAddImage(this.Handle, page.Engine.Handle);
            return result != 0;
        }

        public override int GetPageNumber()
        {
            this.ThrowIfDisposed();
            return this.native.ResultRendererImageNum(this.Handle);
        }

        private void FinishDocument()
        {
            this.native.ResultRendererEndDocument(this.Handle);
            this.FreeTitlePtr();
        }

        protected override void Dispose(bool disposing)
        {
            if (this.IsDisposed == false && disposing)
            {
                this.FinishDocument();

                this.Handle = new HandleRef(this, IntPtr.Zero);
            }

            base.Dispose(disposing);
        }
    }
}