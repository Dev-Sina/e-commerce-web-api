using eCommerceWebAPI.Application.Configuration.Queries;

namespace eCommerceWebAPI.Application.Customers.Queries.GetCustomer
{
    public class GetAllCustomersQuery : BasePaginationQuery, IQuery<IList<CustomerDto>>
    {
    }
}
