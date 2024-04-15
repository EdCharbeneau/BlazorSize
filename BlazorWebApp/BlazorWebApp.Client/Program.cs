using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorPro.BlazorSize;
using TestComponents;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddResizeListener();
builder.Services.AddMediaQueryService();
builder.Services.AddScoped<IWeatherForecastService, WeatherForecastService>();
await builder.Build().RunAsync();
