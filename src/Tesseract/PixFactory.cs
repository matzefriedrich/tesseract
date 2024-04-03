namespace Tesseract
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Abstractions;
    using Interop.Abstractions;

    public sealed unsafe class PixFactory : IPixFactory
    {
        private static readonly List<int> AllowedDepths = [1, 2, 4, 8, 16, 32];
        private readonly ILeptonicaApiSignatures leptonicaApi;

        public PixFactory(ILeptonicaApiSignatures leptonicaApi)
        {
            this.leptonicaApi = leptonicaApi ?? throw new ArgumentNullException(nameof(leptonicaApi));
        }

        public Pix Create(int width, int height, int depth)
        {
            if (!AllowedDepths.Contains(depth))
                throw new ArgumentException(Resources.Resources.PixFactory_Create_Depth_must_be_1__2__4__8__16__or_32_bits_, nameof(depth));

            if (width <= 0) throw new ArgumentException(Resources.Resources.PixFactory_Create_Width_must_be_greater_than_zero, nameof(width));
            if (height <= 0) throw new ArgumentException(Resources.Resources.PixFactory_Create_Height_must_be_greater_than_zero, nameof(height));

            IntPtr handle = this.leptonicaApi.pixCreate(width, height, depth);
            if (handle == IntPtr.Zero) throw new InvalidOperationException("Failed to create pix, this normally occurs because the requested image size is too large, please check Standard Error Output.");

            return this.Create(handle);
        }

        public Pix Create(IntPtr handle)
        {
            if (handle == IntPtr.Zero) throw new ArgumentException(Resources.Resources.PixFactory_Create_Pix_handle_must_not_be_zero__null__, nameof(handle));

            return new Pix(this.leptonicaApi, handle);
        }

        public Pix LoadFromFile(string filename)
        {
            if (string.IsNullOrWhiteSpace(filename)) throw new ArgumentException(Resources.Resources.Value_cannot_be_null_or_whitespace, nameof(filename));
            IntPtr pixHandle = this.leptonicaApi.pixRead(filename);
            if (pixHandle == IntPtr.Zero) throw new IOException($"Failed to load image '{filename}'.");
            return this.Create(pixHandle);
        }

        public Pix LoadFromMemory(byte[] bytes)
        {
            ArgumentNullException.ThrowIfNull(bytes);

            IntPtr handle;
            fixed (byte* ptr = bytes)
            {
                handle = this.leptonicaApi.pixReadMem(ptr, bytes.Length);
            }

            if (handle == IntPtr.Zero) throw new IOException("Failed to load image from memory.");
            return this.Create(handle);
        }

        public Pix LoadTiffFromMemory(byte[] bytes)
        {
            ArgumentNullException.ThrowIfNull(bytes);

            IntPtr handle;
            fixed (byte* ptr = bytes)
            {
                handle = this.leptonicaApi.pixReadMemTiff(ptr, bytes.Length, 0);
            }

            if (handle == IntPtr.Zero) throw new IOException("Failed to load image from memory.");
            return this.Create(handle);
        }

        public Pix ReadFromMultiPageTiff(string filename, ref int offset)
        {
            if (string.IsNullOrWhiteSpace(filename)) throw new ArgumentException(Resources.Resources.Value_cannot_be_null_or_whitespace, nameof(filename));

            IntPtr handle = this.leptonicaApi.pixReadFromMultipageTiff(filename, ref offset);

            if (handle == IntPtr.Zero) throw new IOException($"Failed to load image from multi-page Tiff at offset {offset}.");
            return this.Create(handle);
        }

        /// <summary>
        ///     Increments reference count of the given <see cref="Pix" /> object and returns a new <see cref="Pix" /> instance with the same data.
        /// </summary>
        /// <remarks>
        ///     A "clone" is simply a reference to an existing pix. It is implemented this way because image can be large and hence expensive to copy and extra handles need to be made with a simple policy to avoid double frees and memory leaks. The general usage protocol is:
        ///     <list type="number">
        ///         <item>Whenever you want a new reference to an existing <see cref="Pix" /> call <see cref="PixFactory.Clone" />.</item>
        ///         <item>
        ///             Always call <see cref="Pix.Dispose" /> on all references. This decrements the reference count and will destroy the pix when the reference count reaches zero.
        ///         </item>
        ///     </list>
        /// </remarks>
        /// <returns>Returns a new <see cref="Pix" /> object with an incremented reference count.</returns>
        public Pix Clone(Pix source)
        {
            ArgumentNullException.ThrowIfNull(source);

            IntPtr clonedHandle = this.leptonicaApi.pixClone(source.Handle);
            return new Pix(this.leptonicaApi, clonedHandle);
        }
    }
}