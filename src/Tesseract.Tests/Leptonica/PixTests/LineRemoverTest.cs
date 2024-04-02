namespace Tesseract.Tests.Leptonica.PixTests
{
    using Abstractions;
    using ImageProcessing;
    using Interop.Abstractions;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;

    [TestFixture]
    public class LineRemoverTest : TesseractTestBase
    {
        [SetUp]
        public void Init()
        {
            this.services.AddTesseract();
            this.provider = this.services.BuildServiceProvider();
        }

        [TearDown]
        public void Teardown()
        {
            this.provider?.Dispose();
        }

        private readonly ServiceCollection services = new();
        private ServiceProvider? provider;
        private const string ResultsDirectory = @"Results/ImageManipulation/";

        [Test]
        public void RemoveLinesTest()
        {
            // Arrange
            var pixFactory = (this.provider ?? throw new InvalidOperationException()).GetRequiredService<IPixFactory>();
            var api = this.provider.GetRequiredService<ILeptonicaApiSignatures>();
            var writer = this.provider.GetRequiredService<IPixFileWriter>();

            var sut = new LineRemover(api);

            string sourcePixFilename = MakeAbsoluteTestFilePath(@"processing/table.png");
            using Pix sourcePix = pixFactory.LoadFromFile(sourcePixFilename);

            var rotator = new ImageRotator(api, pixFactory);

            // Act
            using Pix result = sut.RemoveHorizontalLines(sourcePix);
            using Pix result1 = rotator.RotateImage90(result, RotateDirection.Clockwise);
            using Pix result2 = sut.RemoveHorizontalLines(result1);
            using Pix result3 = rotator.RotateImage90(result2, RotateDirection.CounterClockwise);

            // Assert

            // TODO: Visually confirm successful rotation and then setup an assertion to compare that result is the same.
            this.SaveResult(writer, result3, ResultsDirectory, "tableBordersRemoved.png");
        }
    }
}