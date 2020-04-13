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

        [CascadingParameter] public MediaQueryList MediaQueryList { get; set; }

        public MediaQueryArgs InternalMedia { get; private set; } = new MediaQueryArgs();

        private DotNetObjectReference<MediaQuery> DotNetInstance;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                DotNetInstance = DotNetObjectReference.Create(this);
                InternalMedia = await Js.InvokeAsync<MediaQueryArgs>($"{ns}.addMediaQuery", MediaQueryList.DotNetInstance, DotNetInstance, Media);
                Console.WriteLine($"[internal] { InternalMedia.Media }");
                MediaQueryList.AddQuery(this);
                MediaQueryChanged(InternalMedia);
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        public void MediaQueryChanged(MediaQueryArgs args)
        {

            if (args.Media == InternalMedia.Media)
            {
                InternalMedia.Matches = args.Matches;
                MatchesChanged.InvokeAsync(args.Matches);
                if (Matched != null || Unmatched != null)
                {
                    StateHasChanged();
                }
            }
        }

        public void Dispose()
        {
            if (DotNetInstance != null)
            {
                Js.InvokeVoidAsync($"{ns}.removeMediaQuery", MediaQueryList.DotNetInstance, DotNetInstance);
                DotNetInstance.Dispose();
                DotNetInstance = null;
            }
        }
    }
}