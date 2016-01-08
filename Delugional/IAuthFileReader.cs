using System;

namespace Delugional
{
    public interface IAuthFileReader : IDisposable
    {
        User Read();
        User[] ReadAll();
    }
}