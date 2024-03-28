namespace Tesseract
{
    using System;
    using System.Runtime.InteropServices;
    using Interop;

    /// <summary>
    ///     Class to iterate over the classifier choices for a single symbol.
    /// </summary>
    public sealed class ChoiceIterator : DisposableBase
    {
        private readonly HandleRef _handleRef;

        internal ChoiceIterator(IntPtr handle)
        {
            this._handleRef = new HandleRef(this, handle);
        }

        /// <summary>
        ///     Moves to the next choice for the symbol and returns false if there are none left.
        /// </summary>
        /// <returns>true|false</returns>
        public bool Next()
        {
            this.VerifyNotDisposed();
            if (this._handleRef.Handle == IntPtr.Zero)
                return false;
            return TessApi.Native.ChoiceIteratorNext(this._handleRef) != 0;
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
            this.VerifyNotDisposed();
            if (this._handleRef.Handle == IntPtr.Zero)
                return 0f;

            return TessApi.Native.ChoiceIteratorGetConfidence(this._handleRef);
        }

        /// <summary>
        ///     Returns the text string for the current choice.
        /// </summary>
        /// <returns>string</returns>
        public string GetText()
        {
            this.VerifyNotDisposed();
            if (this._handleRef.Handle == IntPtr.Zero)
                return string.Empty;

            return TessApi.ChoiceIteratorGetUTF8Text(this._handleRef);
        }

        protected override void Dispose(bool disposing)
        {
            if (this._handleRef.Handle != IntPtr.Zero) TessApi.Native.ChoiceIteratorDelete(this._handleRef);
        }
    }
}