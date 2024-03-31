namespace Tesseract.Rendering
{
    using System;
    using System.Runtime.InteropServices;

    using Abstractions;

    using Interop.Abstractions;

    using JetBrains.Annotations;

    /// <summary>
    ///     Encapsulates a renderer handle and manages all memory and state of a result-renderer document.
    /// </summary>
    public sealed class ResultRendererDocument : UnmanagedDocument
    {
        private readonly ITessApiSignatures native;
        private IntPtr titlePtr;

        internal ResultRendererDocument(
            [NotNull] ITessApiSignatures native,
            HandleRef rendererHandle,
            [NotNull] string title)
        {
            if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(title));
            this.native = native ?? throw new ArgumentNullException(nameof(native));
            this.Title = title;
            this.handle = rendererHandle;
            this.titlePtr = this.GetTitlePtr(title);
        }

        public string Title { get; }

        private IntPtr GetTitlePtr(string s)
        {
            IntPtr titlePtr = Marshal.StringToHGlobalAnsi(s);
            if (this.native.ResultRendererBeginDocument(this.handle, titlePtr) == 0)
            {
                // release the pointer first before throwing an error.
                Marshal.FreeHGlobal(titlePtr);
                throw new InvalidOperationException($"Failed to begin document \"{s}\".");
            }

            return titlePtr;
        }

        private void FreeTitlePtr()
        {
            if (this.titlePtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(this.titlePtr);
                this.titlePtr = IntPtr.Zero;
            }
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

            int result = this.native.ResultRendererAddImage(this.handle, page.Engine.Handle);
            return result != 0;
        }

        public override int GetPageNumber()
        {
            this.ThrowIfDisposed();
            return this.native.ResultRendererImageNum(this.handle);
        }

        private void FinishDocument()
        {
            this.native.ResultRendererEndDocument(this.handle);
            this.FreeTitlePtr();
        }

        protected override void Dispose(bool disposing)
        {
            if (this.IsDisposed == false && disposing)
            {
                this.FinishDocument();

                this.handle = new HandleRef(this, IntPtr.Zero);
            }
        }
    }
}