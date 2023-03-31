namespace eCommerceWebAPI.Application.Invoices.Commands.AddManualInvoice
{
    public class AddManualInvoiceItemDto
    {
        public long ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public string ProductCode { get; set; } = string.Empty;

        public decimal ProductUnitPriceExcludeTax { get; set; }

        public decimal ProductUnitDiscountAmountExcludeTax { get; set; }

        public int Quantity { get; set; }
    }
}
