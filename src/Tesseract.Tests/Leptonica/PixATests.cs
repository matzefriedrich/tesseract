namespace Tesseract.Tests.Leptonica.PixTests
{
    using NUnit.Framework;

    [TestFixture]
    public class PixATests : TesseractTestBase
    {
        [Test]
        public void CanCreatePixArray()
        {
            using (var pixA = PixArray.Create(0))
            {
                Assert.That(pixA.Count, Is.EqualTo(0));
            }
        }

        [Test]
        public void CanAddPixToPixArray()
        {
            string sourcePixPath = TestFilePath(@"Ocr/phototest.tif");
            using (var pixA = PixArray.Create(0))
            {
                using (Pix sourcePix = Pix.LoadFromFile(sourcePixPath))
                {
                    pixA.Add(sourcePix);
                    Assert.That(pixA.Count, Is.EqualTo(1));
                    using (Pix targetPix = pixA.GetPix(0))
                    {
                        Assert.That(targetPix, Is.EqualTo(sourcePix));
                    }
                }
            }
        }

        [Test]
        public void CanRemovePixFromArray()
        {
            string sourcePixPath = TestFilePath(@"Ocr/phototest.tif");
            using (var pixA = PixArray.Create(0))
            {
                using (Pix sourcePix = Pix.LoadFromFile(sourcePixPath))
                {
                    pixA.Add(sourcePix);
                }

                pixA.Remove(0);
                Assert.That(pixA.Count, Is.EqualTo(0));
            }
        }

        [Test]
        public void CanClearPixArray()
        {
            string sourcePixPath = TestFilePath(@"Ocr/phototest.tif");
            using (var pixA = PixArray.Create(0))
            {
                using (Pix sourcePix = Pix.LoadFromFile(sourcePixPath))
                {
                    pixA.Add(sourcePix);
                }

                pixA.Clear();

                Assert.That(pixA.Count, Is.EqualTo(0));
            }
        }
    }
}