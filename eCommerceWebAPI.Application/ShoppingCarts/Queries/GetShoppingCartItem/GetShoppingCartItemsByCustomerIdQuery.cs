using eCommerceWebAPI.Application.Configuration.Queries;

namespace eCommerceWebAPI.Application.ShoppingCarts.Queries.GetShoppingCartItem
{
    public class GetShoppingCartItemsByCustomerIdQuery : IQuery<IList<ShoppingCartItemDto>>
    {
        public long CustomerId { get; set; }
    }
}
