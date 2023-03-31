using eCommerceWebAPI.Application.Configuration.Commands;
using eCommerceWebAPI.Application.SeedWork;

namespace eCommerceWebAPI.Application.ShoppingCarts.Commands.UpdateShoppingCartItem
{
    public class UpdateShoppingCartItemCommand : CommandBase<BaseResponse>
    {
        public long CustomerId { get; set; }

        public UpdateShoppingCartItemDto UpdateShoppingCartItemDto { get; set; } = new();
    }
}
