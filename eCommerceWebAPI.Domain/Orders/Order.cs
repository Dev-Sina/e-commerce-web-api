using eCommerceWebAPI.Domain.Addresses;
using eCommerceWebAPI.Domain.Customers;

namespace eCommerceWebAPI.Domain.Orders
{
    /// <summary>
    /// Represents an order
    /// </summary>
    public class Order : BaseSoftDeleteSqlEntity
    {
        private ICollection<OrderItem> _orderItems;

        /// <summary>
        /// Gets or sets the customer identifier
        /// </summary>
        public long CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the order registered date on UTC
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the order shipping cost without considering tax amount
        /// </summary>
        public decimal ShippingCostExcludeTax { get; set; }

        /// <summary>
        /// Gets or sets the order shipping cost with considering tax amount
        /// </summary>
        public decimal ShippingCostIncludeTax { get; set; }

        /// <summary>
        /// Gets or sets the discount which is applied directly on Order amount without considering tax amount
        /// </summary>
        public decimal OrderDiscountAmountExcludeTax { get; set; }

        /// <summary>
        /// Gets or sets the discount which is applied directly on Order amount with considering tax amount
        /// </summary>
        public decimal OrderDiscountAmountIncludeTax { get; set; }

        /// <summary>
        /// Gets or sets the order prices currency
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// Gets or sets the order address identifier
        /// </summary>
        public long AddressId { get; set; }

        /// <summary>
        /// Gets or sets the customer
        /// </summary>
        public virtual Customer Customer { get; set; }

        /// <summary>
        /// Gets or sets the order address
        /// </summary>
        public virtual Address Address { get; set; }

        /// <summary>
        /// Gets or sets the collection of OrderItem
        /// </summary>
        public virtual ICollection<OrderItem> OrderItems
        {
            get => _orderItems ?? (_orderItems = new List<OrderItem>());
            protected set => _orderItems = value;
        }
    }
}
