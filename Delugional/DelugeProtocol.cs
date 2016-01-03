using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Threading.Tasks;
using Delugional.Utility;
using rencodesharp;

namespace Delugional
{
    public interface IDelugeProtocol : IDisposable
    {
        void Close();
        Task Send(string method, object[] args = null, Dictionary<object, object> kwargs = null);
        Task<object[]> Receive();
    }

    public abstract class DelugeProtocol : IDelugeProtocol
    {
        private bool disposed;

        protected DelugeProtocol(Stream stream)
        {
            BaseStream = stream;
        }

        public Stream BaseStream { get; }

        public virtual void Close()
        {
            BaseStream?.Close();
            BaseStream?.Dispose();
        }

        public abstract Task Send(string method, object[] args = null, Dictionary<object, object> kwargs = null);

        public abstract Task<object[]> Receive();

        public void Dispose()
        {
            if (disposed)
                return;

            Dispose(true);
            GC.SuppressFinalize(this);

            disposed = true;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            Close();
        }

        ~DelugeProtocol()
        {
            Dispose(false);
        }

        public static Task<DelugeProtocol> Create(string host, int port, bool ssl)
        {
            return CreateV3Protocol(host, port, ssl);
        }

        private static async Task<DelugeProtocol> CreateV3Protocol(string host, int port, bool ssl)
        {
            var client = new TcpClient(host, port);

            Stream stream = client.GetStream();

            if (ssl)
            {
                var sslStream = new SslStream(stream, false, (sender, certificate, chain, errors) => true);
                await sslStream.AuthenticateAsClientAsync(host);
                stream = sslStream;
            }

            return new DelugeProtocolV3(stream);
        }
    }

    public class DelugeProtocolV3 : DelugeProtocol
    {
        private const int BufferSize = 1024;

        private static readonly Dictionary<object, object> EmptyKwargs = new Dictionary<object, object>();
        private static readonly object[] EmptyArgs = new object[0];

        private readonly List<byte> readBuffer = new List<byte>();

        public DelugeProtocolV3(Stream stream) 
            : base(stream)
        {
        }

        public override Task Send(string method, object[] args = null, Dictionary<object, object> kwargs = null)
        {
            object formatted = FormatRequestMessage(method, args, kwargs);
            string encoded = Rencode.Encode(formatted);
            byte[] encodedBytes = encoded.Select(Convert.ToByte).ToArray();
            byte[] compressed = Zlib.Deflate(encodedBytes);

            return BaseStream.WriteAsync(compressed, 0, compressed.Length);
        }

        public override async Task<object[]> Receive()
        {
            while (true)
            {
                var buffer = new byte[BufferSize];
                int read = await BaseStream.ReadAsync(buffer, 0, BufferSize);

                if (read == 0)
                    return null;

                IEnumerable<byte> bytesRead = buffer.Where((b, i) => i < read);
                readBuffer.AddRange(bytesRead);

                try
                {
                    buffer = Zlib.Inflate(readBuffer.ToArray());
                    readBuffer.Clear();

                    string encoded = string.Concat(buffer.Select(b => (char) b));
                    return Rencode.Decode(encoded) as object[];
                }
                catch
                {
                    // Message is incomplete
                }
            }
        }

        private static object FormatRequestMessage(string method, object[] args, Dictionary<object, object> kwargs)
        {
            return new object[]
            {
                new object[]
                {
                    IdGenerator.Default.Next(),
                    method,
                    args ?? EmptyArgs,
                    kwargs ?? EmptyKwargs
                }
            };
        }
    }
}