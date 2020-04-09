using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorPro.BlazorSize.MediaQuery
{
    public class MediaQueryListener : IDisposable
    {
        const string ns = "blazorSizeMedia";
        private readonly IJSRuntime jsRuntime;
        private readonly ResizeOptions options;
        private bool disposed;
        public MediaQueryListener(IJSRuntime jsRuntime, IOptions<ResizeOptions> options = null)
        {
            this.options = options.Value ?? new ResizeOptions();
            this.jsRuntime = jsRuntime;
        }

#nullable enable
        private EventHandler<MediaQueryListItem>? onMediaQueryListChanged;

        /// <summary>
        /// Subscribe to the browsers resize() event.
        /// </summary>
        public event EventHandler<MediaQueryListItem>? OnMediaQueryListChanged
        {
            add => Subscribe(value);
            remove => Unsubscribe(value);
        }
#nullable disable

        private void Unsubscribe(EventHandler<MediaQueryListItem> value)
        {
            onMediaQueryListChanged -= value;
            if (onMediaQueryListChanged == null)
            {
                Cancel().ConfigureAwait(false);
            }
        }

        private void Subscribe(EventHandler<MediaQueryListItem> value)
        {
            if (onMediaQueryListChanged == null)
            {
                Task.Run(async () => await Start());
            }
            onMediaQueryListChanged += value;
        }

        private async ValueTask<bool> Start() =>
            await jsRuntime.InvokeAsync<bool>($"{ns}.init", DotNetObjectReference.Create(this), options);

        private async ValueTask Cancel() =>
            await jsRuntime.InvokeVoidAsync($"{ns}.cancelListener");

        public async ValueTask<MediaQueryListItem> SubscribeMediaQuery(MediaQueryListItem mediaQuery) =>
            await jsRuntime.InvokeAsync<MediaQueryListItem>($"{ns}.addMediaQueryListener", mediaQuery.Media);

        [JSInvokable]
        public void RaiseOnMediaQueryListChanged(MediaQueryListItem queryListItem) =>
            onMediaQueryListChanged?.Invoke(this, queryListItem);

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

    public class MediaQueryListItem
    {
        public MediaQueryListItem() { }
        public MediaQueryListItem(string media)
        {
            Media = media;
        }
        public string Media { get; set; }
        public bool Matches { get; set; }
        public Action<MediaQueryListItem>? OnChange { get; set; }
    }

    public static class MediaQueryListItemExtensions
    {
        public static void Update(this MediaQueryListItem[] mq, MediaQueryListItem item)
        {
            var changed = mq.First(q => q.Media == item.Media);
            changed.Matches = item.Matches;
            changed.OnChange?.Invoke(item);
        }
    }
}
