namespace Tesseract
{
    using System;
    using Abstractions;

    internal sealed class DefaultTesseractEngineFactory : ITesseractEngineFactory
    {
        private readonly ConfigurableTesseractEngineFactory configurableTesseractEngineFactory;
        private readonly EngineOptionBuilder optionsBuilder;

        public DefaultTesseractEngineFactory(
            EngineOptionBuilder optionsBuilder,
            ConfigurableTesseractEngineFactory configurableTesseractEngineFactory)
        {
            this.optionsBuilder = optionsBuilder ?? throw new ArgumentNullException(nameof(optionsBuilder));
            this.configurableTesseractEngineFactory = configurableTesseractEngineFactory ?? throw new ArgumentNullException(nameof(configurableTesseractEngineFactory));
        }

        public ITesseractEngine CreateEngine(Action<EngineOptionBuilder>? builder = null)
        {
            builder?.Invoke(this.optionsBuilder);
            TesseractEngineOptions options = this.optionsBuilder.Build();
            return this.configurableTesseractEngineFactory(options);
        }
    }
}