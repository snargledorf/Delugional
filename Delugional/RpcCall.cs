using System.Collections.Generic;
using System.Threading.Tasks;

namespace Delugional
{
    public sealed class RpcCall
    {
        public RpcCall(string method, IDelugeConnection connection)
        {
            Method = method;
            Connection = connection;
        }

        public IDelugeConnection Connection { get; }

        public string Method { get; }

        public Dictionary<object, object> Options { get; } = new Dictionary<object, object>();

        public List<object> Args { get; } = new List<object>();

        public Task<object[]> Execute()
        {
            return Connection.Send(this);
        }
    }
}
