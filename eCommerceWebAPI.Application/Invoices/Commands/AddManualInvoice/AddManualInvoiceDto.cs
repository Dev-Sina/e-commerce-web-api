namespace eCommerceWebAPI.Application.Invoices.Commands.AddManualInvoice
{
    public class AddManualInvoiceDto
    {
        public decimal OrderShippingCostExcludeTax { get; set; }

        public decimal OrderDiscountAmountExcludeTax { get; set; }

        public AddManualInvoiceAddressDto InvoiceAddress { get; set; } = new();

        public virtual List<AddManualInvoiceItemDto> InvoiceItems { get; set; } = new();
    }
}
