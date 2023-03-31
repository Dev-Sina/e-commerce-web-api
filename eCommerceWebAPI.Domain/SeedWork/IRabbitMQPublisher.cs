namespace eCommerceWebAPI.Domain.SeedWork
{
    public interface IRabbitMQPublisher<T> where T : class
    {
        void Publish<T>(T messageObject);
    }
}
