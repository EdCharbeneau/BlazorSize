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

        protected override void OnInitialized()
        {
            MediaQueryList.AddQuery(this);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await MediaQueryList.Initialize(this);
            }
        }

        public void MediaQueryChanged(MediaQueryArgs args)
        {
            MatchesChanged.InvokeAsync(args.Matches);
            InternalMedia = args;
            if (Matched != null || Unmatched != null)
            {
                StateHasChanged();
            }
        }

        public void Dispose()
        {
            MediaQueryList.RemoveQuery(this);
        }
    }
}