namespace Tesseract.Rendering
{
    using System;

    using Interop.Abstractions;

    public sealed class TsvResultRenderer : ResultRenderer
    {
        public TsvResultRenderer(ITessApiSignatures native, string outputFilename) : base(native)
        {
            IntPtr rendererHandle = native.TsvRendererCreate(outputFilename);
            this.Initialise(rendererHandle);
        }
    }
}