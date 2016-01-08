using System.Collections.Generic;
using System.Linq;

namespace Delugional.Utility
{
    public static class ConversionExtensions
    {
        public static Dictionary<object, object> ToObjectDictionary<T,T1>(this IDictionary<T,T1> source)
        {
            return source.ToDictionary(kvp => (object) kvp.Key, kvp => (object)kvp.Value);
        }

        public static object[] ToObjectArray<T>(this IEnumerable<T> source)
        {
            return source.Cast<object>().ToArray();
        }

        public static IDictionary<string, object> ToDictionary(this Filter filter)
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
