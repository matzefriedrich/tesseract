namespace Tesseract
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Abstractions;

    using Interop.Abstractions;

    using JetBrains.Annotations;

    public sealed unsafe class PixFactory : IPixFactory
    {
        private static readonly List<int> AllowedDepths = [1, 2, 4, 8, 16, 32];
        private readonly ILeptonicaApiSignatures leptonicaApi;

        public PixFactory([NotNull] ILeptonicaApiSignatures leptonicaApi)
        {
            this.leptonicaApi = leptonicaApi ?? throw new ArgumentNullException(nameof(leptonicaApi));
        }

        public Pix Create(int width, int height, int depth)
        {
            if (!AllowedDepths.Contains(depth))
                throw new ArgumentException("Depth must be 1, 2, 4, 8, 16, or 32 bits.", nameof(depth));

            if (width <= 0) throw new ArgumentException("Width must be greater than zero", nameof(width));
            if (height <= 0) throw new ArgumentException("Height must be greater than zero", nameof(height));

            IntPtr handle = this.leptonicaApi.pixCreate(width, height, depth);
            if (handle == IntPtr.Zero) throw new InvalidOperationException("Failed to create pix, this normally occurs because the requested image size is too large, please check Standard Error Output.");

            return this.Create(handle);
        }

        public Pix Create(IntPtr handle)
        {
            if (handle == IntPtr.Zero) throw new ArgumentException("Pix handle must not be zero (null).", "handle");

            return new Pix(this.leptonicaApi, handle);
        }

        public Pix LoadFromFile(string filename)
        {
            IntPtr pixHandle = this.leptonicaApi.pixRead(filename);
            if (pixHandle == IntPtr.Zero) throw new IOException($"Failed to load image '{filename}'.");
            return this.Create(pixHandle);
        }

        public Pix LoadFromMemory(byte[] bytes)
        {
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
            IntPtr handle;
            fixed (byte* ptr = bytes)
            {
                handle = this.leptonicaApi.pixReadMemTiff(ptr, bytes.Length, 0);
            }

            if (handle == IntPtr.Zero) throw new IOException("Failed to load image from memory.");
            return this.Create(handle);
        }

        public Pix pixReadFromMultipageTiff(string filename, ref int offset)
        {
            IntPtr handle = this.leptonicaApi.pixReadFromMultipageTiff(filename, ref offset);

            if (handle == IntPtr.Zero) throw new IOException($"Failed to load image from multi-page Tiff at offset {offset}.");
            return this.Create(handle);
        }
    }
}