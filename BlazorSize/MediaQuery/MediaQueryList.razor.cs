using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorPro.BlazorSize
{
    public partial class MediaQueryList : IDisposable
    {
        const string ns = "blazorSizeMedia";
        [Inject] public IJSRuntime Js { get; set; }
        [Parameter] public RenderFragment ChildContent { get; set; }
        public DotNetObjectReference<MediaQueryList> DotNetInstance { get; private set; }
        private readonly List<MediaQueryCache> mediaQueries = new List<MediaQueryCache>();

        public void AddQuery(MediaQuery newMediaQuery)
        {
            bool byMediaProperties(MediaQueryCache q) => q.MediaRequested == newMediaQuery.Media;
            var cache = mediaQueries.Find(byMediaProperties);
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
            Console.WriteLine(mediaQuery.InternalMedia.Media);
            foreach (var item in mediaQueries)
            {
                Console.WriteLine(item.Value.Media);
            }

            bool byMediaProperties(MediaQueryCache q) => q.Value.Media == mediaQuery.InternalMedia.Media;
            var cache = mediaQueries.Find(byMediaProperties);
            if (cache != null)
            {

                cache.MediaQueries.Remove(mediaQuery);
                if (cache.MediaQueries.Count() == 0)
                {
                    Js.InvokeVoidAsync($"{ns}.removeMediaQuery", DotNetInstance, mediaQuery.InternalMedia.Media);
                    mediaQueries.Remove(cache);
                }
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                DotNetInstance = DotNetObjectReference.Create(this);
                await Js.InvokeVoidAsync($"{ns}.addMediaQueryList", DotNetInstance);
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        public async Task Initialize(MediaQuery mediaQuery)
        {
            Console.WriteLine($"Initialize Start {mediaQuery.Media}");

            bool byMediaProperties(MediaQueryCache q) => q.MediaRequested == mediaQuery.Media;
            var cache = mediaQueries.Find(byMediaProperties);
            if (cache.Value == null)
            {
                Console.WriteLine($"Calling JavaScript {mediaQuery.Media}");

                cache.Value = await Js.InvokeAsync<MediaQueryArgs>($"{ns}.addMediaQueryToList", DotNetInstance, cache.MediaRequested);
            }
            mediaQuery.MediaQueryChanged(cache.Value);
            Console.WriteLine($"Initialize End {mediaQuery.Media}");
        }

        [JSInvokable(nameof(MediaQueryList.MediaQueryChanged))]
        public void MediaQueryChanged(MediaQueryArgs args)
        {

            bool byMediaProperties(MediaQueryCache q) => q.Value.Media == args.Media;
            var cache = mediaQueries.Find(byMediaProperties);
            foreach (var item in cache.MediaQueries)
            {
                item.MediaQueryChanged(args);
            }
        }
        public void Dispose()
        {
            if (DotNetInstance != null)
            {
                Js.InvokeVoidAsync($"{ns}.removeMediaQueryList", DotNetInstance);
                DotNetInstance.Dispose();
                DotNetInstance = null;
            }
        }

        internal class MediaQueryCache
        {
            public string MediaRequested { get; set; }
            public MediaQueryArgs Value { get; set; }
            public List<MediaQuery> MediaQueries { get; set; } = new List<MediaQuery>();
        }
    }
}