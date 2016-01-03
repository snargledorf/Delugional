using System.Threading.Tasks;
using Delugional;

namespace DelugionalTests
{
    public class MockDeluge : IDeluge
    {
        public bool IsOpen { get; private set; }

        public Task OpenAsync()
        {
            return Task.Delay(30).ContinueWith(t => IsOpen = true);
        }

        public void Close()
        {
            IsOpen = false;
        }

        public Task<object> Call(RpcRequest request)
        {
            object results = GetResult(request);
            return Task.FromResult(results);
        }

        private object GetResult(RpcRequest request)
        {
            switch (request.Method)
            {
                case "add_torrent_file":
                    return string.Empty;
            }

            return new object();
        }

        public void Dispose()
        {
            Close();
        }
    }
}