namespace Tesseract.Abstractions
{
    using System.Diagnostics.CodeAnalysis;
    using JetBrains.Annotations;

    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class TesseractException : Exception
    {
        public TesseractException()
        {
        }

        public TesseractException([LocalizationRequired(true)] string message) : base(message)
        {
        }

        public TesseractException([LocalizationRequired(true)] string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}