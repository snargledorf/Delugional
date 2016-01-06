using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Threading.Tasks;
using Delugional.Utility;
using Ionic.Zlib;
using rencodesharp;

namespace Delugional.Rpc
{
    public class DelugeRpcConnectionV3 : DelugeRpcConnection
    {
        private const int BufferSize = 4096;

        private static readonly Dictionary<object, object> EmptyKwargs = new Dictionary<object, object>();
        private static readonly object[] EmptyArgs = new object[0];

        private readonly List<byte> readBuffer = new List<byte>();

        private readonly string host;
        private readonly int port;

        private Stream stream;
        private TcpClient client;

        public DelugeRpcConnectionV3(IPEndPoint endPoint)
            : this(endPoint.Address, endPoint.Port)
        {
        }

        public DelugeRpcConnectionV3(IPAddress ipAddress, int port = 58846)
            : this (ipAddress.ToString(), port)
        {
        }

        public DelugeRpcConnectionV3(string host = "localhost", int port = 58846)
        {
            this.host = host;
            this.port = port;
        }

        public override bool IsOpen => stream != null && client != null;

        public Stream Stream => stream;

        public override async Task OpenAsync()
        {
            CheckDisposed();

            var client = new TcpClient(host, port);
            
            var sslStream = new SslStream(client.GetStream(), false, (sender, certificate, chain, errors) => true);
            await sslStream.AuthenticateAsClientAsync(host);

            stream = sslStream;
            this.client = client;
        }

        public override async Task Send(string method, IDictionary<string, object> kwargs, params object[] args)
        {
            CheckDisposed();

            object formatted = FormatRequestMessage(method, kwargs, args);
            string encoded = Rencode.Encode(formatted);
            byte[] encodedBytes = encoded.Select(Convert.ToByte).ToArray();

            using (var zlib = new ZlibStream(Stream, CompressionMode.Compress, true))
            {
                await zlib.WriteAsync(encodedBytes, 0, encodedBytes.Length);
            }
        }

        public override async Task<object[][]> Receive()
        {
            CheckDisposed();

            var buffer = new byte[BufferSize];

            while (true)
            {
                int read = await stream.ReadAsync(buffer, 0, BufferSize);
                if (read == 0)
                    return null;

                IEnumerable<byte> bytesRead = buffer.Where((b, i) => i < read);
                readBuffer.AddRange(bytesRead);

                try
                {
                    byte[] inflated = Zlib.Inflate(readBuffer.ToArray());
                    string encoded = string.Concat(inflated.Select(Convert.ToChar));
                    var result = Rencode.Decode(encoded) as object[];
                    if (result == null)
                        return null;

                    var messages = new List<object[]>();

                    const int partsPerMessage = 3;
                    for (int skip = 0; skip < result.Length; skip+= partsPerMessage)
                    {
                        object[] message = result.Skip(skip).Take(partsPerMessage).ToArray();
                        messages.Add(message);
                    }

                    readBuffer.Clear();

                    return messages.ToArray();
                }
                catch
                {
                    // Message is incomplete
                }
            }
        }

        public override void Close()
        {
            stream?.Close();
            stream?.Dispose();
            stream = null;

            client?.Close();
            client = null;
        }

        private static object FormatRequestMessage(string method, IDictionary<string, object> kwargs, object[] args)
        {
            return new object[]
            {
                new object[]
                {
                    IdGenerator.Default.Next(),
                    method,
                    args ?? EmptyArgs,
                    kwargs?.ToObjectDictionary() ?? EmptyKwargs
                }
            };
        }
    }
}