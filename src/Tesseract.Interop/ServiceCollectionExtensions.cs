namespace Tesseract.Interop
{
    using Abstractions;

    using InteropDotNet;

    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLeptonicaApi(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddSingleton<ILeptonicaApiSignatures>(_ =>
            {
                var signatures = InteropRuntimeImplementer.CreateInstance<ILeptonicaApiSignatures>();
                return signatures ?? throw new InvalidOperationException();
            });

            return services;
        }

        public static IServiceCollection AddTesseractApi(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddTransient<IManagedTesseractApi, TessApi>();
            services.AddSingleton<ITessApiSignatures>(_ =>
            {
                var signatures = InteropRuntimeImplementer.CreateInstance<ITessApiSignatures>();
                return signatures ?? throw new InvalidOperationException();
            });

            return services;
        }
    }
}