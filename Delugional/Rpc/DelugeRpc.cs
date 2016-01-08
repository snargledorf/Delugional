using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Delugional.Utility;
using Whenables;

namespace Delugional.Rpc
{
    public interface IDelugeRpc : IDeluge
    {
        Task<object> Call(string method, params object[] args);
        Task<object> Call(int id, string method, params object[] args);
        Task<object> Call(string method, IDictionary<string, object> kwargs, params object[] args);
        Task<object> Call(int id, string method, IDictionary<string, object> kwargs, params object[] args);
        Task<object> Call(RpcRequest request);
        Task<object[]> Call(params RpcRequest[] requests);
        Task<object[]> Call(IEnumerable<RpcRequest> requests);
        Task<AuthLevels> LoginAsync(string username, string password);
    }

    public class DelugeRpc : Deluge, IDelugeRpc
    {
        private readonly WhenableDictionary<int, RpcMessage> receivedMessages = new WhenableDictionary<int, RpcMessage>();

        public DelugeRpc(IDelugeRpcConnection connection)
        {
            if (!connection.IsOpen)
                throw new InvalidOperationException("Connection not open");

            Connection = connection;

            BeginReceiving();
        }

        public IDelugeRpcConnection Connection { get; }

        public override void Close()
        {
            Connection.Close();
            Connection.Dispose();
        }

        protected RpcResponse CheckResponse(RpcMessage result)
        {
            var error = result as RpcError;
            if (error != null)
                throw new RpcErrorException(error);

            return (RpcResponse)result;
        }

        protected IEnumerable<RpcResponse> CheckResponses(RpcMessage[] messages)
        {
            var exceptions = new List<Exception>();
            var responses = new List<RpcResponse>();

            foreach (RpcMessage message in messages)
            {
                try
                {
                    RpcResponse response = CheckResponse(message);
                    responses.Add(response);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }

            if (exceptions.Any())
                throw new AggregateException(exceptions);

            return responses;
        }

        private async void BeginReceiving()
        {
            try
            {
                while (Connection.IsOpen)
                {
                    RpcMessage[] messages = await Connection.Receive();

                    foreach (RpcMessage message in messages)
                    {
                        if (message.Type == MessageType.Response || message.Type == MessageType.Error)
                        {
                            receivedMessages.Add(message.Id, message);
                        }
                        else if (message.Type == MessageType.Event)
                        {
                            // TODO Events
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private Task<RpcMessage> CreateReceiveMessageTask(RpcRequest request)
        {
            return receivedMessages.WhenAdded(id => id == request.Id)
                .GetAsync()
                .ContinueWith(t =>
                {
                    KeyValuePair<int, RpcMessage> result = t.Result;
                    receivedMessages.Remove(result.Key);
                    return result.Value;
                });
        }

        public override async Task<string> AddMagnetAsync(string url, IDictionary<string, object> options = null)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("Argument is null or whitespace", nameof(url));

            return await Call("core.add_torrent_magnet", url, options?.ToObjectDictionary()) as string;
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

        public override async Task<IDictionary<string, IDictionary<string, object>>> GetTorrentsStatusAsync(Filter filter = null, string[] statusKeys = null, bool diff = false)
        {
            Dictionary<object, object> filters = filter.ToDictionary().ToObjectDictionary();

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

        public override async Task<IDictionary<string, object>> GetTorrentStatusAsync(string torrentId, string[] statusKeys = null, bool diff = false)
        {
            if (string.IsNullOrWhiteSpace(torrentId))
                throw new ArgumentException("Argument is null or whitespace", nameof(torrentId));

            object result = await Call("core.get_torrent_status", torrentId, statusKeys.ToObjectArray(), diff);

            var statuses = (Dictionary<object, object>) result;
            return statuses?.ToDictionary(s => (string)s.Key, s => s.Value);
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

        public virtual Task<object> Call(string method, params object[] args)
        {
            return Call(method, null, args);
        }

        public virtual Task<object> Call(int id, string method, params object[] args)
        {
            return Call(method, null, args);
        }

        public virtual Task<object> Call(string method, IDictionary<string, object> kwargs, params object[] args)
        {
            return Call(IdGenerator.Default.Next(), method, kwargs, args);
        }

        public virtual Task<object> Call(int id, string method, IDictionary<string, object> kwargs, params object[] args)
        {
            if (string.IsNullOrWhiteSpace(method))
                throw new ArgumentException("Argument is null or whitespace", nameof(method));

            var request = new RpcRequest(id, method, args, kwargs);
            return Call(request);
        }

        public virtual async Task<object> Call(RpcRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            object[] results = await Call(new[] {request});
            return results.First();
        }

        public virtual Task<object[]> Call(params RpcRequest[] requests)
        {
            return Call((IEnumerable<RpcRequest>) requests);
        }

        public virtual async Task<object[]> Call(IEnumerable<RpcRequest> requests)
        {
            if (requests == null)
                throw new ArgumentNullException(nameof(requests));

            requests = requests.ToArray();

            if (!requests.Any())
                throw new ArgumentException("Argument is empty collection", nameof(requests));

            if (requests.Any(request => request == null))
                throw new ArgumentException("Argument contains null items", nameof(requests));

            CheckDisposed();

            await Connection.Send(requests);

            IEnumerable<Task<RpcMessage>> getTasks =
                requests.Select(CreateReceiveMessageTask);

            RpcMessage[] messages = await Task.WhenAll(getTasks);

            IEnumerable<RpcResponse> responses = CheckResponses(messages);

            return responses.Select(response => response.Data).ToArray();
        }

        public virtual async Task<AuthLevels> LoginAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Argument is null or whitespace", nameof(username));
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Argument is null or whitespace", nameof(password));
            
            return (AuthLevels)await Call("daemon.login", username, password);
        }
    }
}