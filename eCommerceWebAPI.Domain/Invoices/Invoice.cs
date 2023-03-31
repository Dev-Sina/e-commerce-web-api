namespace eCommerceWebAPI.Domain.Invoices
{
    /// <summary>
    /// Represents an invoice
    /// </summary>
    public class Invoice : BaseSqlEntity
    {
        private ICollection<InvoiceItem> _invoiceItems;

        /// <summary>
        /// Gets or sets the related order identifier
        /// </summary>
        public long OrderId { get; set; } 

        /// <summary>
        /// Gets or sets the order registered date on UTC
        /// </summary>
        public DateTime OrderCreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the order shipping cost without considering tax amount
        /// </summary>
        public decimal OrderShippingCostExcludeTax { get; set; }

        /// <summary>
        /// Gets or sets the order shipping cost with considering tax amount
        /// </summary>
        public decimal OrderShippingCostIncludeTax { get; set; }

        /// <summary>
        /// Gets or sets the discount which is applied directly on Order amount without considering tax amount
        /// </summary>
        public decimal OrderDiscountAmountExcludeTax { get; set; }

        /// <summary>
        /// Gets or sets the discount which is applied directly on Order amount with considering tax amount
        /// </summary>
        public decimal OrderDiscountAmountIncludeTax { get; set; }

        /// <summary>
        /// Gets or sets the all discount amounts which is applied Order without considering tax amount
        /// </summary>
        public decimal OrderTotalDiscountAmountExcludeTax { get; set; }

        /// <summary>
        /// Gets or sets the all discount amounts which is applied Order with considering tax amount
        /// </summary>
        public decimal OrderTotalDiscountAmountIncludeTax { get; set; }

        /// <summary>
        /// Gets or sets the order payable amount
        /// </summary>
        public decimal OrderPayableAmount { get; set; }
        
        /// <summary>
        /// Gets or sets the order prices currency
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// Gets or sets the invoice creation date and time
        /// </summary>
        public DateTime InvoiceCreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the invoice address
        /// </summary>
        public InvoiceAddress InvoiceAddress { get; set; }

        /// <summary>
        /// Gets or sets the collection of InvoiceItem
        /// </summary>
        public virtual ICollection<InvoiceItem> InvoiceItems
        {
            get => _invoiceItems ?? (_invoiceItems = new List<InvoiceItem>());
            protected set => _invoiceItems = value;
        }
    }
}
