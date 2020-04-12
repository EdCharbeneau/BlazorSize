using System;

namespace BlazorPro.BlazorSize
{
    public class BrowserWindowSize : EventArgs
    {
        public int Height { get; set; }
        public int Width { get; set; }
    }
}
