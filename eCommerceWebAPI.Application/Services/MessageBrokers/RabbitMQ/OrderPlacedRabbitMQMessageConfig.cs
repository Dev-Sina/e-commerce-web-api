using eCommerceWebAPI.Domain.SeedWork;
using RabbitMQ.Client;

namespace eCommerceWebAPI.Application.Services.MessageBrokers.RabbitMQ
{
    public class OrderPlacedRabbitMQMessageConfig : IRabbitMQMessageConfig
    {
        private readonly string PropertiesValuesPrefix = string.Empty;

        public OrderPlacedRabbitMQMessageConfig()
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? string.Empty;
            if (environment.ToUpper() == "TEST")
            {
                PropertiesValuesPrefix = $"test_env__";
            }
        }

        public string ExchangeName => $"{PropertiesValuesPrefix}order_exchange";
        public string TypeOfExchange => ExchangeType.Topic;
        public string QueueName => $"{PropertiesValuesPrefix}ordre_placed_queue";
        public string RoutingKey => $"{PropertiesValuesPrefix}order.placed";
    }
}
