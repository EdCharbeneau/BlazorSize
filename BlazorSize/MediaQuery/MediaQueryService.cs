using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorPro.BlazorSize
{
    public class MediaQueryService : IAsyncDisposable, IMediaQueryService
    {
        private readonly Lazy<Task<IJSObjectReference>> moduleTask;

        public MediaQueryService(IJSRuntime jsRuntime)
        {
            moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", "./_content/BlazorPro.BlazorSize/blazorSizeMediaModule.js").AsTask());
        }

        // JavaScript uses this value for tracking MediaQueryList instances.
        private DotNetObjectReference<MediaQueryList> DotNetInstance { get; set; } = null!;
        private readonly List<MediaQueryCache> mediaQueries = new();
        public List<MediaQueryCache> MediaQueries => mediaQueries;

        private MediaQueryCache? GetMediaQueryFromCache(string byMedia) => mediaQueries?.Find(q => q.MediaRequested == byMedia);

        public void AddQuery(MediaQuery newMediaQuery)
        {
            MediaQueryCache? cache = GetMediaQueryFromCache(byMedia: newMediaQuery.Media);
            if (cache is null)
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
            if (mediaQuery is null) return;

            MediaQueryCache? cache = GetMediaQueryFromCache(byMedia: mediaQuery.Media);

            if (cache is null || cache.MediaQueries is null) return;

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
                //https://github.com/EdCharbeneau/BlazorSize/issues/93
                //https://stackoverflow.com/questions/72488563/blazor-server-side-application-throwing-system-invalidoperationexception-javas
            }
        }

        public async Task Initialize(MediaQuery mediaQuery)
        {
            if (mediaQuery?.Media is null) return;
            var cache = GetMediaQueryFromCache(byMedia: mediaQuery.Media);
            if (cache is null) return;

            if (cache.Value is null)
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
                    cache.MediaQueries ??= new List<MediaQuery>();
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
            try
            {
                if (DotNetInstance is null) return;
                
                var module = await moduleTask.Value;
                await module.InvokeVoidAsync("removeMediaQueryList", DotNetInstance);
                DotNetInstance.Dispose();
                await module.DisposeAsync();
                GC.SuppressFinalize(this);
            }
            catch (Exception)
            {
                //https://github.com/EdCharbeneau/BlazorSize/issues/93
                //https://stackoverflow.com/questions/72488563/blazor-server-side-application-throwing-system-invalidoperationexception-javas
            }
        }

    }

}