namespace Tesseract.Tests.Leptonica.PixTests
{
    using Abstractions;
    using ImageProcessing;
    using Interop.Abstractions;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;

    [TestFixture]
    public class SkewCorrectorTest : TesseractTestBase
    {
        private const string ResultsDirectory = @"Results/ImageManipulation/";

        private readonly ServiceCollection services = new();
        private ServiceProvider? provider;

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

        [Test]
        public void DescewTest()
        {
            // Arrange
            var pixFactory = (this.provider ?? throw new InvalidOperationException()).GetRequiredService<IPixFactory>();
            var api = this.provider.GetRequiredService<ILeptonicaApiSignatures>();
            var writer = this.provider.GetRequiredService<IPixFileWriter>();

            string sourcePixPath = MakeAbsoluteTestFilePath(@"Scew/scewed-phototest.png");
            using Pix sourcePix = pixFactory.LoadFromFile(sourcePixPath);

            var sut = new SkewCorrector(api);

            // Act
            using Pix result = sut.DeskewImage(sourcePix, new ScewSweep(range: 45), DeskewExtensions.DefaultBinarySearchReduction, DeskewExtensions.DefaultBinaryThreshold, out Scew scew);

            // Assert
            Assert.That(scew.Angle, Is.EqualTo(-9.953125F).Within(0.00001));
            Assert.That(scew.Confidence, Is.EqualTo(3.782913F).Within(0.00001));

            this.SaveResult(writer, result, ResultsDirectory, "descewedImage.png");
        }
    }
}