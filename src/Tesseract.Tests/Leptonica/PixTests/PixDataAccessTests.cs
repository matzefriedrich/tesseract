namespace Tesseract.Tests.Leptonica.PixTests
{
    using Abstractions;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;

    [TestFixture]
    public unsafe class DataAccessTests
    {
        [SetUp]
        public void Init()
        {
            this.provider = this.services.AddTesseract().BuildServiceProvider();
        }

        [TearDown]
        public void Teardown()
        {
            this.provider?.Dispose();
        }

        private readonly ServiceCollection services = new();
        private ServiceProvider? provider;

        private const int Width = 59, Height = 53;

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(4)]
        [TestCase(8)]
        [TestCase(16)]
        [TestCase(32)]
        public void CanReadAndWriteData(int depth)
        {
            // Arrange
            var pixFactory = (this.provider ?? throw new InvalidOperationException()).GetRequiredService<IPixFactory>();

            using Pix pix = pixFactory.Create(Width, Height, depth);

            // Act
            PixData pixData = pix.GetData();

            for (var y = 0; y < Height; y++)
            {
                uint* line = (uint*)pixData.Data + y * pixData.WordsPerLine;
                for (var x = 0; x < Width; x++)
                {
                    var val = (uint)((y * Width + x) % (1 << depth));
                    uint readVal;
                    if (depth == 1)
                    {
                        PixData.SetDataBit(line, x, val);
                        readVal = PixData.GetDataBit(line, x);
                    }
                    else if (depth == 2)
                    {
                        PixData.SetDataDiBit(line, x, val);
                        readVal = PixData.GetDataDiBit(line, x);
                    }
                    else if (depth == 4)
                    {
                        PixData.SetDataQBit(line, x, val);
                        readVal = PixData.GetDataQBit(line, x);
                    }
                    else if (depth == 8)
                    {
                        PixData.SetDataByte(line, x, val);
                        readVal = PixData.GetDataByte(line, x);
                    }
                    else if (depth == 16)
                    {
                        PixData.SetDataTwoByte(line, x, val);
                        readVal = PixData.GetDataTwoByte(line, x);
                    }
                    else if (depth == 32)
                    {
                        PixData.SetDataFourByte(line, x, val);
                        readVal = PixData.GetDataFourByte(line, x);
                    }
                    else
                    {
                        throw new NotSupportedException();
                    }

                    // Assert
                    Assert.That(readVal, Is.EqualTo(val));
                }
            }
        }
    }
}