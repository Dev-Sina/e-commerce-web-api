namespace eCommerceWebAPI.Application.Invoices
{
    public class InvoiceDto
    {
        public long Id { get; set; }

        public long OrderId { get; set; }

        public DateTime OrderCreatedOnUtc { get; set; }

        public decimal OrderShippingCostExcludeTax { get; set; }
        public string OrderShippingCostExcludeTaxNormalized => OrderShippingCostExcludeTax.ToString("N0") + " " + Currency;

        public decimal OrderShippingCostIncludeTax { get; set; }
        public string OrderShippingCostIncludeTaxNormalized => OrderShippingCostIncludeTax.ToString("N0") + " " + Currency;

        public decimal OrderDiscountAmountExcludeTax { get; set; }
        public string OrderDiscountAmountExcludeTaxNormalized => OrderDiscountAmountExcludeTax.ToString("N0") + " " + Currency;

        public decimal OrderDiscountAmountIncludeTax { get; set; }
        public string OrderDiscountAmountIncludeTaxNormalized => OrderDiscountAmountIncludeTax.ToString("N0") + " " + Currency;

        public decimal OrderTotalDiscountAmountExcludeTax { get; set; }
        public string OrderTotalDiscountAmountExcludeTaxNormalized => OrderTotalDiscountAmountExcludeTax.ToString("N0") + " " + Currency;

        public decimal OrderTotalDiscountAmountIncludeTax { get; set; }
        public string OrderTotalDiscountAmountIncludeTaxNormalized => OrderTotalDiscountAmountIncludeTax.ToString("N0") + " " + Currency;

        public decimal OrderPayableAmount { get; set; }
        public string OrderPayableAmountNormalized => OrderPayableAmount.ToString("N0") + " " + Currency;

        public string Currency { get; set; } = string.Empty;

        public DateTime InvoiceCreatedOnUtc { get; set; }

        public InvoiceAddressDto InvoiceAddress { get; set; } = new();

        public virtual List<InvoiceItemDto> InvoiceItems { get; set; } = new();
    }
}
