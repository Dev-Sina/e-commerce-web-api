using eCommerceWebAPI.Application.Addresses;

namespace eCommerceWebAPI.Application.Customers
{
    public class CustomerAddressMappingDto
    {
        public long Id { get; set; }

        public long CustomerId { get; set; }

        public long AddressId { get; set; }

        public virtual CustomerDto Customer { get; set; } = new();

        public virtual AddressDto Address { get; set; } = new();
    }
}
