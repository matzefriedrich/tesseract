namespace Tesseract.Tests
{
    using System.Xml.Linq;
    using NUnit.Framework;

    public class XDocumentDifferenceHandler : ITestDifferenceHandler
    {
        public void Execute(string actualResultFilename, string expectedResultFilename)
        {
            XDocument left = XDocument.Load(actualResultFilename);
            XDocument right = XDocument.Load(expectedResultFilename);

            bool areEqual = XDocumentComparer.AreEqual(left, right);
            
            // Assert
            Assert.IsTrue(areEqual, "The documents are not equal.");
        }
    }
}