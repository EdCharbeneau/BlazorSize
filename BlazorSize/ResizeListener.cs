using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace BlazorSize
{
    public class ResizeListener
    {
        const string ns = "blazorSize";
        private readonly IJSRuntime jsRuntime;
        private bool disposed;
        private EventHandler<BrowserWindowSize>? onResized;
        public ResizeListener(IJSRuntime jsRuntime) => (this.jsRuntime) = (jsRuntime);

        public event EventHandler<BrowserWindowSize>? OnResized
        {
            add => Subscribe(value);
            remove => Unsubscribe(value);
        }

        private void Unsubscribe(EventHandler<BrowserWindowSize> value)
        {
            onResized -= value;
            if (onResized == null)
            {
                Cancel().ConfigureAwait(false);
            }
        }

        private void Subscribe(EventHandler<BrowserWindowSize> value)
        {
            if (onResized == null)
            {
                Start().ConfigureAwait(false);
            }
            onResized += value;
        }

        private async ValueTask<bool> Start() =>
            await jsRuntime.InvokeAsync<bool>($"{ns}.listenForResize", DotNetObjectReference.Create(this));

        private async ValueTask Cancel() =>
            await jsRuntime.InvokeVoidAsync($"{ns}.cancelListener");

        public async ValueTask<bool> MatchMedia(string mediaQuery) =>
            await jsRuntime.InvokeAsync<bool>($"{ns}.matchMedia", mediaQuery);

        public async ValueTask<BrowserWindowSize> GetBrowserWindowSize() =>
            await jsRuntime.InvokeAsync<BrowserWindowSize>($"{ns}.getBrowserWindowSize");

        [JSInvokable]
        public void RaiseOnResized(BrowserWindowSize browserWindowSize) => 
            onResized?.Invoke(this, browserWindowSize);

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    onResized = null;
                }
                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
