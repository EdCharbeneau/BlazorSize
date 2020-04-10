using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorPro.BlazorSize
{
    public class MediaQueryList : IAsyncDisposable
    {
        const string ns = "blazorSizeMedia";
        private readonly IJSRuntime js;
        private readonly Dictionary<string, Action<MediaQueryArgs>> callbackQueue;
        private DotNetObjectReference<MediaQueryList> DotNetInstance;

        public MediaQueryList(IJSRuntime jSRuntime)
        {
            js = jSRuntime;
            callbackQueue = new Dictionary<string, Action<MediaQueryArgs>>();
            DotNetInstance = DotNetObjectReference.Create(this);
        }

        public async ValueTask<MediaQueryArgs> Add(string mediaQuery, Action<MediaQueryArgs> callback)
        {
            var mq = await js.InvokeAsync<MediaQueryArgs>($"{ns}.addMediaQueryListener", mediaQuery, DotNetInstance);
            callbackQueue.Add(mq.Media, callback);
            return mq;
        }

        public async Task RemoveAllCallbacks()
        {
            await js.InvokeVoidAsync($"{ns}.removeMediaQueryListeners", DotNetInstance);
        }


        [JSInvokable(nameof(MediaQueryList.MediaQueryChanged))]
        public void MediaQueryChanged(MediaQueryArgs args)
        {
            callbackQueue[args.Media].Invoke(args);
        }
        public async ValueTask DisposeAsync()
        {
            if (DotNetInstance != null)
            {
                await RemoveAllCallbacks();
                DotNetInstance.Dispose();
                DotNetInstance = null;
            }
        }

    }

    public class MediaQueryArgs
    {
        public string Media { get; set; }
        public bool Matches { get; set; }
    }
    public class MediaQuery
    {
        public string Media { get; set; }
        public bool Matches { get; set; }
    }
}