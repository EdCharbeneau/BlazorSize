#if NET5_0
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace BlazorPro.BlazorSize
{
    public class ResizeListener : IResizeListener, IAsyncDisposable
    {
        private readonly Lazy<Task<IJSObjectReference>> moduleTask;

        private readonly ResizeOptions options;
        
        private bool disposed;
        public ResizeListener(IJSRuntime jsRuntime, IOptions<ResizeOptions>? options = null)
        {
            this.options = options?.Value ?? new ResizeOptions();
            moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
                        "import", "./_content/BlazorPro.BlazorSize/blazorSizeResizeModule.js").AsTask());
        }

        private EventHandler<BrowserWindowSize>? onResized;

        /// <summary>
        /// Subscribe to the browsers resize() event.
        /// </summary>
        public event EventHandler<BrowserWindowSize>? OnResized
        {
            add => Subscribe(value);
            remove => Unsubscribe(value);
        }

        private void Unsubscribe(EventHandler<BrowserWindowSize>? value)
        {
            onResized -= value;
            if (onResized is null)
            {
                Cancel().ConfigureAwait(false);
            }
        }

        private void Subscribe(EventHandler<BrowserWindowSize>? value)
        {
            if (onResized is null)
            {
                Task.Run(async () => await Start());
            }
            onResized += value;
        }

        private async ValueTask<bool> Start()
        {
            var module = await moduleTask.Value;
            return await module.InvokeAsync<bool>("listenForResize", DotNetObjectReference.Create(this), options);
        }

        private async ValueTask Cancel()
        {
            var module = await moduleTask.Value;
            await module.InvokeVoidAsync("cancelListener");
        }

        /// <summary>
        /// Determine if the Document matches the provided media query.
        /// </summary>
        /// <param name="mediaQuery"></param>
        /// <returns>Returns true if matched.</returns>
        public async ValueTask<bool> MatchMedia(string mediaQuery)
        {
            var module = await moduleTask.Value;
            return await module.InvokeAsync<bool>("matchMedia", mediaQuery);
        }

        /// <summary>
        /// Get the current BrowserWindowSize, this includes the Height and Width of the document.
        /// </summary>
        /// <returns></returns>
        public async ValueTask<BrowserWindowSize> GetBrowserWindowSize()
        {
            var module = await moduleTask.Value;
            return await module.InvokeAsync<BrowserWindowSize>("getBrowserWindowSize");
        }

        /// <summary>
        /// Invoked by jsInterop, use the OnResized event handler to subscribe.
        /// </summary>
        /// <param name="browserWindowSize"></param>
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

        public async ValueTask DisposeAsync()
        {
            if (moduleTask.IsValueCreated)
            {
                var module = await moduleTask.Value;
                await module.DisposeAsync();
            }
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
#endif