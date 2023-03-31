using eCommerceWebAPI.Application.Configuration.Commands;
using eCommerceWebAPI.Application.SeedWork;

namespace eCommerceWebAPI.Application.Invoices.Commands.AddSystemInvoice
{
    public class AddSystemInvoiceCommand : CommandBase<BaseResponse>
    {
        public long OrderId { get; set; }
    }
}
