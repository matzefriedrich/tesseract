namespace Tesseract
{
    using System;
    using System.Runtime.InteropServices;

    using Abstractions;

    using Interop.Abstractions;

    using JetBrains.Annotations;

    /// <summary>
    ///     Class to iterate over the classifier choices for a single symbol.
    /// </summary>
    public sealed class ChoiceIterator : DisposableBase
    {
        private readonly HandleRef _handleRef;
        private readonly IManagedTesseractApi api;
        private readonly ITessApiSignatures nativeApi;

        internal ChoiceIterator([NotNull] IManagedTesseractApi api, [NotNull] ITessApiSignatures nativeApi, IntPtr handle)
        {
            // TODO: this component should not directly interact with the native API, otherwise it is an interop service and thus must be moved to the Interop assembly project

            this.api = api ?? throw new ArgumentNullException(nameof(api));
            this.nativeApi = nativeApi ?? throw new ArgumentNullException(nameof(nativeApi));
            this._handleRef = new HandleRef(this, handle);
        }

        /// <summary>
        ///     Moves to the next choice for the symbol and returns false if there are none left.
        /// </summary>
        /// <returns>true|false</returns>
        public bool Next()
        {
            this.ThrowIfDisposed();
            if (this._handleRef.Handle == IntPtr.Zero)
                return false;
            return this.nativeApi.ChoiceIteratorNext(this._handleRef) != 0;
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
            if (this._handleRef.Handle == IntPtr.Zero)
                return 0f;

            return this.nativeApi.ChoiceIteratorGetConfidence(this._handleRef);
        }

        /// <summary>
        ///     Returns the text string for the current choice.
        /// </summary>
        /// <returns>string</returns>
        public string GetText()
        {
            this.ThrowIfDisposed();
            if (this._handleRef.Handle == IntPtr.Zero)
                return string.Empty;

            return this.api.ChoiceIteratorGetUTF8Text(this._handleRef);
        }

        protected override void Dispose(bool disposing)
        {
            if (this._handleRef.Handle != IntPtr.Zero) this.nativeApi.ChoiceIteratorDelete(this._handleRef);
        }
    }
}