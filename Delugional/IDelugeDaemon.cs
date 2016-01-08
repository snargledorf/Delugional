using System;

namespace Delugional
{
    public interface IDelugeDaemon : IDisposable
    {
        bool Running { get; }
        void Start();
        void Stop();
    }
}