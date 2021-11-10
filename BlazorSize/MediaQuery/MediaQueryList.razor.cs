using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BlazorPro.BlazorSize
{
    public partial class MediaQueryList
    {
        // IMediaQueryService
        [Inject] public IMediaQueryService MqService { get; set; } = null!;

        /// <summary>
        /// Application content where Media Query components may exist.
        /// </summary>
        [Parameter] public RenderFragment? ChildContent { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await MqService.CreateMediaQueryList(DotNetObjectReference.Create(this));
                await base.OnAfterRenderAsync(firstRender);
            }

        }

        public void AddQuery(MediaQuery newMediaQuery) => MqService.AddQuery(newMediaQuery);
        public async Task RemoveQuery(MediaQuery mediaQuery) => await MqService.RemoveQuery(mediaQuery);
        public async Task Initialize(MediaQuery mediaQuery) => await MqService.Initialize(mediaQuery);
        /// <summary>
        /// Called by JavaScript when a media query changes in the dom.
        /// </summary>
        /// <param name="args"></param>
        [JSInvokable(nameof(MediaQueryList.MediaQueryChanged))]
        public void MediaQueryChanged(MediaQueryArgs args)
        {
            // cache must be compared by actual value, not RequestedMedia when invoked from JavaScript
            // DOM Media value my be different that the initally requested media query value.
            var cache = MqService.MediaQueries.Find(q => q.Value?.Media == args.Media);

            if (cache is null) return;

            // Dispatch events to all subscribers
            foreach (var item in cache.MediaQueries)
            {
                item.MediaQueryChanged(args);
            }
        }
    }
}