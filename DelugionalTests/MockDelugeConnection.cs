using System.Linq;
using System.Threading.Tasks;
using Delugional;

namespace DelugionalTests
{
    public class MockDelugeConnection : IDelugeConnection
    {
        public bool IsOpen { get; private set; }

        public Task Open()
        {
            return Task.Delay(30).ContinueWith(t => IsOpen = true);
        }

        public void Close()
        {
            IsOpen = false;
        }

        public Task<object[]> Call(params RpcCall[] calls)
        {
            object[] results = GetResults(calls);
            return Task.FromResult(results);
        }

        private object[] GetResults(RpcCall[] calls)
        {
            return calls.Select(GetResults)
                .Cast<object>()
                .ToArray();
        }

        private object[] GetResults(RpcCall call)
        {
            switch (call.Method)
            {
                case "add_torrent_file":
                    return new object[] { string.Empty };
            }

            return Enumerable.Empty<object>().ToArray();
        }

        public void Dispose()
        {
            Close();
        }
    }
}