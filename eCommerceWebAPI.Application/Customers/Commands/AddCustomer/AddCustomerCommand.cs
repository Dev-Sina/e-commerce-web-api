using eCommerceWebAPI.Application.Configuration.Commands;
using eCommerceWebAPI.Application.SeedWork;

namespace eCommerceWebAPI.Application.Customers.Commands.AddCustomer
{
    public class AddCustomerCommand : CommandBase<BaseResponse>
    {
        public AddCustomerDto AddCustomerDto { get; set; } = new();
    }
}
