namespace Tesseract.Rendering.Abstractions
{
    using System.Runtime.InteropServices;

    using Tesseract.Abstractions;

    /// <summary>
    ///     A base class for result-renderer documents.
    /// </summary>
    public abstract class UnmanagedDocument : DisposableBase
    {
        /// <summary>
        ///     Stores the renderer handle who owns the current document.
        /// </summary>
        protected HandleRef handle;

        public abstract bool AddPage(Page page);
        public abstract int GetPageNumber();
    }
}