using eCommerceWebAPI.Application.Orders;
using eCommerceWebAPI.Application.SeedWork;
using eCommerceWebAPI.Domain;
using eCommerceWebAPI.Domain.Catalogs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace eCommerceWebAPI.Application.Services.MessageBrokers.RabbitMQ
{
    public class OrderPlacedRabbitMQConsumerHostedService : RabbitMQConsumerHostedService<OrderPlacedRabbitMQMessageConfig, OrderDto>
    {
        private readonly IServiceProvider _serviceProvider;

        public OrderPlacedRabbitMQConsumerHostedService(OrderPlacedRabbitMQMessageConfig orderPlacedRabbitMQMessage,
            string connectionHostName,
            string connectionUserName,
            string connectionPassword,
            IServiceProvider serviceProvider)
                : base(orderPlacedRabbitMQMessage, connectionHostName, connectionUserName, connectionPassword)
        {
            _serviceProvider = serviceProvider;
        }

        public override async Task Handle(string? serializedMessage)
        {
            // Check serialized message parameter won't be null or empty
            if (string.IsNullOrEmpty(serializedMessage))
            {
                return;
            }

            // Deserialize message
            OrderDto? orderDto = new();
            try
            {
                orderDto = JsonConvert.DeserializeObject<OrderDto>(serializedMessage);
            }
            catch(Exception ex)
            {
                return;
            }

            // Check obtained object won't be null
            if (orderDto == null)
            {
                return;
            }

            // Check order identifier won't be wrong
            long orderId = orderDto.Id;
            if (orderId <= 0)
            {
                return;
            }

            // Check order item dtos won't be null or empty
            var orderItemDtos = orderDto.OrderItems;
            if (orderItemDtos == null || !orderItemDtos.Any())
            {
                return;
            }

            // Parallel injection of product repository
            using var scope = _serviceProvider.CreateScope();
            var productRepository = scope.ServiceProvider.GetService<IRepository<Product>>();
            if (productRepository == null)
            {
                return;
            }

            // Check products would be existed with given order items product identifiers
            var productIds = orderItemDtos.Select(oi => oi.ProductId).ToList();
            var products = await productRepository
                .AsQueryable()
                .Where(p => productIds.Contains(p.Id))
                .Distinct()
                .ToListAsync();
            if (!products.Any())
            {
                return;
            }

            // Reduce products stock quantities without saving changes
            foreach ( var orderItemDto in orderItemDtos)
            {
                var relatedProduct = products.FirstOrDefault(p => p.Id == orderItemDto.ProductId);
                if (relatedProduct == null)
                {
                    continue;
                }

                relatedProduct.StockQuantity -= orderItemDto.Quantity;
                await productRepository.Update(relatedProduct, false);
            }

            // Save changes
            await productRepository.SaveChangesAsync();
        }
    }
}
