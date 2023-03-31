using eCommerceWebAPI.Domain.Addresses;

namespace eCommerceWebAPI.Domain.Invoices
{
    /// <summary>
    /// Represents an invoice address
    /// </summary>
    public class InvoiceAddress : BaseSqlEntity
    {
        /// <summary>
        /// Gets or sets the address title
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// Gets or sets the address first name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the address last name
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the country name
        /// </summary>
        public string CountryName { get; set; }

        /// <summary>
        /// Gets or sets the province name
        /// </summary>
        public string ProvinceName { get; set; }

        /// <summary>
        /// Gets or sets the city name
        /// </summary>
        public string CityName { get; set; }

        /// <summary>
        /// Gets or sets the StreetAddress
        /// </summary>
        public string StreetAddress { get; set; }

        /// <summary>
        /// Gets or sets the postal code
        /// </summary>
        public string? PostalCode { get; set; }

        /// <summary>
        /// Gets or sets the phone number
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets the invoice identifier
        /// </summary>
        public long InvoiceId { get; set; }

        /// <summary>
        /// Gets or sets the invoice
        /// </summary>
        public virtual Invoice Invoice { get; set; }
    }
}
