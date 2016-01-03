using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Delugional.Utility;

namespace Delugional
{
    public interface IDeluge : IDisposable
    {
        bool IsOpen { get; }
        Task OpenAsync();
        void Close();
        Task<object> Call(RpcRequest request);
    }

    public sealed class Deluge : IDeluge
    {
        private readonly string host;
        private readonly int port;
        private readonly bool ssl;

        private DelugeProtocol protocol;

        private bool disposed;
        
        public Deluge(IPAddress ipAddress, int port=58846, bool ssl = true)
            : this (ipAddress.ToString(), port, ssl)
        {
        }

        public Deluge(IPEndPoint endPoint, bool ssl = true)
            : this (endPoint.Address, endPoint.Port, ssl)
        {
        }

        public Deluge(string host="localhost", int port=58846, bool ssl = true)
        {
            this.host = host;
            this.port = port;
            this.ssl = ssl;
        }

        public bool IsOpen => protocol != null;

        public IDelugeProtocol Protocol => protocol;

        public void Open()
        {
            OpenAsync().Wait();
        }

        public async Task OpenAsync()
        {
            CheckDisposed();

            if (IsOpen)
                throw new InvalidOperationException("Connection is already open");

            protocol = await DelugeProtocol.Create(host, port, ssl);
        }

        public async Task<AuthLevels> LoginAsync(string username, string password)
        {
            RpcRequest rpcRequest = new RpcRequest.Builder("daemon.login")
                .AddArg(username)
                .AddArg(password)
                .Build();

            return (AuthLevels)await Call(rpcRequest);
        }

        public async Task<string> AddTorrentFileAsync(string fileName, byte[] fileDump, Dictionary<string, object> options = null)
        {
            string fileContents = Base64.Encode(fileDump);
            RpcRequest request = new RpcRequest.Builder("core.add_torrent_file")
                .AddArg(fileName)
                .AddArg(fileContents)
                .AddArg(options?.ToObjectDictionary())
                .Build();

            return await Call(request) as string;
        }

        public async Task<string> AddMagnetAsync(string url, Dictionary<string, object> options = null)
        {
            RpcRequest request = new RpcRequest.Builder("core.add_torrent_magnet")
                   .AddArg(url)
                   .AddArg(options?.ToObjectDictionary())
                   .Build();

            return await Call(request) as string;
        }

        public async Task<object> Call(RpcRequest request)
        {
            CheckDisposed();
            
            await protocol.Send(request.Method, request.Args.ToArray(), request.Kwargs);
            object[] result = await protocol.Receive();

            if (result == null)
            {
                Close();
                throw new Exception("Connection closed unexpectedly");
            }

            RpcMessage rpcMessage = RpcMessage.Create(result);

            RpcResponse response = CheckResultMessage(rpcMessage);

            return response.Data;
        }

        public void Close()
        {
            protocol?.Close();
            protocol?.Dispose();
            protocol = null;
        }

        public void Dispose()
        {
            if (disposed)
                return;

            Dispose(true);
            GC.SuppressFinalize(this);

            disposed = true;
        }

        private void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            Close();
        }

        private void CheckDisposed()
        {
            if (disposed)
                throw new ObjectDisposedException(GetType().Name);
        }

        private RpcResponse CheckResultMessage(RpcMessage result)
        {
            var error = result as RpcError;
            if (error != null)
                throw new Exception(error.ExceptionMessage);

            var response = result as RpcResponse;
            if (response == null)
                throw new Exception("Invalid response type: " + result.GetType());

            return response;
        }

        ~Deluge()
        {
            Dispose(false);
        }
    }
}