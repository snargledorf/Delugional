using System;
using Delugional.Rpc;

namespace Delugional
{
    public interface IDelugeDaemon : IDisposable
    {
        bool Running { get; }
        bool Start();
        void Stop();

        IDelugeRpc GetRpc();
    }
}