using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMqHandlerService
{
    public class MqHandlerService<TMessage> : IMqHandlerService<TMessage> where TMessage : class
    {
        private IConnection mConnection;
        private IModel mChannel;

        private static readonly ConnectionFactory ConnectionFactory = new ConnectionFactory
        {
            VirtualHost = "/",
            HostName = "localhost",
            UserName = "guest",
            Password = "guest",
            Port = 5672
        };

        /// <summary>
        /// Имя очереди, к которой подключается Consumer для обработки
        /// </summary>
        private readonly string mQueueName;

        public MqHandlerService(string queueName)
        {
            mQueueName = queueName;
        }


        /// <summary>
        /// Обработка сообщения
        /// </summary>
        /// <param name="messageBody">Тело сообщения JSON</param>
        /// <param name="headers">В заголовки передаются параметры методов, которые необходимо вызвать</param>
        public void ProcessMessage(byte[] messageBody, IDictionary<string, object> headers)
        {
            var messageEntity = JsonSerializer.Deserialize(Encoding.UTF8.GetString(messageBody), typeof(TMessage));
            var methodName = Encoding.UTF8.GetString((byte[])headers["Method"]);
            MessageReceived?.Invoke(this, new MessageReceiveEventArgs(methodName, new []{messageEntity}));
        }

        public event EventHandler<MessageReceiveEventArgs> MessageReceived;
        public delegate void OnMessageReceived(object sender,MessageReceiveEventArgs e);


        /// <summary>
        /// Инициализируем обработчик
        /// </summary>
        public void InitializeHandler()
        {
            mConnection = ConnectionFactory.CreateConnection();
            mChannel = mConnection.CreateModel();
            var consumer = new EventingBasicConsumer(mChannel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var headers = ea.BasicProperties.Headers;
                ProcessMessage(body, headers);
            };

            mChannel.BasicConsume(queue: mQueueName,
                autoAck: true,
                consumer: consumer);
        }



        /// <summary>
        /// Закрываем соединение
        /// </summary>
        public void Dispose()
        {
            mConnection?.Dispose();
            mChannel?.Dispose();
        }


    }
}
