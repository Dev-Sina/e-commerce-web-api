using eCommerceWebAPI.Application.Configuration.Queries;

namespace eCommerceWebAPI.Application.Invoices.Queries.GetInvoice
{
    public class GetAllInvoicesQuery : BasePaginationQuery, IQuery<IList<InvoiceDto>>
    {
    }
}
