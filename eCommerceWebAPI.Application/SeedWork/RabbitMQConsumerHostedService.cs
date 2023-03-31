using eCommerceWebAPI.Domain.SeedWork;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using Microsoft.Extensions.Hosting;

namespace eCommerceWebAPI.Application.SeedWork
{
    public abstract class RabbitMQConsumerHostedService<TRabbitMQMessageConfig, TData> : IHostedService, IDisposable
        where TRabbitMQMessageConfig : IRabbitMQMessageConfig
    {
        private readonly string _connectionHostName;
        private readonly string _connectionUserName;
        private readonly string _connectionPassword;
        private readonly IConnection _connection;
        private readonly string _queueName;
        private readonly IModel _channel;
        private readonly AsyncEventingBasicConsumer _consumer;

        public RabbitMQConsumerHostedService(TRabbitMQMessageConfig rabbitMQMessageConfig,
            string connectionHostName,
            string connectionUserName,
            string connectionPassword)
        {
            _connectionHostName = connectionHostName;
            _connectionUserName = connectionUserName;
            _connectionPassword = connectionPassword;

            _queueName = rabbitMQMessageConfig.QueueName;

            var factory = new ConnectionFactory { HostName = _connectionHostName, UserName = _connectionUserName, Password = _connectionPassword, DispatchConsumersAsync = true };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(exchange: rabbitMQMessageConfig.ExchangeName, type: rabbitMQMessageConfig.TypeOfExchange.ToString());
            _channel.QueueDeclare(queue: rabbitMQMessageConfig.QueueName, durable: true, exclusive: false, autoDelete: false);
            _channel.QueueBind(queue: rabbitMQMessageConfig.QueueName, exchange: rabbitMQMessageConfig.ExchangeName, routingKey: rabbitMQMessageConfig.RoutingKey);

            _consumer = new AsyncEventingBasicConsumer(_channel);
            _consumer.Received += OnMessageReceived;
        }

        public void Start()
        {
            _channel.BasicConsume(queue: _queueName, autoAck: false, consumer: _consumer);
        }

        private async Task OnMessageReceived(object sender, BasicDeliverEventArgs e)
        {
            try
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());

                Handle(message);
                
                _channel.BasicAck(e.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                _channel.BasicNack(e.DeliveryTag, multiple: false, requeue: true);
            }
        }

        public abstract Task Handle(string? serializedMessage);

        public void Dispose()
        {
            _channel.Dispose();
            _connection.Dispose();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Start();
            await Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            Dispose();
            await Task.CompletedTask;
        }
    }
}
