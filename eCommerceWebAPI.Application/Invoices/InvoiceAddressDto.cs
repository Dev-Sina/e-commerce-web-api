namespace eCommerceWebAPI.Application.Invoices
{
    public class InvoiceAddressDto
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

        public long InvoiceId { get; set; }

        public virtual InvoiceDto? Invoice { get; set; }
    }
}
