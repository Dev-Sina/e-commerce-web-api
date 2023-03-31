namespace eCommerceWebAPI.Application.Invoices.Commands.AddManualInvoice
{
    public class AddManualInvoiceAddressDto
    {
        public string? Title { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string CountryName { get; set; } = string.Empty;

        public string ProvinceName { get; set; } = string.Empty;

        public string CityName { get; set; } = string.Empty;

        public string StreetAddress { get; set; } = string.Empty;

        public string? PostalCode { get; set; }

        public string PhoneNumber { get; set; } = string.Empty;
    }
}
