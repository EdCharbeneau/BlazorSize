using BlazorPro.BlazorSize;
using BlazorPro.BlazorSize.MediaQuery;
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorSize.CsbExample
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddResizeListener(options =>
            {
                options.ReportRate = 300;
                options.EnableLogging = true;
                options.SuppressInitEvent = false;
            });
            services.AddScoped<MediaQueryListener>();
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            app.AddComponent<App>("app");
        }
    }
}
