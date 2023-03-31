using eCommerceWebAPI.Domain.Orders;

namespace eCommerceWebAPI.Domain.Invoices
{
    /// <summary>
    /// Represents an invoice item
    /// </summary>
    public class InvoiceItem : BaseSqlEntity
    {
        /// <summary>
        /// Gets or sets the order identifier
        /// </summary>
        public long OrderId { get; set; }

        /// <summary>
        /// Gets or sets the product identifier
        /// </summary>
        public long ProductId { get; set; }

        /// <summary>
        /// Gets or sets the order item product name
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// Gets or sets the order item product code
        /// </summary>
        public string ProductCode { get; set; }

        /// <summary>
        /// Gets or sets the order item product unit price without considering tax amount
        /// </summary>
        public decimal ProductUnitPriceExcludeTax { get; set; }

        /// <summary>
        /// Gets or sets the order item product unit price with considering tax amount
        /// </summary>
        public decimal ProductUnitPriceIncludeTax { get; set; }

        /// <summary>
        /// Gets or sets the order item product unit discount amount without considering tax amount
        /// </summary>
        public decimal ProductUnitDiscountAmountExcludeTax { get; set; }

        /// <summary>
        /// Gets or sets the order item product unit discount amount with considering tax amount
        /// </summary>
        public decimal ProductUnitDiscountAmountIncludeTax { get; set; }

        /// <summary>
        /// Gets or sets the order item product quantity
        /// </summary>
        public int Quantity { get; set; }

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
