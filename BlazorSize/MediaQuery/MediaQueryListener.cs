using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace BlazorPro.BlazorSize
{
    public class MediaQueryListener : IDisposable
    {
        const string ns = "blazorSizeMedia";
        private bool jsInitialized;
        private readonly IJSRuntime jsRuntime;
        private readonly MediaQueryListenerOptions options;
        private bool disposed;
        public MediaQueryListener(IJSRuntime jsRuntime, IOptions<MediaQueryListenerOptions> options = null)
        {
            this.options = options.Value ?? new MediaQueryListenerOptions();
            this.jsRuntime = jsRuntime;
        }

#nullable enable
        private EventHandler<MediaQueryEventArgs>? onMediaQueryListChanged;

        public event EventHandler<MediaQueryEventArgs>? OnMediaQueryListChanged
        {
            add => Subscribe(value);
            remove => Unsubscribe(value);
        }
#nullable disable

        private void Unsubscribe(EventHandler<MediaQueryEventArgs> value)
        {
            onMediaQueryListChanged -= value;
            if (onMediaQueryListChanged == null)
            {
                Cancel().ConfigureAwait(false);
            }
        }

        private void Subscribe(EventHandler<MediaQueryEventArgs> value)
        {
            if (!jsInitialized) Task.Run(async () => await Init());

            onMediaQueryListChanged += value;
        }

        private async Task Init()
        {
            await jsRuntime.InvokeVoidAsync($"{ns}.init", DotNetObjectReference.Create(this), options);
            jsInitialized = true;
        }

        private async Task Cancel() => await jsRuntime.InvokeVoidAsync($"{ns}.cancelListener");

        public async ValueTask<MediaQuery> SubscribeMediaQuery(MediaQuery mediaQuery)
        {
            if (!jsInitialized) await Init();

            var mq = await jsRuntime.InvokeAsync<MediaQueryEventArgs>($"{ns}.addMediaQueryListener", mediaQuery.Media);
            return new MediaQuery(mq.Media, mq.Matches);
        }

        [JSInvokable]
        public void RaiseOnMediaQueryListChanged(MediaQueryEventArgs queryListItem)
        {
            onMediaQueryListChanged?.Invoke(this, queryListItem);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    onMediaQueryListChanged = null;
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
