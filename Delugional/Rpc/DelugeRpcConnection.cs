using System;
using System.Collections.Generic;
using System.Linq;
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
        Task Send(int id, string method, params object[] args);
        Task Send(int id, string method, IDictionary<string, object> kwargs, params object[] args);
        Task Send(RpcRequest request);
        Task Send(params RpcRequest[] requests);
        Task Send(IEnumerable<RpcRequest> requests);

        Task<RpcMessage[]> Receive();
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

        public virtual Task Send(string method, IDictionary<string, object> kwargs, params object[] args)
        {
            return Send(IdGenerator.Default.Next(), method, kwargs, args);
        }

        public virtual Task Send(int id, string method, params object[] args)
        {
            return Send(id, method, null, args);
        }

        public virtual Task Send(int id, string method, IDictionary<string, object> kwargs, params object[] args)
        {
            var request = new RpcRequest(id, method, args, kwargs);
            return Send(request);
        }

        public virtual Task Send(RpcRequest request)
        {
            return Task.FromResult(-1);
        }

        public virtual Task Send(params RpcRequest[] requests)
        {
            return Send((IEnumerable<RpcRequest>)requests);
        }

        public virtual async Task Send(IEnumerable<RpcRequest> requests)
        {
            foreach (RpcRequest request in requests)
            {
                await Send(request);
            }
        }

        public abstract Task<RpcMessage[]> Receive();

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