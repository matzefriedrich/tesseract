namespace Tesseract
{
    using System;
    using System.Runtime.InteropServices;
    using Abstractions;
    using Interop.Abstractions;

    public sealed class Pix : DisposableBase, IEquatable<Pix>
    {
        private readonly ILeptonicaApiSignatures leptonicaApi;

        private PixColormap colormap;
        private HandleRef handle;

        /// <summary>
        ///     Creates a new pix instance using an existing handle to a pix structure.
        /// </summary>
        /// <remarks>
        ///     Note that the resulting instance takes ownership of the data structure.
        /// </remarks>
        /// <param name="leptonicaApi">
        ///     An <see cref="ILeptonicaApiSignatures" /> object that provides access to the native
        ///     Leptonica API.
        /// </param>
        /// <param name="handle"></param>
        internal Pix(ILeptonicaApiSignatures leptonicaApi, IntPtr handle)
        {
            if (handle == IntPtr.Zero) throw new ArgumentNullException(nameof(handle));
            this.leptonicaApi = leptonicaApi;

            this.handle = new HandleRef(this, handle);

            // TODO: this code should go into the PixFactory 
            this.Width = this.leptonicaApi.pixGetWidth(this.handle);
            this.Height = this.leptonicaApi.pixGetHeight(this.handle);
            this.Depth = this.leptonicaApi.pixGetDepth(this.handle);

            IntPtr colorMapHandle = this.leptonicaApi.pixGetColormap(this.handle);
            if (colorMapHandle != IntPtr.Zero) this.colormap = new PixColormap(this.leptonicaApi, colorMapHandle);
        }

        public PixColormap Colormap
        {
            get => this.colormap;
            set
            {
                if (value != null)
                {
                    if (this.leptonicaApi.pixSetColormap(this.handle, value.Handle) == 0) this.colormap = value;
                }
                else
                {
                    if (this.leptonicaApi.pixDestroyColormap(this.handle) == 0) this.colormap = null;
                }
            }
        }

        public int Depth { get; }

        public int Height { get; }

        public int Width { get; }

        public int XRes
        {
            get => this.leptonicaApi.pixGetXRes(this.handle);
            set => this.leptonicaApi.pixSetXRes(this.handle, value);
        }

        public int YRes
        {
            get => this.leptonicaApi.pixGetYRes(this.handle);
            set => this.leptonicaApi.pixSetYRes(this.handle, value);
        }

        internal HandleRef Handle => this.handle;

        public bool Equals(Pix other)
        {
            if (other == null) return false;

            int pixEqual = this.leptonicaApi.pixEqual(this.Handle, other.Handle, out int same);
            if (pixEqual != 0)
                throw new TesseractException("Failed to compare pix");

            return same != 0;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.colormap, this.handle);
        }

        protected override void Dispose(bool disposing)
        {
            if (this.IsDisposed == false && disposing)
            {
                IntPtr tmpHandle = this.handle.Handle;
                this.leptonicaApi.pixDestroy(ref tmpHandle);
                this.handle = new HandleRef(this, IntPtr.Zero);
            }
            
            base.Dispose(disposing);
        }

        public PixData GetData()
        {
            return new PixData(this.leptonicaApi, this);
        }

        public override bool Equals(object obj)
        {
            // Check for null values and compare run-time types.
            if (obj == null || this.GetType() != obj.GetType())
                return false;

            return this.Equals((Pix)obj);
        }
    }
}