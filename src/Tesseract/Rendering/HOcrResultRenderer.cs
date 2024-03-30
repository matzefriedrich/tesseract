namespace Tesseract.Rendering
{
    using System;

    using Interop.Abstractions;

    public sealed class HOcrResultRenderer : ResultRenderer
    {
        public HOcrResultRenderer(ITessApiSignatures native, string outputFilename, bool fontInfo = false) : base(native)
        {
            IntPtr rendererHandle = native.HOcrRendererCreate2(outputFilename, fontInfo ? 1 : 0);
            this.Initialise(rendererHandle);
        }
    }
}