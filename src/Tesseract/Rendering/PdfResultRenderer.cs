namespace Tesseract.Rendering
{
    using System;
    using System.Runtime.InteropServices;
    using Interop.Abstractions;
    using JetBrains.Annotations;

    public sealed class PdfResultRenderer : ResultRenderer
    {
        private readonly IntPtr fontDirectoryHandle;

        public PdfResultRenderer(ITessApiSignatures native, [NotNull] string outputFilename, [NotNull] string fontDirectory, bool isTextOnly) : base(native)
        {
            if (string.IsNullOrWhiteSpace(outputFilename)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(outputFilename));
            if (string.IsNullOrWhiteSpace(fontDirectory)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(fontDirectory));

            this.fontDirectoryHandle = Marshal.StringToHGlobalAnsi(fontDirectory);

            int textOnly = isTextOnly ? 1 : 0;
            IntPtr handle = native.PDFRendererCreate(outputFilename, this.fontDirectoryHandle, textOnly);
            this.AssignHandle(handle);
        }

        protected override void Dispose(bool disposing)
        {
            if (this.IsDisposed == false && disposing) this.FreeDirectoryHandle();
            base.Dispose(disposing);
        }

        private void FreeDirectoryHandle()
        {
            if (this.fontDirectoryHandle != IntPtr.Zero) Marshal.FreeHGlobal(this.fontDirectoryHandle);
        }
    }
}