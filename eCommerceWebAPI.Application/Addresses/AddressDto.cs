using eCommerceWebAPI.Application.Customers;
using eCommerceWebAPI.Application.Orders;

namespace eCommerceWebAPI.Application.Addresses
{
    public partial class AddressDto
    {
        public long Id { get; set; }

        public string? Title { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string CountryName { get; set; } = string.Empty;

        public string ProvinceName { get; set; } = string.Empty;

        public string CityName { get; set; } = string.Empty;

        public string StreetAddress { get; set; } = string.Empty;

        public string? PostalCode { get; set; }

        public string PhoneNumber { get; set; } = string.Empty;

        public virtual List<CustomerAddressMappingDto> CustomerAddressMappings { get; set; } = new();

        public virtual List<OrderDto> Orders { get; set; } = new();
    }
}
