namespace Tesseract.Rendering
{
    using System;
    using System.Runtime.InteropServices;

    using Interop.Abstractions;

    public sealed class PdfResultRenderer : ResultRenderer
    {
        private IntPtr _fontDirectoryHandle;

        public PdfResultRenderer(ITessApiSignatures native, string outputFilename, string fontDirectory, bool textonly) : base(native)
        {
            IntPtr fontDirectoryHandle = Marshal.StringToHGlobalAnsi(fontDirectory);
            IntPtr rendererHandle = native.PDFRendererCreate(outputFilename, fontDirectoryHandle, textonly ? 1 : 0);

            this.Initialise(rendererHandle);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            // dispose of font
            if (this._fontDirectoryHandle != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(this._fontDirectoryHandle);
                this._fontDirectoryHandle = IntPtr.Zero;
            }
        }
    }
}