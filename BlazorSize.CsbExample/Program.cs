using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Blazor.Hosting;
using Microsoft.Extensions.DependencyInjection;
using BlazorPro.BlazorSize;

namespace BlazorSize.CsbExample
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");
            builder.Services.AddResizeListener(options =>
            {
                options.ReportRate = 300;
                options.EnableLogging = false;
                options.SuppressInitEvent = false;
            });
            await builder.Build().RunAsync();
        }
    }
}
