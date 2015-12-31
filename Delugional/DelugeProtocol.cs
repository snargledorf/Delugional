using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Delugional.Utility;
using rencodesharp;

namespace Delugional
{
    public interface IDelugeProtocol : IDisposable
    {
        void Close();
        Task Write(RpcCall[] calls);
        Task<object[]> Read();
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

        public abstract Task Write(RpcCall[] calls);

        public abstract Task<object[]> Read();

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

        private StreamReader reader;
        private StreamWriter writer;

        public DelugeProtocolV3(Stream stream) 
            : base(stream)
        {
            reader = new StreamReader(stream);
            writer = new StreamWriter(stream);
        }

        public override Task Write(RpcCall[] calls)
        {
            object[] messages = CreateMessages(calls);
            string encodedMessages = Rencode.Encode(messages);
            string compressedMessages = Zlib.Compress(encodedMessages);

            return writer.WriteAsync(compressedMessages);
        }

        public override async Task<object[]> Read()
        {
            var buffer = new char[BufferSize];

            int charactersRead = await reader.ReadAsync(buffer, 0, BufferSize);

            var data = new string(buffer, 0, charactersRead);

            string decompressedMessage = Zlib.Decompress(data);

            return Rencode.Decode(decompressedMessage) as object[];
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
            return calls.Select(call => new object[]
            {
                IdGenerator.Default.Next(),
                call.Method,
                call.Args,
                call.Options
            })
                .Cast<object>()
                .ToArray();
        }
    }
}