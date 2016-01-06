using System;

namespace Delugional.Rpc
{
    public class RpcErrorException : Exception
    {
        public RpcErrorException(RpcError error)
            : base(error.ExceptionMessage)
        {
            Error = error;
        }

        public RpcError Error { get; }
    }
}