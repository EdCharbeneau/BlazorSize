using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorPro.BlazorSize.MediaQuery
{
    public class MediaQueryList : IDisposable
    {
        const string ns = "blazorSizeMedia";
        private readonly IJSRuntime jsRuntime;
        private bool jsInitialized;
        private readonly MediaQueryListOptions options;
        private bool disposed;
        private readonly Dictionary<string, List<Action<MediaQueryListEventArgs>>> callbackLists;

        public MediaQueryList(IJSRuntime jsRuntime)
        {
            this.jsRuntime = jsRuntime;
            callbackLists = new Dictionary<string, List<Action<MediaQueryListEventArgs>>>();
        }

        public async Task AddMediaQueryListener(string mediaQuery, Action<MediaQueryListEventArgs> callback)
        {
            if (!jsInitialized)
            {
                await jsRuntime.InvokeVoidAsync($"{ns}.init", DotNetObjectReference.Create(this), options);
                jsInitialized = true;
            }
            if (!callbackLists.ContainsKey(mediaQuery))
            {
                callbackLists.Add(mediaQuery, new List<Action<MediaQueryListEventArgs>>());
                await jsRuntime.InvokeVoidAsync($"{ns}.addMediaQueryListener", mediaQuery);
            }
            callbackLists[mediaQuery].Add(callback);
        }

        public async Task RemoveMediaQueryListener(string mediaQuery, Action<MediaQueryListEventArgs> callback)
        {
            if (!callbackLists.ContainsKey(mediaQuery))
            {
                throw new ArgumentOutOfRangeException(nameof(mediaQuery));
            }

            var callbackList = callbackLists[mediaQuery];

            if (callbackList.Remove(callback) && !callbackList.Any())
            {
                // last callback for this mediaquery, let's remove the javascript part as well.
                await jsRuntime.InvokeVoidAsync($"{ns}.removeMediaQueryListener", mediaQuery);
            }
        }

        public async ValueTask<Dictionary<string, bool>> GetState()
        {
            var result = await jsRuntime.InvokeAsync<List<MediaQueryListEventArgs>>($"{ns}.getState");
            return result.ToDictionary(k => k.Media, v => v.Matches);
        }

        [JSInvokable]
        public void HandleMediaQueryListEvent(MediaQueryListEventArgs args)
        {
            foreach (var item in callbackLists[args.Media])
            {
                item.Invoke(args);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    //onResized = null;
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

public class MediaQueryListOptions
{
    /// <summary>
    /// Report resize events and media queries in the browser's console.
    /// </summary>
    public bool EnableLogging { get; set; } = false;

    /// <summary>
    /// Suppress the first OnResized that is invoked when a new event handler is added.
    /// </summary>
    public bool SuppressInitEvent { get; set; } = false;
}