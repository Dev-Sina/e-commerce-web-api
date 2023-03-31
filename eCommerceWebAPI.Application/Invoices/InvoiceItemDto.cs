namespace eCommerceWebAPI.Application.Invoices
{
    public class InvoiceItemDto
    {
        public long Id { get; set; }

        public long OrderId { get; set; }

        public long ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public string ProductCode { get; set; } = string.Empty;

        public decimal ProductUnitPriceExcludeTax { get; set; }
        public string ProductUnitPriceExcludeTaxNormalized => ProductUnitPriceExcludeTax.ToString("N0") + " IRT";

        public decimal ProductUnitPriceIncludeTax { get; set; }
        public string ProductUnitPriceIncludeTaxNormalized => ProductUnitPriceIncludeTax.ToString("N0") + " IRT";

        public decimal ProductUnitDiscountAmountExcludeTax { get; set; }
        public string ProductUnitDiscountAmountExcludeTaxNormalized => ProductUnitDiscountAmountExcludeTax.ToString("N0") + " IRT";

        public decimal ProductUnitDiscountAmountIncludeTax { get; set; }
        public string ProductUnitDiscountAmountIncludeTaxNormalized => ProductUnitDiscountAmountIncludeTax.ToString("N0") + " IRT";

        public int Quantity { get; set; }

        public long InvoiceId { get; set; }

        public virtual InvoiceDto Invoice { get; set; } = new();
    }
}
