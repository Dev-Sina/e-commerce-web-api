using eCommerceWebAPI.Application.Orders;
using eCommerceWebAPI.Application.SeedWork;

namespace eCommerceWebAPI.Application.Services.MessageBrokers.RabbitMQ
{
    public class OrderPlacedRabbitMQPublisher : RabbitMQPublisher<OrderPlacedRabbitMQMessageConfig, OrderDto>
    {
        public OrderPlacedRabbitMQPublisher(OrderPlacedRabbitMQMessageConfig orderPlacedRabbitMQMessageConfig,
            string connectionHostName,
            string connectionUserName,
            string connectionPassword)
                : base(orderPlacedRabbitMQMessageConfig, connectionHostName, connectionUserName, connectionPassword)
        {
        }

        public override void Publish<T>(T orderDto)
        {
            if (orderDto is not OrderDto)
            {
                return;
            }

            base.Publish(orderDto);
        }
    }
}
