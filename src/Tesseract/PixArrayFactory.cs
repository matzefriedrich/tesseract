namespace Tesseract
{
    using System;
    using System.IO;
    using Abstractions;
    using Interop.Abstractions;

    public sealed class PixArrayFactory : IPixArrayFactory
    {
        private readonly ILeptonicaApiSignatures leptonicaApi;
        private readonly IPixFactory pixFactory;

        public PixArrayFactory(
            ILeptonicaApiSignatures leptonicaApi,
            IPixFactory pixFactory)
        {
            this.leptonicaApi = leptonicaApi ?? throw new ArgumentNullException(nameof(leptonicaApi));
            this.pixFactory = pixFactory ?? throw new ArgumentNullException(nameof(pixFactory));
        }

        /// <summary>
        ///     Loads the multi-page tiff located at <paramref name="filename" />.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public PixArray LoadMultiPageTiffFromFile(string filename)
        {
            if (string.IsNullOrWhiteSpace(filename)) throw new ArgumentException(Resources.Resources.Value_cannot_be_null_or_whitespace, nameof(filename));

            IntPtr pixaHandle = this.leptonicaApi.pixaReadMultipageTiff(filename);
            if (pixaHandle == IntPtr.Zero) throw new IOException($"Failed to load image '{filename}'.");

            return new PixArray(this.leptonicaApi, this.pixFactory, pixaHandle);
        }

        public PixArray Create(int n)
        {
            IntPtr pixaHandle = this.leptonicaApi.pixaCreate(n);
            if (pixaHandle == IntPtr.Zero) throw new IOException("Failed to create PixArray");

            return new PixArray(this.leptonicaApi, this.pixFactory, pixaHandle);
        }
    }
}