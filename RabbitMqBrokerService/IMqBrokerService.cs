using System.Collections.Generic;

namespace RabbitMqBrokerService
{
    public interface IMqBrokerService
    {
        /// <summary>
        /// Создание подключения к очереди
        /// </summary>
        /// <param name="isCreatingNewQueue">Создать очередь</param>
        /// <param name="newQueueName">Имя очереди</param>
        void InitializeMqConnection(bool isCreatingNewQueue = false, string newQueueName = "new Queue");

        /// <summary>
        /// Отправить сообщение в очередь
        /// </summary>
        /// <param name="message">Сообщение</param>
        /// <param name="headers">Заголовки (метод, который вызвать в апи)</param>
        void SendMessage(object message, Dictionary<string, object> headers);

        /// <summary>
        /// Закрыть подключение
        /// </summary>
        void CloseMqConnection();
    }
}