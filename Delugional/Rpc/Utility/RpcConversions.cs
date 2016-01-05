using System.Collections.Generic;
using System.Linq;

namespace Delugional.Rpc.Utility
{
    internal sealed class RpcConversions
    {
        public static IDictionary<string, object> FilterToDictionary(Filter filter)
        {
            var dict = new Dictionary<string, object>();

            if (filter.Ids.Count > 0)
                dict.Add("id", filter.Ids.ToArray<object>());
            if (filter.States.Count > 0)
                dict.Add("state", filter.States.Select(s => s.ToString()).ToArray<object>());
            if (filter.Keywords.Count > 0)
                dict.Add("keyword", filter.Keywords.ToArray<object>());
            if (filter.TrackerHosts.Count > 0)
                dict.Add("tracker_host", filter.TrackerHosts.ToArray<object>());

            return dict;
        }
    }
}