using System.Collections.Generic;
using System.Linq;

namespace Delugional.Utility
{
    public static class Conversions
    {
        public static Dictionary<object, object> ToObjectDictionary<T,T1>(this IDictionary<T,T1> source)
        {
            return source.ToDictionary(kvp => (object) kvp.Key, kvp => (object)kvp.Value);
        } 
    }
}
