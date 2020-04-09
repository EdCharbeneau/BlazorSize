using System;

namespace BlazorPro.BlazorSize
{
    public class MediaQuery
    {
        public MediaQuery(string media, bool matches = false)
        {
            Media = media;
            Matches = matches;
        }

        public string Media { get; }
        public bool Matches { get; set; }
#nullable enable
        public Action<MediaQueryEventArgs>? OnChange { get; set; }
#nullable disable
    }
}
