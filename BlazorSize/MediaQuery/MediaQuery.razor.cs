using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace BlazorPro.BlazorSize
{
    public partial class MediaQuery : IAsyncDisposable
    {
        /// <summary>
        /// Media Query string, using standard media query syntax.
        /// Ex: "(max-width: 767.98px)"
        /// </summary>
        [Parameter] public string Media { get; set; } = "";

        /// <summary>
        /// The result of the specified Media property.
        /// Use the @bind-Media directive for automated binding.
        /// </summary>
        [Parameter] public bool Matches { get; set; }

        /// <summary>
        /// Handles the changed event when the media query's Matches property is changed.
        /// Use the @bind-Media directive for automated binding.
        /// </summary>
        [Parameter] public EventCallback<bool> MatchesChanged { get; set; }

        /// <summary>
        /// Optional template, shown only when the Matches value is true.
        /// Using a template will disable @bind-Matches for performance.
        /// </summary>
        [Parameter] public RenderFragment? Matched { get; set; }

        /// <summary>
        /// Optional template, shown only when the Matches value is false.
        /// Using a template will disable @bind-Matches for performance.
        /// </summary>
        [Parameter] public RenderFragment? Unmatched { get; set; }

        /// <summary>
        /// Gets the parent MediaQueryList container.
        /// </summary>
        [CascadingParameter] public MediaQueryList? MediaQueryList { get; set; }

        /// <summary>
        /// Set by the DOM, the InternalMedia represents the actual Media Query value held by the DOM.
        /// </summary>
        public MediaQueryArgs InternalMedia { get; private set; } = new MediaQueryArgs();

        protected override void OnInitialized()
        {
            if (MediaQueryList == null)
            {
                throw new Exception("MediaQueryList is null. The MediaQueryList component should be added to the root or MainLayout of your applicaiton.");
            }
            else
            {
                MediaQueryList.AddQuery(this);
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (MediaQueryList == null) return;
            if (firstRender)
            {
                await MediaQueryList.Initialize(this);
            }
        }

        /// <summary>
        /// Used by the MediaQueryList. Invokes the full media query changed event.
        /// </summary>
        /// <param name="args">MediaQueryArgs</param>
        public void MediaQueryChanged(MediaQueryArgs args)
        {
            MatchesChanged.InvokeAsync(args.Matches);
            InternalMedia = args;
            if (Matched != null || Unmatched != null)
            {
                StateHasChanged();
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (MediaQueryList == null) return;

            try
            {
                await MediaQueryList.RemoveQuery(this);
            }
            catch (JSDisconnectedException jsEx)
            {
                // Ignore 
                //https://github.com/EdCharbeneau/BlazorSize/issues/93
            }

        }
    }
}