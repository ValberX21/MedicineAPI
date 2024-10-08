﻿using RabbitMQ.Client;
using System.Text;

namespace GoodAPI.Mensage
{
    public class RabbitMQService
    {
        private readonly IConnection _connection;

        public RabbitMQService(IConnection connection)
        {
            _connection = connection;
        }

        public void SendMessage(string message, string queueName)
        {
            using var channel = _connection.CreateModel();
            channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

            var body = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body);
        }
    }
}
