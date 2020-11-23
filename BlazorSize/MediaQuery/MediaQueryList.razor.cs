using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorPro.BlazorSize
{

    public partial class MediaQueryList
    {
        [Inject] public IJSRuntime Js { get; set; }

        /// <summary>
        /// Application content where Media Query components may exist.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        // JavaScript uses this value for tracking MediaQueryList instances.
        private DotNetObjectReference<MediaQueryList> DotNetInstance { get; set; }
        private readonly List<MediaQueryCache> mediaQueries = new List<MediaQueryCache>();

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

        public void RemoveQuery(MediaQuery mediaQuery)
        {
            var cache = GetMediaQueryFromCache(byMedia: mediaQuery.Media);
            if (cache != null)
            {

                cache.MediaQueries.Remove(mediaQuery);
                if (cache.MediaQueries.Count() == 0)
                {
                    mediaQueryJs.InvokeVoidAsync($"{ns}removeMediaQuery", DotNetInstance, mediaQuery.InternalMedia.Media);
                    mediaQueries.Remove(cache);
                }
            }
        }

        public async Task Initialize(MediaQuery mediaQuery)
        {
            // if the module is not loaded, deferred initialization to after import, InitializeCachedMediaQueries.
            if (mediaQueryJs == null) return;

            var cache = GetMediaQueryFromCache(byMedia: mediaQuery.Media);
            if (cache.Value == null)
            {
                // If we don't flag the cache as loading, duplicate requests will be sent async.
                // Duplicate requests = poor performance, esp with web sockets.
                if (!cache.Loading)
                {
                    cache.Loading = true;
                    var task = mediaQueryJs.InvokeAsync<MediaQueryArgs>($"{ns}addMediaQueryToList", DotNetInstance, cache.MediaRequested);
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
                var task = mediaQueryJs.InvokeAsync<MediaQueryArgs>($"{ns}getMediaQueryArgs", cache.MediaRequested);
                cache.Value = await task;

                mediaQuery.MediaQueryChanged(cache.Value);
            }
        }

        /// <summary>
        /// Called by JavaScript when a media query changes in the dom.
        /// </summary>
        /// <param name="args"></param>
        [JSInvokable(nameof(MediaQueryList.MediaQueryChanged))]
        public void MediaQueryChanged(MediaQueryArgs args)
        {
            // cache must be compared by actual value, not RequestedMedia when invoked from JavaScript
            // DOM Media value my be different that the initally requested media query value.
            var cache = mediaQueries.Find(q => q.Value.Media == args.Media);

            // Dispatch events to all subscribers
            foreach (var item in cache.MediaQueries)
            {
                item.MediaQueryChanged(args);
            }
        }

    }
}