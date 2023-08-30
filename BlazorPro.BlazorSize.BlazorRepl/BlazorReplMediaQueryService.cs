namespace BlazorPro.BlazorSize
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.JSInterop;
    using BlazorPro.BlazorSize;

    public class BlazorReplMediaQueryService : IAsyncDisposable, IMediaQueryService
    {
        private readonly Lazy<Task<IJSObjectReference>> moduleTask;

        public BlazorReplMediaQueryService(IJSRuntime jsRuntime)
        {
            moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", "https://edcharbeneau.com/javascript/blazorSizeMediaModule.js").AsTask());
        }

        // JavaScript uses this value for tracking MediaQueryList instances.
        private DotNetObjectReference<MediaQueryList> DotNetInstance { get; set; } = null!;
        private readonly List<MediaQueryCache> mediaQueries = new();
        public List<MediaQueryCache> MediaQueries => mediaQueries;

        private MediaQueryCache? GetMediaQueryFromCache(string byMedia) => mediaQueries?.Find(q => q.MediaRequested == byMedia);

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
            cache.MediaQueries?.Add(newMediaQuery);
        }

        public async Task RemoveQuery(MediaQuery mediaQuery)
        {
            if (mediaQuery == null) return;

            MediaQueryCache? cache = GetMediaQueryFromCache(byMedia: mediaQuery.Media);

            if (cache == null || cache.MediaQueries == null) return;

            try
            {
                cache.MediaQueries.Remove(mediaQuery);
                if (cache.MediaQueries.Any()) return;
                mediaQueries.Remove(cache);
                IJSObjectReference module = await moduleTask.Value;
                await module.InvokeVoidAsync($"removeMediaQuery", DotNetInstance, mediaQuery.InternalMedia.Media);
            }
            catch (Exception)
            {
                //https://stackoverflow.com/questions/72488563/blazor-server-side-application-throwing-system-invalidoperationexception-javas
            }
        }

        public async Task Initialize(MediaQuery mediaQuery)
        {
            if (mediaQuery?.Media == null) return;
            var cache = GetMediaQueryFromCache(byMedia: mediaQuery.Media);
            if (cache == null) return;

            if (cache.Value == null)
            {
                // If we don't flag the cache as loading, duplicate requests will be sent async.
                // Duplicate requests = poor performance, esp with web sockets.
                if (!cache.Loading)
                {
                    cache.Loading = true;

                    var module = await moduleTask.Value;
                    var task = module.InvokeAsync<MediaQueryArgs>($"addMediaQueryToList", DotNetInstance, cache.MediaRequested);
                    cache.Value = await task;
                    cache.Loading = task.IsCompleted;
                    // When loading is complete dispatch an update to all subscribers.
                    if (cache.MediaQueries is null) return;
                    foreach (var item in cache.MediaQueries)
                    {
                        item.MediaQueryChanged(cache.Value);
                    }
                }
            }
            else
            {
                var module = await moduleTask.Value;
                var task = module.InvokeAsync<MediaQueryArgs>($"getMediaQueryArgs", cache.MediaRequested);
                cache.Value = await task;

                mediaQuery.MediaQueryChanged(cache.Value);
            }
        }

        public async Task CreateMediaQueryList(DotNetObjectReference<MediaQueryList> dotNetObjectReference)
        {
            DotNetInstance = dotNetObjectReference;
            var module = await moduleTask.Value;
            await module.InvokeVoidAsync("addMediaQueryList", dotNetObjectReference);
        }

        public async ValueTask DisposeAsync()
        {
            if (DotNetInstance is not null)
            {
                var module = await moduleTask.Value;
                await module.InvokeVoidAsync("removeMediaQueryList", DotNetInstance);
                DotNetInstance.Dispose();
                GC.SuppressFinalize(this);
            }
        }
    }
}