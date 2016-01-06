using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Delugional.Rpc.Utility;
using Delugional.Utility;

namespace Delugional.Rpc
{
    public interface IDelugeRpc : IDeluge
    {
        Task<AuthLevels> LoginAsync(string username, string password);

        Task<object> Call(string method, params object[] args);
        Task<object> Call(string method, Dictionary<string, object> kwargs, params object[] args);
        Task<object> Call(RpcRequest request);
    }

    public class DelugeRpc : Deluge, IDelugeRpc
    {
        private readonly IDelugeRpcConnection connection;

        public DelugeRpc(IDelugeRpcConnection connection)
        {
            if (!connection.IsOpen)
                throw new InvalidOperationException("Connection not open");

            this.connection = connection;
        }

        public virtual async Task<AuthLevels> LoginAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Argument is null or whitespace", nameof(username));
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Argument is null or whitespace", nameof(password));
            
            return (AuthLevels)await Call("daemon.login", username, password);
        }

        public override async Task<string> AddTorrentAsync(string fileName, byte[] fileDump, IDictionary<string, object> options = null)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("Argument is null or whitespace", nameof(fileName));
            if (fileDump == null)
                throw new ArgumentNullException(nameof(fileDump));
            if (fileDump.Length == 0)
                throw new ArgumentException("Argument is empty collection", nameof(fileDump));

            string fileContents = Base64.Encode(fileDump);

            return await Call("core.add_torrent_file", fileName, fileContents, options?.ToObjectDictionary()) as string;
        }

        public override async Task<string> AddMagnetAsync(string url, IDictionary<string, object> options = null)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("Argument is null or whitespace", nameof(url));

            return await Call("core.add_torrent_magnet", url, options?.ToObjectDictionary()) as string;
        }

        public override async Task<bool> RemoveTorrentAsync(string torrentId, bool removeData = false)
        {
            if (string.IsNullOrWhiteSpace(torrentId))
                throw new ArgumentException("Argument is null or whitespace", nameof(torrentId));

            object result = await Call("core.remove_torrent", torrentId, removeData);

            return result is bool && (bool)result;
        }

        public override async Task<object[]> RemoveTorrentsAsync(string[] torrentIds, bool removeData = false)
        {
            if (torrentIds == null)
                throw new ArgumentNullException(nameof(torrentIds));
            if (torrentIds.Length == 0)
                throw new ArgumentException("Argument is empty collection", nameof(torrentIds));

            return await Call("core.remove_torrents", torrentIds.ToObjectArray(), removeData) as object[];
        }

        public override async Task<IDictionary<string, object>> GetTorrentStatusAsync(string torrentId, string[] statusKeys = null, bool diff = false)
        {
            if (string.IsNullOrWhiteSpace(torrentId))
                throw new ArgumentException("Argument is null or whitespace", nameof(torrentId));

            object result = await Call("core.get_torrent_status", torrentId, statusKeys.ToObjectArray(), diff);

            var statuses = (Dictionary<object, object>) result;
            return statuses?.ToDictionary(s => (string)s.Key, s => s.Value);
        }

        public override async Task<IDictionary<string, IDictionary<string, object>>> GetTorrentsStatusAsync(Filter filter = null, string[] statusKeys = null, bool diff = false)
        {
            Dictionary<object, object> filters = RpcConversions.FilterToDictionary(filter).ToObjectDictionary();

            object result = await Call("core.get_torrents_status", filters, statusKeys.ToObjectArray(), diff);
            if (result == null)
                return null;

            var dict = new Dictionary<string, IDictionary<string, object>>();
            var torrents = (Dictionary<object, object>)result;
            foreach (var torrent in torrents)
            {
                var torrentId = (string)torrent.Key;
                var statuses = (Dictionary<object, object>)torrent.Value;

                dict[torrentId] = statuses.ToDictionary(s => (string) s.Key, s => s.Value);
            }

            return dict;
        }

        public virtual Task<object> Call(string method, params object[] args)
        {
            return Call(method, null, args);
        }

        public virtual Task<object> Call(string method, Dictionary<string, object> kwargs, params object[] args)
        {
            if (string.IsNullOrWhiteSpace(method))
                throw new ArgumentException("Argument is null or whitespace", nameof(method));

            var request = new RpcRequest(method, args, kwargs);
            return Call(request);
        }

        public virtual async Task<object> Call(RpcRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            CheckDisposed();
            
            await connection.Send(request.Method, request.Kwargs, request.Args.ToArray());
            object[][] result = await connection.Receive();

            if (result == null)
            {
                Close();
                throw new Exception("Connection closed unexpectedly");
            }

            object[] messageParts = result.First();
            RpcMessage rpcMessage = RpcMessage.Create(messageParts);

            RpcResponse response = CheckResultMessage(rpcMessage);

            return response.Data;
        }

        public override void Close()
        {
            connection.Close();
            connection.Dispose();
        }

        protected RpcResponse CheckResultMessage(RpcMessage result)
        {
            var error = result as RpcError;
            if (error != null)
                throw new Exception(error.ExceptionMessage);

            var response = result as RpcResponse;
            if (response == null)
                throw new Exception("Invalid response type: " + result.GetType());

            return response;
        }
    }
}