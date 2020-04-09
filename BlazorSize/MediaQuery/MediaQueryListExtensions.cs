using System.Collections.Generic;
using System.Linq;

namespace BlazorPro.BlazorSize
{
    public static class MediaQueryListExtensions
    {
        public static void ApplyChanges(this MediaQuery[] mq, MediaQueryEventArgs item)
        {
            var changed = mq.First(q => q.Media == item.Media);
            changed.Matches = item.Matches;
            changed.OnChange?.Invoke(item);
        }
    }
}
