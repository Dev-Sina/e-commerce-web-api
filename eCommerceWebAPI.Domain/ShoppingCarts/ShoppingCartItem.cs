namespace eCommerceWebAPI.Domain.ShoppingCarts
{
    /// <summary>
    /// Represents a shopping cart item
    /// </summary>
    public class ShoppingCartItem : ShoppingCartItemRedis
    {
        /// <summary>
        /// Gets or sets the shopping cart item customer identifier
        /// </summary>
        public long CustomerId { get; set; }
    }
}
