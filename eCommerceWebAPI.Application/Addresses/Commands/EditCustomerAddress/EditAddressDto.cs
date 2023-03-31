namespace eCommerceWebAPI.Application.Addresses.Commands.EditCustomerAddress
{
    public class EditAddressDto
    {
        public long Id { get; set; }

        public string? Title { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public long CityId { get; set; }

        public string StreetAddress { get; set; } = string.Empty;

        public string? PostalCode { get; set; }

        public string PhoneNumber { get; set; } = string.Empty;
    }
}
