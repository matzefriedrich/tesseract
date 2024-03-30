namespace Tesseract.Rendering
{
    using System;

    using Interop.Abstractions;

    public sealed class AltoResultRenderer : ResultRenderer
    {
        public AltoResultRenderer(ITessApiSignatures native, string outputFilename) : base(native)
        {
            IntPtr rendererHandle = native.AltoRendererCreate(outputFilename);
            this.Initialise(rendererHandle);
        }
    }
}