namespace Tesseract.Rendering.Abstractions
{
    using System;

    public interface IResultRenderer : IDisposable
    {
        /// <summary>
        ///     Begins a new document with the specified <paramref name="title" />.
        /// </summary>
        /// <param name="title">The title of the new document.</param>
        /// <returns>Returns a <see cref="UnmanagedDocument" /> object representing the result document.</returns>
        UnmanagedDocument BeginDocument(string title);
    }
}