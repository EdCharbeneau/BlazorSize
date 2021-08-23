using BlazorPro.BlazorSize;
using BlazorSize.ExampleNet6.Data;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TestComponents;

namespace BlazorSize.ExampleNet6
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            //builder.Services.AddScoped<WeatherForecastService>();
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddScoped<IResizeListener, ResizeListener>();

            builder.Services.AddScoped<IMediaQueryService, MediaQueryService>();
            //builder.Services.AddMediaQueryService();

            builder.Services.AddSingleton<IWeatherForecastService, WeatherForecastService>();
            await builder.Build().RunAsync();
        }
    }
}
