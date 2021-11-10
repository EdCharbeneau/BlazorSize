using BlazorPro.BlazorSize;
using Microsoft.JSInterop;

namespace Bunit
{
    public class FakeMediaQueryService : IMediaQueryService
    {
        private readonly TestContextBase contextBase;
        private MediaQueryArgs activeMediaQuery = null!;

        public List<MediaQueryCache> MediaQueries { get; } = new();

        public FakeMediaQueryService(TestContextBase contextBase)
        {
            this.contextBase = contextBase;
        }

        public void AddQuery(MediaQuery newMediaQuery)
        {
            var mediaQueryCache = GetMediaQueryFromCache(newMediaQuery.Media);

            if (mediaQueryCache is null)
            {
                mediaQueryCache = new MediaQueryCache()
                {
                    MediaRequested = newMediaQuery.Media
                };
                MediaQueries.Add(mediaQueryCache);
            }

            mediaQueryCache.MediaQueries.Add(newMediaQuery);

            SetActiveMediaQuery(activeMediaQuery);
        }

        public Task CreateMediaQueryList(DotNetObjectReference<MediaQueryList> dotNetObjectReference)
            => Task.CompletedTask;

        public Task Initialize(MediaQuery mediaQuery)
            => Task.CompletedTask;

        public Task RemoveQuery(MediaQuery mediaQuery)
        {
            var mediaQueryFromCache = GetMediaQueryFromCache(mediaQuery.Media);
            if (mediaQueryFromCache is null)
                return Task.CompletedTask;

            mediaQueryFromCache.MediaQueries.Remove(mediaQuery);

            if (mediaQueryFromCache.MediaQueries.Count == 0)
                MediaQueries.Remove(mediaQueryFromCache);

            return Task.CompletedTask;
        }

        public void SetActiveBreakPoint(string breakpoint)
            => SetActiveMediaQuery(new MediaQueryArgs
            {
                Matches = true,
                Media = breakpoint
            });

        public void SetActiveMediaQuery(MediaQueryArgs args)
        {
            if (args.Media == null) return;

            activeMediaQuery = args;

            // cache must be compared by actual value, not RequestedMedia when invoked from JavaScript
            // DOM Media value my be different that the initally requested media query value.
            var cache = GetMediaQueryFromCache(args.Media);

            if (cache is null) return;

            // Dispatch events to all subscribers
            contextBase.Renderer.Dispatcher.InvokeAsync(() =>
            {
                foreach (var item in cache.MediaQueries)
                {
                    item.MediaQueryChanged(args);
                }
            });
        }

        private MediaQueryCache? GetMediaQueryFromCache(string byMedia)
            => MediaQueries?.Find(q => q.MediaRequested == byMedia);
    }
}
