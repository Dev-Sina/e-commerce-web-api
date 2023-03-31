namespace eCommerceWebAPI.Domain.SeedWork
{
    public interface IRabbitMQMessageConfig
    {
        string ExchangeName { get; }

        string TypeOfExchange { get; }

        string QueueName { get; }

        string RoutingKey { get; }
    }
}
