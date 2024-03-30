namespace Tesseract.Rendering
{
    using System;

    using Interop.Abstractions;

    public sealed class UnlvResultRenderer : ResultRenderer
    {
        public UnlvResultRenderer(ITessApiSignatures native, string outputFilename) : base(native)
        {
            IntPtr rendererHandle = native.UnlvRendererCreate(outputFilename);
            this.Initialise(rendererHandle);
        }
    }
}