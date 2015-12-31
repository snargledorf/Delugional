using System.Collections.Generic;
using System.Threading.Tasks;
using Delugional.Utility;

namespace Delugional
{
    public sealed class Deluge
    {
        private readonly IDelugeConnection connection;

        public Deluge(IDelugeConnection connection)
        {
            this.connection = connection;
        }

        public async Task<string> AddTorrentFile(string fileName, byte[] fileDump, Dictionary<object, object> options = null)
        {
            string fileContents = Base64.Encode(fileDump);
            RpcCall rpcCall = new RpcCall.Builder("core.add_torrent_file")
                .AddArg(fileName)
                .AddArg(fileContents)
                .AddArg(options)
                .Build();

            object[] result = await connection.Call(rpcCall);

            return result?[0] as string;
        }

        public async Task<bool> Login(string username, string password)
        {
            RpcCall rpcCall = new RpcCall.Builder("daemon.login")
                .AddArg(username)
                .AddArg(password)
                .Build();

            object[] results = await connection.Call(rpcCall);

            // TODO Proper return type
            return results != null;
        }
    }
}
