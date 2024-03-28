namespace Tesseract
{
    using System;
    using System.Runtime.InteropServices;
    using Interop;

    /// <summary>
    ///     Represents a colormap.
    /// </summary>
    /// <remarks>
    ///     Once the colormap is assigned to a pix it is owned by that pix and will be disposed off automatically
    ///     when the pix is disposed off.
    /// </remarks>
    public sealed class PixColormap : IDisposable
    {
        internal PixColormap(IntPtr handle)
        {
            this.Handle = new HandleRef(this, handle);
        }

        internal HandleRef Handle { get; private set; }

        public int Depth => LeptonicaApi.Native.pixcmapGetDepth(this.Handle);

        public int Count => LeptonicaApi.Native.pixcmapGetCount(this.Handle);

        public int FreeCount => LeptonicaApi.Native.pixcmapGetFreeCount(this.Handle);

        public PixColor this[int index]
        {
            get
            {
                int color;
                if (LeptonicaApi.Native.pixcmapGetColor32(this.Handle, index, out color) == 0)
                    return PixColor.FromRgb((uint)color);
                throw new InvalidOperationException("Failed to retrieve color.");
            }
            set
            {
                if (LeptonicaApi.Native.pixcmapResetColor(this.Handle, index, value.Red, value.Green, value.Blue) != 0) throw new InvalidOperationException("Failed to reset color.");
            }
        }

        public void Dispose()
        {
            IntPtr tmpHandle = this.Handle.Handle;
            LeptonicaApi.Native.pixcmapDestroy(ref tmpHandle);
            this.Handle = new HandleRef(this, IntPtr.Zero);
        }

        public static PixColormap Create(int depth)
        {
            if (!(depth == 1 || depth == 2 || depth == 4 || depth == 8)) throw new ArgumentOutOfRangeException("depth", "Depth must be 1, 2, 4, or 8 bpp.");

            IntPtr handle = LeptonicaApi.Native.pixcmapCreate(depth);
            if (handle == IntPtr.Zero) throw new InvalidOperationException("Failed to create colormap.");
            return new PixColormap(handle);
        }

        public static PixColormap CreateLinear(int depth, int levels)
        {
            if (!(depth == 1 || depth == 2 || depth == 4 || depth == 8)) throw new ArgumentOutOfRangeException("depth", "Depth must be 1, 2, 4, or 8 bpp.");
            if (levels < 2 || levels > 2 << depth)
                throw new ArgumentOutOfRangeException("levels", "Depth must be 2 and 2^depth (inclusive).");

            IntPtr handle = LeptonicaApi.Native.pixcmapCreateLinear(depth, levels);
            if (handle == IntPtr.Zero) throw new InvalidOperationException("Failed to create colormap.");
            return new PixColormap(handle);
        }

        public static PixColormap CreateLinear(int depth, bool firstIsBlack, bool lastIsWhite)
        {
            if (!(depth == 1 || depth == 2 || depth == 4 || depth == 8)) throw new ArgumentOutOfRangeException("depth", "Depth must be 1, 2, 4, or 8 bpp.");

            IntPtr handle = LeptonicaApi.Native.pixcmapCreateRandom(depth, firstIsBlack ? 1 : 0, lastIsWhite ? 1 : 0);
            if (handle == IntPtr.Zero) throw new InvalidOperationException("Failed to create colormap.");
            return new PixColormap(handle);
        }

        public bool AddColor(PixColor color)
        {
            return LeptonicaApi.Native.pixcmapAddColor(this.Handle, color.Red, color.Green, color.Blue) == 0;
        }

        public bool AddNewColor(PixColor color, out int index)
        {
            return LeptonicaApi.Native.pixcmapAddNewColor(this.Handle, color.Red, color.Green, color.Blue, out index) == 0;
        }

        public bool AddNearestColor(PixColor color, out int index)
        {
            return LeptonicaApi.Native.pixcmapAddNearestColor(this.Handle, color.Red, color.Green, color.Blue, out index) == 0;
        }

        public bool AddBlackOrWhite(int color, out int index)
        {
            return LeptonicaApi.Native.pixcmapAddBlackOrWhite(this.Handle, color, out index) == 0;
        }

        public bool SetBlackOrWhite(bool setBlack, bool setWhite)
        {
            return LeptonicaApi.Native.pixcmapSetBlackAndWhite(this.Handle, setBlack ? 1 : 0, setWhite ? 1 : 0) == 0;
        }

        public bool IsUsableColor(PixColor color)
        {
            int usable;
            if (LeptonicaApi.Native.pixcmapUsableColor(this.Handle, color.Red, color.Green, color.Blue, out usable) == 0)
                return usable == 1;
            throw new InvalidOperationException("Failed to detect if color was usable or not.");
        }

        public void Clear()
        {
            if (LeptonicaApi.Native.pixcmapClear(this.Handle) != 0) throw new InvalidOperationException("Failed to clear color map.");
        }
    }
}