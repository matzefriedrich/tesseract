namespace Tesseract.Interop
{
    using System.Diagnostics.CodeAnalysis;
    using Abstractions;
    using InteropDotNet;
    using Microsoft.Extensions.DependencyInjection;

    [SuppressMessage("ReSharper", "UnusedMethodReturnValue.Global")]
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLeptonicaApi(this IServiceCollection services)
        {
            ArgumentNullException.ThrowIfNull(services);

            services.AddSingleton<ILeptonicaApiSignatures>(_ =>
            {
                var signatures = InteropRuntimeImplementer.CreateInstance<ILeptonicaApiSignatures>();
                return signatures ?? throw new InvalidOperationException();
            });

            return services;
        }

        public static IServiceCollection AddTesseractApi(this IServiceCollection services)
        {
            ArgumentNullException.ThrowIfNull(services);

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