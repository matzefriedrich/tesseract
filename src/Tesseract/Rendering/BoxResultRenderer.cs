namespace Tesseract.Rendering
{
    using System;

    using Interop.Abstractions;

    public sealed class BoxResultRenderer : ResultRenderer
    {
        public BoxResultRenderer(ITessApiSignatures native, string outputFilename) : base(native)
        {
            IntPtr rendererHandle = native.BoxTextRendererCreate(outputFilename);
            this.Initialise(rendererHandle);
        }
    }
}