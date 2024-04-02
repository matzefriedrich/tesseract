namespace Tesseract
{
    using System;

    using Abstractions;

    using Interop.Abstractions;

    using JetBrains.Annotations;

    public sealed class PixColorMapFactory : IPixColorMapFactory
    {
        private readonly ILeptonicaApiSignatures leptonicaApi;

        public PixColorMapFactory([NotNull] ILeptonicaApiSignatures leptonicaApi)
        {
            this.leptonicaApi = leptonicaApi ?? throw new ArgumentNullException(nameof(leptonicaApi));
        }

        public PixColormap Create(int depth)
        {
            if (!(depth == 1 || depth == 2 || depth == 4 || depth == 8)) throw new ArgumentOutOfRangeException(nameof(depth), "Depth must be 1, 2, 4, or 8 bpp.");

            IntPtr handle = this.leptonicaApi.pixcmapCreate(depth);
            if (handle == IntPtr.Zero) throw new InvalidOperationException("Failed to create colormap.");
            return new PixColormap(this.leptonicaApi, handle);
        }

        public PixColormap CreateLinear(int depth, int levels)
        {
            if (!(depth == 1 || depth == 2 || depth == 4 || depth == 8)) throw new ArgumentOutOfRangeException(nameof(depth), "Depth must be 1, 2, 4, or 8 bpp.");
            if (levels < 2 || levels > 2 << depth)
                throw new ArgumentOutOfRangeException(nameof(levels), "Depth must be 2 and 2^depth (inclusive).");

            IntPtr handle = this.leptonicaApi.pixcmapCreateLinear(depth, levels);
            if (handle == IntPtr.Zero) throw new InvalidOperationException("Failed to create colormap.");
            return new PixColormap(this.leptonicaApi, handle);
        }

        public PixColormap CreateLinear(int depth, bool firstIsBlack, bool lastIsWhite)
        {
            if (!(depth == 1 || depth == 2 || depth == 4 || depth == 8)) throw new ArgumentOutOfRangeException(nameof(depth), "Depth must be 1, 2, 4, or 8 bpp.");

            IntPtr handle = this.leptonicaApi.pixcmapCreateRandom(depth, firstIsBlack ? 1 : 0, lastIsWhite ? 1 : 0);
            if (handle == IntPtr.Zero) throw new InvalidOperationException("Failed to create colormap.");
            return new PixColormap(this.leptonicaApi, handle);
        }
    }
}