using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Delugional.Rpc
{
    public interface IDelugeRpcConnection : IDisposable
    {
        bool IsOpen { get; }

        Task OpenAsync();
        void Close();

        Task Send(string method, params object[] args);
        Task Send(string method, IDictionary<string, object> kwargs, params object[] args);
        Task<object[]> Receive();
    }

    public abstract class DelugeRpcConnection : IDelugeRpcConnection
    {
        private bool disposed;

        public virtual bool IsOpen => false;

        public virtual Task OpenAsync()
        {
            return Task.Delay(0);
        }

        public virtual void Close()
        {
        }

        public virtual Task Send(string method, params object[] args)
        {
            return Send(method, null, args);
        }

        public abstract Task Send(string method, IDictionary<string, object> kwargs, params object[] args);

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

        protected void CheckDisposed()
        {
            if (disposed)
                throw new ObjectDisposedException(GetType().Name);
        }

        ~DelugeRpcConnection()
        {
            Dispose(false);
        }
    }
}