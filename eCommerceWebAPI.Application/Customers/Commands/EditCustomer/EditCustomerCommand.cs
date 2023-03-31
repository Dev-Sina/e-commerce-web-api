using eCommerceWebAPI.Application.Configuration.Commands;
using eCommerceWebAPI.Application.SeedWork;

namespace eCommerceWebAPI.Application.Customers.Commands.EditCustomer
{
    public class EditCustomerCommand : CommandBase<BaseResponse>
    {
        public long CustomerId { get; set; }
        public EditCustomerDto EditCustomerDto { get; set; } = new();
    }
}
