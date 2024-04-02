namespace Tesseract.Abstractions
{
    public class TesseractException : Exception
    {
        public TesseractException()
        {
        }

        public TesseractException(string message) : base(message)
        {
        }

        public TesseractException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}