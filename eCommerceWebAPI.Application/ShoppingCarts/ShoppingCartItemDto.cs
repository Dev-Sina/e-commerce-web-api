namespace eCommerceWebAPI.Application.ShoppingCarts
{
    public class ShoppingCartItemDto : ShoppingCartItemRedisDto
    {
        public long CustomerId { get; set; }
    }
}
