using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace BlazorSize
{
    public class ResizeListener
    {
        private Action<BrowserWindowSize> OnResized;

        private readonly IJSRuntime _jsRuntime;

        public ResizeListener(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }
        public async ValueTask<bool> WatchResize(Action<BrowserWindowSize> onResize, string mediaQuery)
        {
            OnResized = onResize;

            return await _jsRuntime.InvokeAsync<bool>("resizeListener.watchOnResize", DotNetObjectReference.Create(this), mediaQuery);
        }

        [JSInvokable]
        public void RaiseOnResized(BrowserWindowSize browserWindowSize) => OnResized?.Invoke(browserWindowSize);

    }
}
