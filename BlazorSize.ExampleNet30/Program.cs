using BlazorPro.BlazorSize;
using BlazorSize.ExampleNet30.Data;
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

namespace BlazorSize.ExampleNet30
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");
            builder.Services.AddScoped<ResizeListener>();
            builder.Services.AddScoped<IMediaQueryService, MediaQueryService>();
            builder.Services.AddSingleton<IWeatherForecastService, WeatherForecastService>();

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            await Task.Delay(1000);
            await builder.Build().RunAsync();
        }
    }
}
