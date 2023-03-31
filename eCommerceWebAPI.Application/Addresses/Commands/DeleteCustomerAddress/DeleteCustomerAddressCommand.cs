using eCommerceWebAPI.Application.Configuration.Commands;
using eCommerceWebAPI.Application.SeedWork;

namespace eCommerceWebAPI.Application.Addresses.Commands.DeleteCustomerAddress
{
    public class DeleteCustomerAddressCommand : CommandBase<BaseResponse>
    {
        public long CustomerId { get; set; }
        public long AddressId { get; set; }
    }
}
