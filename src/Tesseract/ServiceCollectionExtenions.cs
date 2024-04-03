namespace Tesseract
{
    using System;
    using Abstractions;
    using ImageProcessing;
    using Interop;
    using Interop.Abstractions;
    using Microsoft.Extensions.DependencyInjection;
    using Rendering;
    using Rendering.Abstractions;

    public static class ServiceCollectionExtenions
    {
        public static IServiceCollection AddTesseract(this IServiceCollection services)
        {
            ArgumentNullException.ThrowIfNull(services);

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
            services.AddTransient<TesseractEngineFactory>(provider =>
            {
                return options =>
                {
                    var api = provider.GetRequiredService<IManagedTesseractApi>();
                    var native = provider.GetRequiredService<ITessApiSignatures>();
                    var leptonicaNativeApi = provider.GetRequiredService<ILeptonicaApiSignatures>();
                    var pixFactory = provider.GetRequiredService<IPixFactory>();
                    var pixFileWriter = provider.GetRequiredService<IPixFileWriter>();
                    return new TesseractEngine(api, native, leptonicaNativeApi, pixFactory, pixFileWriter, options);
                };
            });

            services.AddImageProcessors();
            
            return services;
        }
    }
}