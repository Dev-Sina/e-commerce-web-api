using eCommerceWebAPI.Application.Configuration.Commands;
using eCommerceWebAPI.Application.SeedWork;

namespace eCommerceWebAPI.Application.Orders.Commands.AddOrder
{
    public class AddOrderCommand : CommandBase<BaseResponse>
    {
        public long CustomerId { get; set; }

        public AddOrderDto AddOrderDto { get; set; } = new();
    }
}
