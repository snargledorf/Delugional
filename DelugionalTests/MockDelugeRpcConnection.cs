using System.Collections.Generic;
using System.Threading.Tasks;
using Delugional.Rpc;

namespace DelugionalTests
{
    public class MockDelugeRpcConnection : DelugeRpcConnection
    {
        public override Task Send(int id, string method, IDictionary<string, object> kwargs, params object[] args)
        {
            // TODO Simulate calls
            return Task.Delay(0);
        }

        public override Task<RpcMessage[]> Receive()
        {
            // TODO Result result for calls
            return Task.FromResult(new RpcMessage[0]);
        }
    }
}