using eCommerceWebAPI.Application.Orders;

namespace eCommerceWebAPI.Application.Customers
{
    public class CustomerDto
    {
        public long Id { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string? NationalCode { get; set; }

        public DateTime CreatedOnUtc { get; set; }

        public bool Deleted { get; set; } = false;

        public virtual List<CustomerAddressMappingDto> CustomerAddressMappings { get; set; } = new();

        public virtual List<OrderDto> Orders { get; set; } = new();
    }
}
