namespace Tesseract
{
    using System;
    using Abstractions;
    using Interop.Abstractions;

    public sealed class PixColorMapFactory : IPixColorMapFactory
    {
        private readonly ILeptonicaApiSignatures leptonicaApi;

        public PixColorMapFactory(ILeptonicaApiSignatures leptonicaApi)
        {
            this.leptonicaApi = leptonicaApi ?? throw new ArgumentNullException(nameof(leptonicaApi));
        }

        public PixColormap Create(int depth)
        {
            if (depth is not (1 or 2 or 4 or 8)) throw new ArgumentOutOfRangeException(nameof(depth), Resources.Resources.PixColorMapFactory_CreateLinear_Depth_must_be_1__2__4__or_8_bpp_);

            IntPtr handle = this.leptonicaApi.pixcmapCreate(depth);
            if (handle == IntPtr.Zero) throw new InvalidOperationException("Failed to create color map.");
            return new PixColormap(this.leptonicaApi, handle);
        }

        public PixColormap CreateLinear(int depth, int levels)
        {
            if (depth is not (1 or 2 or 4 or 8)) throw new ArgumentOutOfRangeException(nameof(depth), Resources.Resources.PixColorMapFactory_CreateLinear_Depth_must_be_1__2__4__or_8_bpp_);
            if (levels < 2 || levels > 2 << depth)
                throw new ArgumentOutOfRangeException(nameof(levels), @"Depth must be 2 and 2^depth (inclusive).");

            IntPtr handle = this.leptonicaApi.pixcmapCreateLinear(depth, levels);
            if (handle == IntPtr.Zero) throw new InvalidOperationException("Failed to create color map.");
            return new PixColormap(this.leptonicaApi, handle);
        }

        public PixColormap CreateLinear(int depth, bool firstIsBlack, bool lastIsWhite)
        {
            if (depth is not (1 or 2 or 4 or 8)) throw new ArgumentOutOfRangeException(nameof(depth), Resources.Resources.PixColorMapFactory_CreateLinear_Depth_must_be_1__2__4__or_8_bpp_);

            IntPtr handle = this.leptonicaApi.pixcmapCreateRandom(depth, firstIsBlack ? 1 : 0, lastIsWhite ? 1 : 0);
            if (handle == IntPtr.Zero) throw new InvalidOperationException("Failed to create color map.");
            return new PixColormap(this.leptonicaApi, handle);
        }
    }
}