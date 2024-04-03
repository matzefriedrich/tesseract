namespace Tesseract.Tests.Leptonica.PixTests
{
    using Abstractions;
    using ImageProcessing;
    using ImageProcessing.Abstractions;
    using Interop.Abstractions;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;

    [TestFixture]
    public class NoiseRemoverTest : TesseractTestBase
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

        private const string ResultsDirectory = @"Results/ImageManipulation/";

        private readonly ServiceCollection services = new();
        private ServiceProvider? provider;

        [Test]
        public void Despeckle_ReduceSpeckleNoise_Test()
        {
            // Arrange
            var pixFactory = (this.provider ?? throw new InvalidOperationException()).GetRequiredService<IPixFactory>();
            var api = this.provider.GetRequiredService<ILeptonicaApiSignatures>();
            var writer = this.provider.GetRequiredService<IPixFileWriter>();

            string sourcePixFilename = MakeAbsoluteTestFilePath(@"processing/w91frag.jpg");
            using Pix sourcePix = pixFactory.LoadFromFile(sourcePixFilename);

            var sut = new NoiseRemover(api);

            // Act
            using Pix result = sut.ReduceSpeckleNoise(sourcePix, SelString.Str2, 2);

            // Assert
            // TODO: Visually confirm successful despeckle and then setup an assertion to compare that result is the same.
            this.SaveResult(writer, result, ResultsDirectory, "w91frag-despeckled.png");
        }
    }
}