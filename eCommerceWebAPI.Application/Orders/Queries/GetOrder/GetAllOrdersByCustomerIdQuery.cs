using eCommerceWebAPI.Application.Configuration.Queries;

namespace eCommerceWebAPI.Application.Orders.Queries.GetOrder
{
    public class GetAllOrdersByCustomerIdQuery : BasePaginationQuery, IQuery<IList<OrderDto>>
    {
        public long CustomerId { get; set; }
    }
}
