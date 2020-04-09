using Microsoft.Extensions.DependencyInjection;
using System;

namespace BlazorPro.BlazorSize
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds a BlazorSize.ResizeListener as a Scoped instance.
        /// </summary>
        /// <param name="configure">Defines settings for this instance.</param>
        /// <returns>Continues the IServiceCollection chain.</returns>
        public static IServiceCollection AddResizeListener(this IServiceCollection services,
            Action<ResizeOptions> configure)
        {
            services.AddScoped<ResizeListener>();
            services.Configure(configure);
            return services;
        }

        /// <summary>
        /// Adds a BlazorSize.MediaQueryListener as a Scoped instance.
        /// </summary>
        /// <param name="configure">Defines settings for this instance.</param>
        /// <returns>Continues the IServiceCollection chain.</returns>
        public static IServiceCollection AddMediaQueryListener(this IServiceCollection services,
            Action<MediaQueryListenerOptions> configure)
        {
            services.AddScoped<MediaQueryListener>();
            services.Configure(configure);
            return services;
        }
    }
}