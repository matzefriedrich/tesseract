namespace Tesseract.ImageProcessing
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Abstractions;
    using Microsoft.Extensions.DependencyInjection;

    [SuppressMessage("ReSharper", "UnusedMethodReturnValue.Global")]
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddImageProcessors(this IServiceCollection services)
        {
            ArgumentNullException.ThrowIfNull(services);

            services
                .AddTransient<IColorQuantizer, ColorQuantizer>()
                .AddTransient<IGrayscaleConverter, GrayscaleConverter>()
                .AddTransient<IImageBinarizer, ImageBinarizer>()
                .AddTransient<IImageInverter, ImageInverter>()
                .AddTransient<IImageRotator, ImageRotator>()
                .AddTransient<ILineRemover, LineRemover>()
                .AddTransient<INoiseRemover, NoiseRemover>()
                .AddTransient<ISkewCorrector, SkewCorrector>();

            return services;
        }
    }
}