using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Delugional.Utility;
using MiscUtil.Conversion;
using rencodesharp;

namespace Delugional
{
    public interface IDelugeProtocol : IDisposable
    {
        void Close();
        Task Send(params RpcCall[] calls);
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
        }

        public abstract Task Send(params RpcCall[] calls);

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
    }

    public class DelugeProtocolV3 : DelugeProtocol
    {
        private const int BufferSize = 1024;

        private readonly List<byte> readBuffer = new List<byte>();

        private BinaryReader reader;
        private BinaryWriter writer;

        public DelugeProtocolV3(Stream stream) 
            : base(stream)
        {
            reader = new BinaryReader(stream, Encoding.ASCII);
            writer = new BinaryWriter(stream, Encoding.ASCII);
        }

        public override Task Send(params RpcCall[] calls)
        {
            object[] messages = CreateMessages(calls);
            string encoded = Rencode.Encode(messages);
            byte[] compressed = Zlib.Compress(encoded);

            return Task.Run(() => writer.Write(compressed));
        }

        public override Task<object[]> Receive()
        {
            return Task.Run(() =>
            {
                while (true)
                {
                    byte[] bytes = reader.ReadBytes(BufferSize);

                    if (bytes.Length == 0)
                        return null;

                    readBuffer.AddRange(bytes);

                    try
                    {
                        bytes = Zlib.Decompress(readBuffer.ToArray());

                        var result = Rencode.Decode(bytes) as object[];

                        if (result != null)
                        {
                            readBuffer.Clear();
                            return result;
                        }
                    }
                    catch
                    {
                        // Message is incomplete
                    }
                }
            });
        }

        public override void Close()
        {
            reader?.Close();
            reader?.Dispose();
            reader = null;

            writer?.Close();
            writer?.Dispose();
            writer = null;
        }

        private static object[] CreateMessages(IEnumerable<RpcCall> calls)
        {
            return calls.Select(CreateMessage).ToArray();
        }

        private static object CreateMessage(RpcCall call)
        {
            return new object[]
            {
                IdGenerator.Default.Next(),
                call.Method,
                call.Args,
                call.Kwargs
            };
        }
    }
}