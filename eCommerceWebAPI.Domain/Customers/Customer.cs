using eCommerceWebAPI.Domain.Orders;

namespace eCommerceWebAPI.Domain.Customers
{
    /// <summary>
    /// Represents a customer
    /// </summary>
    public class Customer : BaseSoftDeleteSqlEntity
    {
        private ICollection<CustomerAddressMapping> _customerAddressMappings;
        private ICollection<Order> _orders;

        /// <summary>
        /// Gets or sets the customer first name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the customer last name
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the customer national code
        /// </summary>
        public string? NationalCode { get; set; }

        /// <summary>
        /// Gets or sets the customer registration date and time
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

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
