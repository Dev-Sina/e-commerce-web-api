using eCommerceWebAPI.Application.Configuration.Commands;
using eCommerceWebAPI.Application.SeedWork;

namespace eCommerceWebAPI.Application.Addresses.Commands.AddCustomerAddress
{
    public class AddCustomerAddressCommand : CommandBase<BaseResponse>
    {
        public long CustomerId { get; set; }
        public AddAddressDto AddAddressDto { get; set; } = new();
    }
}
