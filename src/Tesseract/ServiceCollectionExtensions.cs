namespace Tesseract
{
    using System;
    using Abstractions;
    using ImageProcessing;
    using Interop;
    using Interop.Abstractions;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;
    using Microsoft.Extensions.Options;
    using Rendering;
    using Rendering.Abstractions;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTesseract(this IServiceCollection services, EngineOptionDefaults? engineDefaults = null)
        {
            ArgumentNullException.ThrowIfNull(services);

            if (engineDefaults != null)
            {
                var wrapper = new OptionsWrapper<EngineOptionDefaults>(engineDefaults);
                services.TryAddSingleton<IOptions<EngineOptionDefaults>>(wrapper);
            }

            services.TryAddSingleton<ILoggerFactory>(new NullLoggerFactory());
            services.AddTransient<EngineOptionBuilder>();
            
            services
                .AddTesseractApi()
                .AddLeptonicaApi();

            services.AddSingleton<IPixFactory, PixFactory>();
            services.AddSingleton<IPixArrayFactory, PixArrayFactory>();
            services.AddSingleton<IPixColorMapFactory, PixColorMapFactory>();

            services.AddTransient<IPixToBitmapConverter, PixToBitmapConverter>();
            services.AddTransient<IBitmapToPixConverter, BitmapToPixConverter>();
            services.AddTransient<IPixConverter, PixConverter>();
            services.AddTransient<IPixFileWriter, PixFileWriter>();

            services.AddTransient<IResultRendererFactory, ResultRendererFactory>();
            services.AddSingleton<IPageFactory, PageFactory>();
            services.AddTransient<ITesseractEngineFactory, DefaultTesseractEngineFactory>();
            services.AddTransient<ConfigurableTesseractEngineFactory>(provider =>
            {
                return options =>
                {
                    var api = provider.GetRequiredService<IManagedTesseractApi>();
                    var native = provider.GetRequiredService<ITessApiSignatures>();
                    return new TesseractEngine(api, native, options);
                };
            });

            services.AddImageProcessors();
            
            return services;
        }
    }
}