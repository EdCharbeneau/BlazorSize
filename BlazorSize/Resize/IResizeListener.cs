using System;
using System.Threading.Tasks;

namespace BlazorPro.BlazorSize
{
    public interface IResizeListener
    {
        event EventHandler<BrowserWindowSize> OnResized;

        ValueTask<BrowserWindowSize> GetBrowserWindowSize();
        ValueTask<bool> MatchMedia(string mediaQuery);
    }
}