#if !NET5_0
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace BlazorPro.BlazorSize
{
    public partial class MediaQueryList : IDisposable
    {
        // JavaScript namespace
        const string ns = "blazorSizeMedia.";

        // Alias for Js, this alias is used to support .NET 5.0 modules.
        IJSRuntime mediaQueryJs => Js;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                DotNetInstance = DotNetObjectReference.Create(this);
                await mediaQueryJs.InvokeVoidAsync($"{ns}addMediaQueryList", DotNetInstance);
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        public void Dispose()
        {
            if (DotNetInstance != null)
            {
                mediaQueryJs.InvokeVoidAsync($"{ns}removeMediaQueryList", DotNetInstance);
                DotNetInstance.Dispose();
                DotNetInstance = null;
            }
        }

    }
}
#endif