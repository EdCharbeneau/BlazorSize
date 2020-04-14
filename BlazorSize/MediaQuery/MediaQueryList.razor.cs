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
        private List<MediaQuery> mediaQueries = new List<MediaQuery>();

        public void AddQuery(MediaQuery mq)
        {
            mediaQueries.Add(mq);
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

        [JSInvokable(nameof(MediaQueryList.MediaQueryChanged))]
        public void MediaQueryChanged(MediaQueryArgs args)
        {
            foreach (var item in mediaQueries)
            {
                if (item.InternalMedia.Media == args.Media)
                {
                    item.MediaQueryChanged(args);
                }
            }
        }
        public void Dispose()
        {
            if (DotNetInstance != null)
            {
                foreach (var item in mediaQueries)
                {
                    item.Dispose();
                }
                Js.InvokeVoidAsync($"{ns}.removeMediaQueryList", DotNetInstance);
                DotNetInstance.Dispose();
                DotNetInstance = null;
            }
        }
    }
}