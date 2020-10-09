using System.Collections.Generic;
using System.Text.Json;
using RabbitMQ.Client;

namespace RabbitMqBrokerService
{
    public class MqBrokerService : IMqBrokerService
    {
        private static readonly ConnectionFactory ConnectionFactory = new ConnectionFactory
        {
            VirtualHost = "/",
            HostName = "localhost",
            UserName = "guest",
            Password = "guest",
            Port = 5672
        };
        private IModel mChannel;
        private IConnection mConnection;
        private IBasicProperties mChannelProperties;
        private string mRoutingKey;

        private static string exchangeName = "Simple exchange";

        public MqBrokerService(string routingKey, bool isCreatingNewQueue = false, string newQueueName = "new Queue")
        {
            mRoutingKey = routingKey;
            InitializeMqConnection(isCreatingNewQueue, newQueueName);
        }

        /// <summary>
        /// Создание подключения к очереди
        /// </summary>
        /// <param name="isCreatingNewQueue">Создать очередь</param>
        /// <param name="newQueueName">Имя очереди</param>
        public void InitializeMqConnection(bool isCreatingNewQueue = false, string newQueueName = "new Queue")
        {
            mConnection = ConnectionFactory.CreateConnection();
            mChannel = mConnection.CreateModel();
            if (isCreatingNewQueue)
                CreateQueue(newQueueName);
            mChannelProperties = mChannel.CreateBasicProperties();
        }


        /// <summary>
        /// Создать очередь
        /// </summary>
        /// <param name="queueName">Имя очереди</param>
        private void CreateQueue(string queueName)
        {
            mChannel.ExchangeDeclare(exchangeName, ExchangeType.Direct, true, false);
            mChannel.QueueDeclare(queueName, true, false, false);
            mChannel.QueueBind(queueName, exchangeName, mRoutingKey);
        }

        /// <summary>
        /// Отправить сообщение в очередь
        /// </summary>
        /// <param name="message">Сообщение</param>
        /// <param name="headers">Заголовки (метод, который вызвать в апи)</param>
        public void SendMessage(object message, Dictionary<string, object> headers)
        {
            mChannelProperties.Headers = headers;
            mChannel.BasicPublish(exchangeName, mRoutingKey, mChannelProperties, JsonSerializer.SerializeToUtf8Bytes(message));
        }

        /// <summary>
        /// Закрыть подключение
        /// </summary>
        public void CloseMqConnection()
        {
            mChannel.Close();
            mConnection.Close();
        }
    }
}