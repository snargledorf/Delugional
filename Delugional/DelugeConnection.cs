using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Delugional
{
    public interface IDelugeConnection : IDisposable
    {
        bool IsOpen { get; }
        Task Open();
        void Close();
        Task<object[]> Send(params RpcCall[] calls);
    }

    public sealed class DelugeConnection : IDelugeConnection
    {
        private readonly string host;
        private readonly int port;
        private readonly bool ssl;

        private TcpClient client;
        private Stream stream;
        private DelugeProtocol protocol;

        private bool disposed;

        public DelugeConnection()
            : this(IPAddress.Loopback, 58846)
        {
        }
        
        public DelugeConnection(IPAddress ipAddress, int port, bool ssl = true)
            : this (ipAddress.ToString(), port, ssl)
        {
        }

        public DelugeConnection(IPEndPoint endPoint, bool ssl = true)
            : this (endPoint.Address, endPoint.Port, ssl)
        {
        }

        public DelugeConnection(string host, int port, bool ssl = true)
        {
            this.host = host;
            this.port = port;
            this.ssl = ssl;
        }

        public bool IsOpen => client != null;

        public async Task Open()
        {
            CheckDisposed();

            if (IsOpen)
                throw new InvalidOperationException("Connection is already open");

            var client = new TcpClient();

            await client.ConnectAsync(host, port);

            Stream stream = client.GetStream();

            if (ssl)
                stream = await CreateSslStream(stream);

            protocol = new DelugeProtocolV3(stream);

            this.stream = stream;
            this.client = client;
        }

        private async Task<SslStream> CreateSslStream(Stream stream)
        {
            var sslStream = new SslStream(stream, false, (sender, certificate, chain, errors) => true);
            await sslStream.AuthenticateAsClientAsync(host);
            return sslStream;
        }

        public void Close()
        {
            client?.Close();
            client = null;

            stream?.Close();
            stream?.Dispose();
            stream = null;

            protocol?.Close();
            protocol?.Dispose();
            protocol = null;
        }

        public async Task<object[]> Send(params RpcCall[] calls)
        {
            CheckDisposed();

            await protocol.Write(calls);
            return await protocol.Read();
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

        ~DelugeConnection()
        {
            Dispose(false);
        }
    }
}