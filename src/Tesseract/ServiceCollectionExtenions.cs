namespace Tesseract
{
    using System;

    using Abstractions;

    using Interop;
    using Interop.Abstractions;

    using JetBrains.Annotations;

    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceCollectionExtenions
    {
        public static IServiceCollection AddTesseract([NotNull] this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services
                .AddTesseractApi()
                .AddLeptonicaApi();
            
            services.AddSingleton<IPixFactory, PixFactory>();
            services.AddSingleton<IPixArrayFactory, PixArrayFactory>();
            services.AddSingleton<IPixColorMapFactory, PixColorMapFactory>();

            services.AddTransient<IPixToBitmapConverter, PixToBitmapConverter>();
            services.AddTransient<IBitmapToPixConverter, BitmapToPixConverter>();
            services.AddTransient<IPixConverter, PixConverter>();

            services.AddTransient<IResultRendererFactory, ResultRendererFactory>();
            services.AddTransient<TesseractEngineFactory>(provider =>
            {
                return options =>
                {
                    var api = provider.GetRequiredService<IManagedTesseractApi>();
                    var native = provider.GetRequiredService<ITessApiSignatures>();
                    var leptonicaNativeApi = provider.GetRequiredService<ILeptonicaApiSignatures>();
                    var pixFactory = provider.GetRequiredService<IPixFactory>();
                    return new TesseractEngine(api, native, leptonicaNativeApi, pixFactory, options);

                };
            });

            return services;
        }
    }
}