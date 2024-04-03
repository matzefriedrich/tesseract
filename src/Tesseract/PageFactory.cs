namespace Tesseract
{
    using System;
    using Abstractions;
    using Interop;
    using Interop.Abstractions;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Provides functionality to create a new <see cref="Page"/> instances attached to a new <see cref="ITesseractEngine"/>.
    /// </summary>
    public sealed class PageFactory : DisposableBase, IPageFactory
    {
        private readonly IManagedTesseractApi api;
        private readonly ITesseractEngineFactory engineFactory;
        private readonly ILeptonicaApiSignatures leptonicaNativeApi;
        private readonly ILoggerFactory loggerFactory;
        private readonly ITessApiSignatures native;
        private readonly IPixFactory pixFactory;
        private readonly IPixFileWriter pixFileWriter;

        public PageFactory(
            ITesseractEngineFactory engineFactory,
            IManagedTesseractApi api,
            ITessApiSignatures native,
            ILeptonicaApiSignatures leptonicaNativeApi,
            IPixFactory pixFactory,
            IPixFileWriter pixFileWriter,
            ILoggerFactory loggerFactory)
        {
            this.engineFactory = engineFactory ?? throw new ArgumentNullException(nameof(engineFactory));
            this.api = api ?? throw new ArgumentNullException(nameof(api));
            this.native = native ?? throw new ArgumentNullException(nameof(native));
            this.leptonicaNativeApi = leptonicaNativeApi ?? throw new ArgumentNullException(nameof(leptonicaNativeApi));
            this.pixFactory = pixFactory ?? throw new ArgumentNullException(nameof(pixFactory));
            this.pixFileWriter = pixFileWriter ?? throw new ArgumentNullException(nameof(pixFileWriter));
            this.loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        public Page CreatePage(Pix image, Action<PageBuilder>? configurePage = null)
        {
            var builder = new PageBuilder(image);
            configurePage?.Invoke(builder);

            PageBuilder.CreatePageParams pageConfiguration = builder.BuildPageConfiguration();
            return this.CreatePage(
                pageConfiguration.Image,
                pageConfiguration.InputName, 
                pageConfiguration.Region, 
                pageConfiguration.PageSegMode, 
                pageConfiguration.EngineOptionBuilder);
        }

        private Page CreatePage(Pix image, string? inputName, Rect region, PageSegMode? pageSegMode = null, Action<EngineOptionBuilder>? engineBuilder = null)
        {
            ArgumentNullException.ThrowIfNull(image);
            if (region.X1 < 0 || region.Y1 < 0 || region.X2 > image.Width || region.Y2 > image.Height)
                throw new ArgumentException(Resources.Resources.TesseractEngine_Process_The_image_region_to_be_processed_must_be_within_the_image_bounds_, nameof(region));

            ITesseractEngine engine = this.engineFactory.CreateEngine(engineBuilder);
            
            PageSegMode actualPageSegmentMode = pageSegMode ?? engine.DefaultPageSegMode;
            this.native.SetPageSegMode(engine.Handle, actualPageSegmentMode);
            this.native.SetImage(engine.Handle, image.Handle);
            if (!string.IsNullOrEmpty(inputName))
                this.native.SetInputName(engine.Handle, inputName);

            ILogger<Page> pageLogger = this.loggerFactory.CreateLogger<Page>();
            return new Page(engine,
                this.api, this.native, this.leptonicaNativeApi,
                this.pixFactory, this.pixFileWriter,
                pageLogger, image, inputName, region,
                actualPageSegmentMode);
        }
    }
}