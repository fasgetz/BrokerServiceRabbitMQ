using System;
using System.Collections.Generic;
using RabbitMQ.Client.Events;

namespace RabbitMqHandlerService
{
    public interface IMqHandlerService<TMessage> : IDisposable where TMessage : class
    {
        /// <summary>
        /// Инициализировать обработчик
        /// </summary>
        void InitializeHandler();

        /// <summary>
        /// Сообщение
        /// </summary>
        /// <param name="messageBody">Тело сообщение</param>
        /// <param name="headers">Метод, который вызвать в апи</param>
        void ProcessMessage(byte[] messageBody, IDictionary<string, object> headers);

        /// <summary>
        /// Аргументы заголовка (метод, который вызвать в апи)
        /// </summary>

        event EventHandler<MessageReceiveEventArgs> MessageReceived;
    }
}
