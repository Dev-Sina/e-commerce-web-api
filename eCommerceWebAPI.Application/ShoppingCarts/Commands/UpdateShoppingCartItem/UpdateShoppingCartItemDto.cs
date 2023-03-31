namespace eCommerceWebAPI.Application.ShoppingCarts.Commands.UpdateShoppingCartItem
{
    public class UpdateShoppingCartItemDto
    {
        public long ProductId { get; set; }

        public int Quantity { get; set; }
    }
}
