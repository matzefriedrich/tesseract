#if NETFULL

namespace Tesseract.Tests.Leptonica
{
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using NUnit.Framework;

    [TestFixture]
    [Ignore("Performance tests are disabled by default, theres probably a better way of doing this but for now it's ok")]
    public class LeptonicaPerformanceTests
    {
        [Test]
        public void ConvertToBitmap()
        {
            const double BaseRunTime = 793.382;
            const int Runs = 1000;

            string sourceFilePath = Path.Combine("./Data/Conversion", "photo_palette_8bpp.tif");
            using (var bmp = new Bitmap(sourceFilePath))
            {
                // Don't include the first conversion since it will also handle loading the library etc (upfront costs).
                using (var pix = PixConverter.ToPix(bmp))
                {
                }

                // copy 100 times take the average
                var watch = new Stopwatch();
                watch.Start();
                for (var i = 0; i < Runs; i++)
                    using (var pix = PixConverter.ToPix(bmp))
                    {
                    }

                watch.Stop();

                double delta = watch.ElapsedTicks / (BaseRunTime * Runs);
                Console.WriteLine("Delta: {0}", delta);
                Console.WriteLine("Elapsed Ticks: {0}", watch.ElapsedTicks);
                Console.WriteLine("Elapsed Time: {0}ms", watch.ElapsedMilliseconds);
                Console.WriteLine("Average Time: {0}ms", (double)watch.ElapsedMilliseconds / Runs);

                Assert.That(delta, Is.EqualTo(1.0).Within(0.25));
            }
        }
    }
}

#endif