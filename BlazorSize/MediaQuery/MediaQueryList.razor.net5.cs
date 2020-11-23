#if NET5_0
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace BlazorPro.BlazorSize
{
    public partial class MediaQueryList : IAsyncDisposable
    {
        // JavaScript namespace is empty for JS Modules
        const string ns = "";

        private IJSObjectReference mediaQueryJs;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                mediaQueryJs = await Js.InvokeAsync<IJSObjectReference>(
                    "import",
                    "./_content/BlazorPro.BlazorSize/blazorSizeMediaModule.js"
                    );

                DotNetInstance = DotNetObjectReference.Create(this);
                var result = mediaQueryJs.InvokeVoidAsync("addMediaQueryList", DotNetInstance);
                await result;
                // Because MediaQueries are added by the child component before IJSObjectReference completes.
                // We must cache the MediaQueries and load them after the JS module is imported.
                // On additional renders the module is already available and child components can self initialize.
                await InitializeCachedMediaQueries();
            }
            await base.OnAfterRenderAsync(firstRender);

            async Task InitializeCachedMediaQueries()
            {
                foreach (var item in mediaQueries)
                {
                    foreach (var mq in item.MediaQueries)
                    {
                        await Initialize(mq);
                    }
                }
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (DotNetInstance != null)
            {
                await mediaQueryJs.InvokeVoidAsync("removeMediaQueryList", DotNetInstance);
                DotNetInstance.Dispose();
                DotNetInstance = null;
            }
        }
    }
}
#endif