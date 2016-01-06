using System;
using System.Collections.Generic;
using System.Linq;

namespace Delugional.Rpc
{
    public sealed class RpcRequest
    {
        public RpcRequest(string method, IEnumerable<object> args = null, IDictionary<string, object> kwargs = null)
            : this(IdGenerator.Default.Next(), method, args, kwargs)
        {
        }

        public RpcRequest(int id, string method, IEnumerable<object> args = null, IDictionary<string, object> kwargs = null)
        {
            if (string.IsNullOrWhiteSpace(method))
                throw new ArgumentException("Argument is null or whitespace", nameof(method));

            Id = id;
            Method = method;
            Args = args?.ToList() ?? new List<object>();
            Kwargs = kwargs ?? new Dictionary<string, object>();
        }

        public int Id { get; }

        public string Method { get; }

        public IDictionary<string, object> Kwargs { get; }

        public IList<object> Args { get; }

        public class Builder
        {
            private readonly string method;

            private readonly List<object> args = new List<object>();

            private readonly Dictionary<string, object> kwargs = new Dictionary<string, object>(); 

            public Builder(string method)
            {
                this.method = method;
            }

            public Builder AddArg(object arg)
            {
                args.Add(arg);
                return this;
            }

            public Builder AddKeywordArg(string key, object value)
            {
                kwargs.Add(key, value);
                return this;
            }

            public RpcRequest Build()
            {
                return new RpcRequest(method, args, kwargs);
            }
        }
    }
}
