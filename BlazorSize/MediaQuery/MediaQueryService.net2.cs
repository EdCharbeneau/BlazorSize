#if !NET5_0

using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorPro.BlazorSize
{
    public class MediaQueryService : IDisposable, IMediaQueryService
    {
        // JavaScript namespace is empty for JS Modules
        const string ns = "blazorSizeMedia.";

        public MediaQueryService(IJSRuntime jsRuntime)
        {
            this.jsRuntime = jsRuntime;
        }

        // JavaScript uses this value for tracking MediaQueryList instances.
        private DotNetObjectReference<MediaQueryList> DotNetInstance { get; set; }
        private readonly List<MediaQueryCache> mediaQueries = new List<MediaQueryCache>();
        private readonly IJSRuntime jsRuntime;

        public List<MediaQueryCache> MediaQueries => mediaQueries;

        private MediaQueryCache GetMediaQueryFromCache(string byMedia) => mediaQueries?.Find(q => q.MediaRequested == byMedia);

        public void AddQuery(MediaQuery newMediaQuery)
        {
            var cache = GetMediaQueryFromCache(byMedia: newMediaQuery.Media);
            if (cache == null)
            {
                cache = new MediaQueryCache
                {
                    MediaRequested = newMediaQuery.Media,
                };
                mediaQueries.Add(cache);
            }
            cache.MediaQueries.Add(newMediaQuery);
        }

        public async Task RemoveQuery(MediaQuery mediaQuery)
        {
            var cache = GetMediaQueryFromCache(byMedia: mediaQuery.Media);
            if (cache != null)
            {
                cache.MediaQueries.Remove(mediaQuery);
                if (cache.MediaQueries.Any())
                {
                    mediaQueries.Remove(cache);
                    await jsRuntime.InvokeVoidAsync($"{ns}removeMediaQuery", DotNetInstance, mediaQuery.InternalMedia.Media);
                }
            }
        }

        public async Task Initialize(MediaQuery mediaQuery)
        {
            var cache = GetMediaQueryFromCache(byMedia: mediaQuery.Media);
            if (cache.Value == null)
            {
                // If we don't flag the cache as loading, duplicate requests will be sent async.
                // Duplicate requests = poor performance, esp with web sockets.
                if (!cache.Loading)
                {
                    cache.Loading = true;

                    var task = jsRuntime.InvokeAsync<MediaQueryArgs>($"{ns}addMediaQueryToList", DotNetInstance, cache.MediaRequested);
                    cache.Value = await task;
                    cache.Loading = task.IsCompleted;
                    // When loading is complete dispatch an update to all subscribers.
                    foreach (var item in cache.MediaQueries)
                    {
                        item.MediaQueryChanged(cache.Value);
                    }
                }
            }
            else
            {
                var task = jsRuntime.InvokeAsync<MediaQueryArgs>($"{ns}getMediaQueryArgs", cache.MediaRequested);
                cache.Value = await task;

                mediaQuery.MediaQueryChanged(cache.Value);
            }
        }

        public async Task CreateMediaQueryList(DotNetObjectReference<MediaQueryList> dotNetObjectReference)
        {
            DotNetInstance = dotNetObjectReference;
            await jsRuntime.InvokeVoidAsync($"{ns}addMediaQueryList", dotNetObjectReference);
        }

        public void Dispose()
        {
            if (DotNetInstance != null)
            {
                jsRuntime.InvokeVoidAsync($"{ns}removeMediaQueryList", DotNetInstance);
                DotNetInstance.Dispose();
                DotNetInstance = null;
            }
        }

    }

}
#endif