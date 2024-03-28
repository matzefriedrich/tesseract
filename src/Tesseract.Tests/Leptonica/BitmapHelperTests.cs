namespace Tesseract.Tests.Leptonica
{
    using NUnit.Framework;

    [TestFixture]
    public class BitmapHelperTests
    {
        [Test]
        public void ConvertRgb555ToPixColor()
        {
            ushort originalVal = 0x39EC;
            uint convertedValue = BitmapHelper.ConvertRgb555ToRGBA(originalVal);
            Assert.That(convertedValue, Is.EqualTo(0x737B63FF));
        }

        [Test]
        [TestCase(0xB9EC, 0x737B63FF)]
        [TestCase(0x39EC, 0x737B6300)]
        public void ConvertArgb555ToPixColor(int originalVal, int expectedVal)
        {
            uint convertedValue = BitmapHelper.ConvertArgb1555ToRGBA((ushort)originalVal);
            Assert.That(convertedValue, Is.EqualTo((uint)expectedVal));
        }

        [Test]
        public void ConvertRgb565ToPixColor()
        {
            ushort originalVal = 0x73CC;
            uint convertedValue = BitmapHelper.ConvertRgb565ToRGBA(originalVal);
            Assert.That(convertedValue, Is.EqualTo(0x737963FF));
        }
    }
}