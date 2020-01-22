using Microsoft.Extensions.DependencyInjection;
using System;

namespace BlazorPro.BlazorSize
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddResizeListener(this IServiceCollection services,
            Action<ResizeOptions> configure)
        {
            var options = new ResizeOptions();

            services.AddScoped<ResizeListener>();

            services.Configure(configure);
            return services;
        }
    }
}