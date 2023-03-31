using eCommerceWebAPI.Application.Configuration.Queries;

namespace eCommerceWebAPI.Application.Addresses.Queries.GetAddress
{
    public class GetCustomerAddressesByCustomerIdQuery : BasePaginationQuery, IQuery<IList<AddressDto>>
    {
        public long CustomerId { get; set; }
    }
}
