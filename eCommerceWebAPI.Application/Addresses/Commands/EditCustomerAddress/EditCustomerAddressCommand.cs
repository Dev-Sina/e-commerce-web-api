using eCommerceWebAPI.Application.Configuration.Commands;
using eCommerceWebAPI.Application.SeedWork;

namespace eCommerceWebAPI.Application.Addresses.Commands.EditCustomerAddress
{
    public class EditCustomerAddressCommand : CommandBase<BaseResponse>
    {
        public long CustomerId { get; set; }
        public EditAddressDto EditAddressDto { get; set; } = new();
    }
}
