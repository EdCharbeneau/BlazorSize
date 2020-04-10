using BlazorPro.BlazorSize;
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
                options.EnableLogging = false;
                options.SuppressInitEvent = false;
            });
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            app.AddComponent<App>("app");
        }
    }
}
