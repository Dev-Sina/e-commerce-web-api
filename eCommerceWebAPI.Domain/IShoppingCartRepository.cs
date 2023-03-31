using eCommerceWebAPI.Domain.ShoppingCarts;

namespace eCommerceWebAPI.Domain
{
    public interface IShoppingCartRepository
    {
        Task<List<ShoppingCartItem>> GetShoppingCartAsync(long customerId);

        Task UpdateShoppingCartAsync(long customerId, List<ShoppingCartItemRedis> shoppingCartItemRedisList);

        Task ClearShoppingCartAsync(long customerId);

        Task FlushAllShoppingCartsAsync();
    }
}
