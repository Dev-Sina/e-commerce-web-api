using Autofac;
using eCommerceWebAPI.Application.Orders;
using eCommerceWebAPI.Application.Services.MessageBrokers.RabbitMQ;
using eCommerceWebAPI.Domain.SeedWork;

namespace eCommerceWebAPI.Infrastructure.Processing
{
    public class MessageBrokerModule : Module
    {
        private readonly string _rabbitMQHostName;
        private readonly string _rabbitMQUserName;
        private readonly string _rabbitMQPassword;
        
        public MessageBrokerModule(string rabbitMQHostName,
            string rabbitMQUserName,
            string rabbitMQPassword)
        {
            _rabbitMQHostName = rabbitMQHostName;
            _rabbitMQUserName = rabbitMQUserName;
            _rabbitMQPassword = rabbitMQPassword;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<OrderPlacedRabbitMQPublisher>()
                .As<IRabbitMQPublisher<OrderDto>>()
                .WithParameter("orderPlacedRabbitMQMessageConfig", new OrderPlacedRabbitMQMessageConfig())
                .WithParameter("connectionHostName", _rabbitMQHostName)
                .WithParameter("connectionUserName", _rabbitMQUserName)
                .WithParameter("connectionPassword", _rabbitMQPassword)
                .InstancePerLifetimeScope();
        }
    }
}
