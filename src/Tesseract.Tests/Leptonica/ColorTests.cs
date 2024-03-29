namespace Tesseract.Tests.Leptonica
{
    using System.Drawing;

    using NUnit.Framework;

    [TestFixture]
    public class ColorTests
    {
        [TestCase]
        public void Color_CastColorToNetColor()
        {
            var color = new PixColor(100, 150, 200);
            var castColor = (Color)color;
            Assert.That(castColor.R, Is.EqualTo(color.Red));
            Assert.That(castColor.G, Is.EqualTo(color.Green));
            Assert.That(castColor.B, Is.EqualTo(color.Blue));
            Assert.That(castColor.A, Is.EqualTo(color.Alpha));
        }

        [TestCase]
        public void Color_ConvertColorToNetColor()
        {
            var color = new PixColor(100, 150, 200);
            var castColor = color.ToColor();
            Assert.That(castColor.R, Is.EqualTo(color.Red));
            Assert.That(castColor.G, Is.EqualTo(color.Green));
            Assert.That(castColor.B, Is.EqualTo(color.Blue));
            Assert.That(castColor.A, Is.EqualTo(color.Alpha));
        }

        [TestCase]
        public void Color_ConvertNetColorToColor()
        {
            Color color = Color.FromArgb(100, 150, 200);
            var castColor = color.ToPixColor();
            Assert.That(color.R, Is.EqualTo(castColor.Red));
            Assert.That(color.G, Is.EqualTo(castColor.Green));
            Assert.That(color.B, Is.EqualTo(castColor.Blue));
            Assert.That(color.A, Is.EqualTo(castColor.Alpha));
        }
    }
}