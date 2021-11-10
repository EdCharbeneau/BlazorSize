using BlazorPro.BlazorSize;
using Microsoft.Extensions.DependencyInjection;

namespace Bunit
{
    public static class BunitExtensions
        {
            public static FakeMediaQueryService AddBlazorSize(this TestContextBase testContext)
            {
                testContext.RenderTree.Add<MediaQueryList>();
                var mediaQueryServices = new FakeMediaQueryService(testContext);
                testContext.Services.AddSingleton<IMediaQueryService>(mediaQueryServices);
                return mediaQueryServices;
            }
        }
}
