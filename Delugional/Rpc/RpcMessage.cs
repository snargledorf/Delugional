using System;

namespace Delugional.Rpc
{
    public class RpcResponse : RpcMessage
    {
        public RpcResponse(int id, object data)
            : base(id, MessageType.Response)
        {
            Data = data;
        }

        public object Data { get; }
    }

    public enum MessageType
    {
        Response = 1,
        Error = 2,
        Event = 3
    }

    public class RpcError : RpcMessage
    {
        public RpcError(int id, string exceptionType, string exceptionMessage, string traceback)
            : base(id, MessageType.Response)
        {
            ExceptionType = exceptionType;
            ExceptionMessage = exceptionMessage;
            Traceback = traceback;
        }

        public string ExceptionType { get; }

        public string ExceptionMessage { get; }

        public string Traceback { get; }
    }

    public class RpcEvent : RpcMessage
    {
        public RpcEvent(string eventName, object data) 
            : base(-1, MessageType.Response)
        {
            EventName = eventName;
            Data = data;
        }

        public string EventName { get; }

        public object Data { get; }
    }

    public abstract class RpcMessage
    {
        protected RpcMessage(MessageType type)
            : this(-1, type)
        {
        }

        protected RpcMessage(int id, MessageType type)
        {
            Id = id;
            Type = type;
        }

        public int Id { get; }

        public MessageType Type { get; }

        public static RpcMessage Create(object[] result)
        {
            var responseType = (MessageType)result[0];
            switch (responseType)
            {
                case MessageType.Response:
                    return CreateResponseMessage(result);
                case MessageType.Event:
                    return CreateEventMessage(result);
                case MessageType.Error:
                    return CreateErrorMessage(result);
            }

            throw new InvalidOperationException("Invalid response format: " + string.Join(",", result));
        }

        private static RpcResponse CreateResponseMessage(object[] result)
        {
            int id = (int)result[1];
            return new RpcResponse(id, result[2]);
        }

        private static RpcEvent CreateEventMessage(object[] result)
        {
            string eventName = (string) result[1];
            return new RpcEvent(eventName, result[2]);
        }

        private static RpcError CreateErrorMessage(object[] result)
        {
            int id = (int) result[1];

            var errorDetails = (object[]) result[2];
            var exceptionType = (string)errorDetails[0];
            var exceptionMessage = (string)errorDetails[1];
            var traceback = (string)errorDetails[2];

            return new RpcError(id, exceptionType, exceptionMessage, traceback);
        }
    }
}