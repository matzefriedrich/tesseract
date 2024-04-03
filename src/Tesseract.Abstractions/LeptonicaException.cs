namespace Tesseract.Abstractions
{
    using JetBrains.Annotations;

    [Serializable]
    public class LeptonicaException : Exception
    {
        public LeptonicaException()
        {
        }

        public LeptonicaException([LocalizationRequired(true)] string message) : base(message)
        {
        }

        public LeptonicaException([LocalizationRequired(true)] string message, Exception inner) : base(message, inner)
        {
        }
    }
}