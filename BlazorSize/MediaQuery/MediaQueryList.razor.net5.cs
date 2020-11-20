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
                await mediaQueryJs.InvokeVoidAsync("addMediaQueryList", DotNetInstance);
            }
            await base.OnAfterRenderAsync(firstRender);
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