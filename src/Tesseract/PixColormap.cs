namespace Tesseract
{
    using System;
    using System.Runtime.InteropServices;

    using Abstractions;

    using Interop.Abstractions;

    using JetBrains.Annotations;

    /// <summary>
    ///     Represents a colormap.
    /// </summary>
    /// <remarks>
    ///     Once the colormap is assigned to a pix it is owned by that pix and will be disposed off automatically
    ///     when the pix is disposed off.
    /// </remarks>
    public sealed class PixColormap : IDisposable
    {
        private readonly ILeptonicaApiSignatures leptonicaApi;

        internal PixColormap([NotNull] ILeptonicaApiSignatures leptonicaApi, IntPtr handle)
        {
            this.leptonicaApi = leptonicaApi ?? throw new ArgumentNullException(nameof(leptonicaApi));
            this.Handle = new HandleRef(this, handle);
        }

        internal HandleRef Handle { get; private set; }

        public int Depth => this.leptonicaApi.pixcmapGetDepth(this.Handle);

        public int Count => this.leptonicaApi.pixcmapGetCount(this.Handle);

        public int FreeCount => this.leptonicaApi.pixcmapGetFreeCount(this.Handle);

        public PixColor this[int index]
        {
            get
            {
                if (this.leptonicaApi.pixcmapGetColor32(this.Handle, index, out int color) == 0)
                    return PixColor.FromRgb((uint)color);
                throw new InvalidOperationException("Failed to retrieve color.");
            }
            set
            {
                int result = this.leptonicaApi.pixcmapResetColor(this.Handle, index, value.Red, value.Green, value.Blue);
                if (result != 0) throw new InvalidOperationException("Failed to reset color.");
            }
        }

        public void Dispose()
        {
            IntPtr tmpHandle = this.Handle.Handle;
            this.leptonicaApi.pixcmapDestroy(ref tmpHandle);
            this.Handle = new HandleRef(this, IntPtr.Zero);
        }

        public bool AddColor(PixColor color)
        {
            return this.leptonicaApi.pixcmapAddColor(this.Handle, color.Red, color.Green, color.Blue) == 0;
        }

        public bool AddNewColor(PixColor color, out int index)
        {
            return this.leptonicaApi.pixcmapAddNewColor(this.Handle, color.Red, color.Green, color.Blue, out index) == 0;
        }

        public bool AddNearestColor(PixColor color, out int index)
        {
            return this.leptonicaApi.pixcmapAddNearestColor(this.Handle, color.Red, color.Green, color.Blue, out index) == 0;
        }

        public bool AddBlackOrWhite(int color, out int index)
        {
            return this.leptonicaApi.pixcmapAddBlackOrWhite(this.Handle, color, out index) == 0;
        }

        public bool SetBlackOrWhite(bool setBlack, bool setWhite)
        {
            return this.leptonicaApi.pixcmapSetBlackAndWhite(this.Handle, setBlack ? 1 : 0, setWhite ? 1 : 0) == 0;
        }

        public bool IsUsableColor(PixColor color)
        {
            int result = this.leptonicaApi.pixcmapUsableColor(this.Handle, color.Red, color.Green, color.Blue, out int usable);
            if (result == 0)
                return usable == 1;

            throw new InvalidOperationException("Failed to detect if color was usable or not.");
        }

        public void Clear()
        {
            int result = this.leptonicaApi.pixcmapClear(this.Handle);
            if (result != 0) throw new InvalidOperationException("Failed to clear color map.");
        }
    }
}