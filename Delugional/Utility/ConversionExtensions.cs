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
    }
}
