using System.Collections.Generic;
namespace BlazorPro.BlazorSize
{
    public class MediaQueryCache
    {
        /// <summary>
        /// Initally requested (unmodified) media query.
        /// </summary>
        public string? MediaRequested { get; set; }

        /// <summary>
        /// Is this item already awating a JavaScript interop call.
        /// </summary>
        public bool Loading { get; set; }

        /// <summary>
        /// The actual value represented by the DOM. This may differ from the initially requested media query.
        /// </summary>
        public MediaQueryArgs? Value { get; set; }

        /// <summary>
        /// Media Queries that share a RequestedMedia value. Used to aggregate event handlers and minimize JS calls.
        /// </summary>
        // Nullable because Blazor's lifecycle sometimes GC's this object before the cleanup routines are finished.
        public List<MediaQuery>? MediaQueries { get; set; } = new List<MediaQuery>();
    }
}

