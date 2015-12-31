using System.Collections.Generic;

namespace Delugional
{
    public sealed class RpcCall
    {
        public RpcCall()
        {
        }

        private RpcCall(string method, List<object> args, Dictionary<object, object> kwargs)
        {
            Method = method;
            Args = args;
            Kwargs = kwargs;
        }

        public string Method { get; }

        public Dictionary<object, object> Kwargs { get; } = new Dictionary<object, object>();

        public List<object> Args { get; } = new List<object>();

        public class Builder
        {
            private readonly string method;

            private readonly List<object> args = new List<object>();

            private readonly Dictionary<object, object> kwargs = new Dictionary<object, object>(); 

            public Builder(string method)
            {
                this.method = method;
            }

            public Builder AddArg(object arg)
            {
                args.Add(arg);
                return this;
            }

            public Builder AddKeywordArg(object key, object value)
            {
                kwargs.Add(key, value);
                return this;
            }

            public RpcCall Build()
            {
                return new RpcCall(method, args, kwargs);
            }
        }
    }
}
