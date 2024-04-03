namespace Tesseract
{
    using System;
    using Abstractions;
    using Interop;

    public class PageBuilder
    {
        private readonly Pix image;
        private Action<EngineOptionBuilder>? engineOptionBuilder;
        private string? inputName;
        private PageSegMode? pageSegMode;
        private Rect region;

        /// <summary>
        ///     Creates a new <see cref="PageBuilder" /> object.
        /// </summary>
        /// <param name="image">The image to analyze.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public PageBuilder(Pix image)
        {
            this.image = image ?? throw new ArgumentNullException(nameof(image));
        }

        /// <summary>
        ///     Sets the region within the image.
        /// </summary>
        /// <param name="region">A <see cref="Rect" /> value indicating the bounds of the region.</param>
        /// <returns></returns>
        public PageBuilder Region(Rect region)
        {
            this.region = region;
            return this;
        }

        /// <summary>
        ///     Sets the input file's name, only needed for training or loading a uzn file.
        /// </summary>
        /// <param name="name">The informational name of the input image. The parameter can be null.</param>
        /// <returns></returns>
        public PageBuilder WithInputName(string? name = null)
        {
            this.inputName = name;
            return this;
        }

        /// <summary>
        ///     Sets a method that is called to configure engine options.
        /// </summary>
        /// <param name="engineOptionBuilder">A function delegate representing a method that will be called to request engine options. This parameter can be null.</param>
        /// <returns></returns>
        public PageBuilder WithEngineOptions(Action<EngineOptionBuilder>? engineOptionBuilder = null)
        {
            this.engineOptionBuilder = engineOptionBuilder;
            return this;
        }

        /// <summary>
        ///     The page layout analysis method to use. This parameter can be null.
        /// </summary>
        /// <param name="pageSegMode"></param>
        /// <returns></returns>
        public PageBuilder WithAnalysisMode(PageSegMode? pageSegMode = null)
        {
            this.pageSegMode = pageSegMode;
            return this;
        }

        public virtual CreatePageParams BuildPageConfiguration()
        {
            return new CreatePageParams(this.image, this.inputName, this.region, this.pageSegMode, this.engineOptionBuilder);
        }

        public sealed class CreatePageParams
        {
            public CreatePageParams(Pix image, string? inputName, Rect? region, PageSegMode? pageSegMode, Action<EngineOptionBuilder>? engineOptionBuilder)
            {
                int imageWidth = image.Width;
                int imageHeight = image.Height;
                Rect r = region ?? new Rect(0, 0, imageWidth, imageHeight);
                
                if (r.X1 < 0 || r.Y1 < 0 || r.X2 > imageWidth || r.Y2 > imageHeight)
                    throw new ArgumentException(Resources.Resources.TesseractEngine_Process_The_image_region_to_be_processed_must_be_within_the_image_bounds_, nameof(region));

                this.Image = image;
                this.InputName = inputName;
                this.Region = r;
                this.PageSegMode = pageSegMode;
                this.EngineOptionBuilder = engineOptionBuilder;
            }

            public string? InputName { get; }

            public Pix Image { get; }

            public Rect Region { get; }

            public PageSegMode? PageSegMode { get; }

            public Action<EngineOptionBuilder>? EngineOptionBuilder { get; }
        }
    }
}