using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitMqHandlerService
{
    /// <summary>
    /// Аргументы принятого сообщения, какой метод необходимо вызывать в API
    /// </summary>
    public class MessageReceiveEventArgs : EventArgs
    {
        public MessageReceiveEventArgs(string methodName, object[] parameters)
        {
            MethodName = methodName;
            Parameters = parameters;
        }

        public string MethodName { get; }
        public object[] Parameters { get; }
    }
}
