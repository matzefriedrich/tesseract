namespace Tesseract.Abstractions
{
    using System.Runtime.Serialization;

    [Serializable]
    public class TesseractException : Exception, ISerializable
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