using eCommerceWebAPI.Application.Configuration.Commands;
using eCommerceWebAPI.Application.SeedWork;

namespace eCommerceWebAPI.Application.Invoices.Commands.AddManualInvoice
{
    public class AddManualInvoiceCommand : CommandBase<BaseResponse>
    {
        public long OrderId { get; set; }

        public AddManualInvoiceDto AddManualInvoiceDto { get; set; } = new();
    }
}
