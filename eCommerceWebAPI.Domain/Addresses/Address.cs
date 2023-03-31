using eCommerceWebAPI.Domain.Customers;
using eCommerceWebAPI.Domain.Orders;

namespace eCommerceWebAPI.Domain.Addresses
{
    /// <summary>
    /// Represents an address
    /// </summary>
    public partial class Address : BaseSqlEntity
    {
        private ICollection<CustomerAddressMapping> _customerAddressMappings;
        private ICollection<Order> _orders;

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
        /// Gets or sets the collection of CustomerAddressMapping
        /// </summary>
        public virtual ICollection<CustomerAddressMapping> CustomerAddressMappings
        {
            get => _customerAddressMappings ?? (_customerAddressMappings = new List<CustomerAddressMapping>());
            protected set => _customerAddressMappings = value;
        }

        /// <summary>
        /// Gets or sets the collection of Order
        /// </summary>
        public virtual ICollection<Order> Orders
        {
            get => _orders ?? (_orders = new List<Order>());
            protected set => _orders = value;
        }
    }
}
