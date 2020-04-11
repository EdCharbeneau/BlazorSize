using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace BlazorPro.BlazorSize
{
    public partial class MediaQuery : IDisposable
    {
        const string ns = "blazorSizeMedia";
        [Inject] public IJSRuntime Js { get; set; }
        [Parameter] public string Media { get; set; }
        [Parameter] public bool Matches { get; set; }
        [Parameter] public EventCallback<bool> MatchesChanged { get; set; }
        [Parameter] public RenderFragment Matched { get; set; }
        [Parameter] public RenderFragment Unmatched { get; set; }

        private MediaQueryArgs internalMedia = new MediaQueryArgs();
        private DotNetObjectReference<MediaQuery> DotNetInstance;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                DotNetInstance = DotNetObjectReference.Create(this);
                var mq = await Js.InvokeAsync<MediaQueryArgs>($"{ns}.addMediaQueryListener", Media, DotNetInstance);
                internalMedia = mq;
                MediaQueryChanged(mq);
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        [JSInvokable(nameof(MediaQuery.MediaQueryChanged))]
        public void MediaQueryChanged(MediaQueryArgs args)
        {
            if (args.Media == internalMedia.Media)
            {
                MatchesChanged.InvokeAsync(args.Matches);
                internalMedia = args;
                if (Matched != null || Unmatched != null)
                {
                    StateHasChanged();
                }
            }
        }

        public async void Dispose()
        {
            if (DotNetInstance != null)
            {
                await Js.InvokeVoidAsync($"{ns}.removeMediaQueryListeners", DotNetInstance);
                DotNetInstance.Dispose();
                DotNetInstance = null;
            }
        }
    }
}