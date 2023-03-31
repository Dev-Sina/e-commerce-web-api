using eCommerceWebAPI.Domain.SeedWork;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace eCommerceWebAPI.Application.SeedWork
{
    public class RabbitMQPublisher<TRabbitMQMessageConfig, T> : IRabbitMQPublisher<T>
        where TRabbitMQMessageConfig : IRabbitMQMessageConfig
        where T : class
    {
        private readonly string _connectionHostName;
        private readonly string _connectionUserName;
        private readonly string _connectionPassword;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly string _exchangeName;
        private readonly string _routingKey;

        public RabbitMQPublisher(TRabbitMQMessageConfig rabbitMQMessageConfig,
            string connectionHostName,
            string connectionUserName,
            string connectionPassword)
        {
            _connectionHostName = connectionHostName;
            _connectionUserName = connectionUserName;
            _connectionPassword = connectionPassword;

            _exchangeName = rabbitMQMessageConfig.ExchangeName;
            _routingKey = rabbitMQMessageConfig.RoutingKey;

            var factory = new ConnectionFactory { HostName = _connectionHostName, UserName = _connectionUserName, Password = _connectionPassword, DispatchConsumersAsync = true };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(exchange: rabbitMQMessageConfig.ExchangeName, type: rabbitMQMessageConfig.TypeOfExchange.ToString());
            _channel.QueueDeclare(queue: rabbitMQMessageConfig.QueueName, durable: true, exclusive: false, autoDelete: false);
            _channel.QueueBind(queue: rabbitMQMessageConfig.QueueName, exchange: rabbitMQMessageConfig.ExchangeName, routingKey: rabbitMQMessageConfig.RoutingKey);
        }

        public virtual void Publish<T>(T messageObject)
        {
            string messageObjectStringified = JsonConvert.SerializeObject(messageObject, Formatting.Indented,
                new JsonSerializerSettings
                {
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects
                });

            var body = Encoding.UTF8.GetBytes(messageObjectStringified);

            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;

            _channel.BasicPublish(exchange: _exchangeName, routingKey: _routingKey, basicProperties: properties, body: body);
        }
    }
}
