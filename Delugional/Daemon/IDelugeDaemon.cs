using System;
using System.Threading.Tasks;
using Delugional.Rpc;

namespace Delugional.Daemon
{
    public interface IDelugeDaemon : IDisposable
    {
        bool Running { get; }
        bool Start();
        void Stop();

        IDelugeRpc OpenRpc();
        Task<IDelugeRpc> OpenRpcAsync();
    }
}