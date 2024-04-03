namespace Tesseract.Abstractions
{
    using System;
    using System.Runtime.InteropServices;
    using Interop;

    public interface ITesseractEngine : IDisposable
    {
        HandleRef Handle { get; }
        
        string? Version { get; }

        /// <summary>
        ///     Gets or sets default <see cref="PageSegMode" /> mode used by
        ///     <see cref="Tesseract.TesseractEngine.Process(Pix, Rect, PageSegMode?)" />.
        /// </summary>
        PageSegMode DefaultPageSegMode { get; set; }
        
        bool SetDebugVariable(string name, string value);

        /// <summary>
        ///     Sets the value of a string variable.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="value">The new value of the variable.</param>
        /// <returns>Returns <c>True</c> if successful; otherwise <c>False</c>.</returns>
        bool SetVariable(string name, string value);

        /// <summary>
        ///     Sets the value of a boolean variable.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="value">The new value of the variable.</param>
        /// <returns>Returns <c>True</c> if successful; otherwise <c>False</c>.</returns>
        bool SetVariable(string name, bool value);

        /// <summary>
        ///     Sets the value of a integer variable.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="value">The new value of the variable.</param>
        /// <returns>Returns <c>True</c> if successful; otherwise <c>False</c>.</returns>
        bool SetVariable(string name, int value);

        /// <summary>
        ///     Sets the value of a double variable.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="value">The new value of the variable.</param>
        /// <returns>Returns <c>True</c> if successful; otherwise <c>False</c>.</returns>
        bool SetVariable(string name, double value);

        /// <summary>
        ///     Attempts to retrieve the value for a boolean variable.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="value">The current value of the variable.</param>
        /// <returns>Returns <c>True</c> if successful; otherwise <c>False</c>.</returns>
        bool TryGetBoolVariable(string name, out bool value);

        /// <summary>
        ///     Attempts to retrieve the value for a double variable.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="value">The current value of the variable.</param>
        /// <returns>Returns <c>True</c> if successful; otherwise <c>False</c>.</returns>
        bool TryGetDoubleVariable(string name, out double value);

        /// <summary>
        ///     Attempts to retrieve the value for an integer variable.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="value">The current value of the variable.</param>
        /// <returns>Returns <c>True</c> if successful; otherwise <c>False</c>.</returns>
        bool TryGetIntVariable(string name, out int value);

        /// <summary>
        ///     Attempts to retrieve the value for a string variable.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="value">The current value of the variable.</param>
        /// <returns>Returns <c>True</c> if successful; otherwise <c>False</c>.</returns>
        bool TryGetStringVariable(string name, out string? value);

        /// <summary>
        ///     Attempts to print the variables to the file.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        bool TryPrintVariablesToFile(string filename);
    }
}