namespace Tesseract
{
    using System;
    using System.Runtime.InteropServices;
    using Abstractions;
    using Interop.Abstractions;

    /// <summary>
    ///     Class to iterate over the classifier choices for a single symbol.
    /// </summary>
    public sealed class ChoiceIterator : DisposableBase
    {
        private readonly IManagedTesseractApi api;
        private readonly HandleRef handleRef;
        private readonly ITessApiSignatures nativeApi;

        internal ChoiceIterator(IManagedTesseractApi api, ITessApiSignatures nativeApi, IntPtr handle)
        {
            // TODO: this component should not directly interact with the native API, otherwise it is an interop service and thus must be moved to the Interop assembly project

            this.api = api ?? throw new ArgumentNullException(nameof(api));
            this.nativeApi = nativeApi ?? throw new ArgumentNullException(nameof(nativeApi));
            this.handleRef = new HandleRef(this, handle);
        }

        /// <summary>
        ///     Moves to the next choice for the symbol and returns false if there are none left.
        /// </summary>
        /// <returns>true|false</returns>
        public bool Next()
        {
            this.ThrowIfDisposed();
            if (this.handleRef.Handle == IntPtr.Zero)
                return false;
            return this.nativeApi.ChoiceIteratorNext(this.handleRef) != 0;
        }

        /// <summary>
        ///     Returns the confidence of the current choice.
        /// </summary>
        /// <remarks>
        ///     The number should be interpreted as a percent probability. (0.0f-100.0f)
        /// </remarks>
        /// <returns>float</returns>
        public float GetConfidence()
        {
            this.ThrowIfDisposed();
            if (this.handleRef.Handle == IntPtr.Zero)
                return 0f;

            return this.nativeApi.ChoiceIteratorGetConfidence(this.handleRef);
        }

        /// <summary>
        ///     Returns the text string for the current choice.
        /// </summary>
        /// <returns>string</returns>
        public string GetText()
        {
            this.ThrowIfDisposed();
            if (this.handleRef.Handle == IntPtr.Zero)
                return string.Empty;

            return this.api.ChoiceIteratorGetUtf8Text(this.handleRef) ?? string.Empty;
        }

        protected override void Dispose(bool disposing)
        {
            if (this.IsDisposed == false && disposing)
                if (this.handleRef.Handle != IntPtr.Zero)
                    this.nativeApi.ChoiceIteratorDelete(this.handleRef);

            base.Dispose(disposing);
        }
    }
}