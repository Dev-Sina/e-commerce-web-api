namespace eCommerceWebAPI.Domain.ShoppingCarts
{
    /// <summary>
    /// Represents a shopping cart item redis
    /// </summary>
    public class ShoppingCartItemRedis : BaseRedisEntity
    {
        /// <summary>
        /// Gets or sets the shopping cart item product identifier
        /// </summary>
        public long ProductId { get; set; }

        /// <summary>
        /// Gets or sets the shopping cart item product name
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// Gets or sets the shopping cart item product code
        /// </summary>
        public string ProductCode { get; set; }

        /// <summary>
        /// Gets or sets the shopping cart item product unit price without considering tax amount
        /// </summary>
        public decimal ProductUnitPriceExcludeTax { get; set; }

        /// <summary>
        /// Gets or sets the shopping cart item product unit price with considering tax amount
        /// </summary>
        public decimal ProductUnitPriceIncludeTax { get; set; }

        /// <summary>
        /// Gets or sets the shopping cart item product unit discount amount without considering tax amount
        /// </summary>
        public decimal ProductUnitDiscountAmountExcludeTax { get; set; }

        /// <summary>
        /// Gets or sets the shopping cart item product unit discount amount with considering tax amount
        /// </summary>
        public decimal ProductUnitDiscountAmountIncludeTax { get; set; }

        /// <summary>
        /// Gets or sets the shopping cart item product quantity
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the shopping cart item product currency
        /// </summary>
        public string Currency { get; set; }
    }
}
