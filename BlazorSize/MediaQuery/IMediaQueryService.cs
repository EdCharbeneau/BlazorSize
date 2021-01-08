using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorPro.BlazorSize
{
    public interface IMediaQueryService
    {
        List<MediaQueryCache> MediaQueries { get; }

        void AddQuery(MediaQuery newMediaQuery);
        Task CreateMediaQueryList(DotNetObjectReference<MediaQueryList> dotNetObjectReference);

        Task Initialize(MediaQuery mediaQuery);
        Task RemoveQuery(MediaQuery mediaQuery);

    }
}