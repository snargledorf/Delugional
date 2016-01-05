using System.Collections.Generic;
using System.Threading.Tasks;
using Delugional.Rpc;

namespace DelugionalTests
{
    public class MockDelugeRpcConnection : DelugeRpcConnection
    {
        public override Task Send(string method, IDictionary<string, object> kwargs, params object[] args)
        {
            // TODO Simulate calls
            return Task.Delay(0);
        }

        public override Task<object[]> Receive()
        {
            // TODO Result result for calls
            return Task.FromResult(new object[0]);
        }
    }
}