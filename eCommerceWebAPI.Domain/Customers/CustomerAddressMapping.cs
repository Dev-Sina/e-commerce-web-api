using eCommerceWebAPI.Domain.Addresses;

namespace eCommerceWebAPI.Domain.Customers
{
    /// <summary>
    /// Represents a customer address mapping
    /// </summary>
    public class CustomerAddressMapping : BaseSqlEntity
    {
        /// <summary>
        /// Gets or sets the customer identifier
        /// </summary>
        public long CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the address identifier
        /// </summary>
        public long AddressId { get; set; }

        /// <summary>
        /// Gets or sets the customer
        /// </summary>
        public virtual Customer Customer { get; set; }

        /// <summary>
        /// Gets or sets the address
        /// </summary>
        public virtual Address Address { get; set; }
    }
}
