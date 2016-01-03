using System;
using System.Collections.Generic;

namespace Delugional
{
    public sealed class RpcRequest
    {
        private RpcRequest(string method, List<object> args, Dictionary<object, object> kwargs)
        {
            Method = method;
            Args = args;
            Kwargs = kwargs;
        }

        public string Method { get; }

        public Dictionary<object, object> Kwargs { get; }

        public List<object> Args { get; }

        public class Builder
        {
            private readonly string method;

            private readonly List<object> args = new List<object>();

            private readonly Dictionary<object, object> kwargs = new Dictionary<object, object>(); 

            public Builder(string method)
            {
                this.method = method;
            }

            public Builder AddArg(object arg)
            {
                args.Add(arg);
                return this;
            }

            public Builder AddKeywordArg(object key, object value)
            {
                kwargs.Add(key, value);
                return this;
            }

            public RpcRequest Build()
            {
                return new RpcRequest(method, args, kwargs);
            }
        }
    }

    public class RpcResponse : RpcMessage
    {
        public RpcResponse(int id, object data)
            : base(id, MessageType.Response)
        {
            Data = data;
        }

        public object Data { get; }
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
            string exceptionType = (string) result[2];
            string exceptionMessage = (string) result[3];
            string traceback = (string) result[4];

            return new RpcError(id, exceptionType, exceptionMessage, traceback);
        }
    }

    public enum MessageType
    {
        Response = 1,
        Error = 2,
        Event = 3
    }
}
