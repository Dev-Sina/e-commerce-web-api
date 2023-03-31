using eCommerceWebAPI.Domain;
using eCommerceWebAPI.Domain.ShoppingCarts;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace eCommerceWebAPI.Infrastructure.Domain
{
    public class ShoppingCartRepository : IShoppingCartRepository
    {
        private readonly ConnectionMultiplexer _redis;
        private readonly string _redisConnectionString;
        private readonly string _hashSetName;

        public ShoppingCartRepository(ConnectionMultiplexer redis,
            string redisConnectionString)
        {
            _redis = redis;
            _redisConnectionString = redisConnectionString;

            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? string.Empty;
            if (environment.ToUpper() == "TEST")
            {
                _hashSetName = "test.cart";
            }
            else
            {
                _hashSetName = "cart";
            }
            _redisConnectionString = redisConnectionString;
        }

        public async Task<List<ShoppingCartItem>> GetShoppingCartAsync(long customerId)
        {
            var redisDb = _redis.GetDatabase();
            var shoppingCartItemsJson = await redisDb.StringGetAsync($"{_hashSetName}:{customerId}");

            if (!shoppingCartItemsJson.HasValue)
            {
                return await Task.FromResult(new List<ShoppingCartItem>());
            }

            List<ShoppingCartItem> shoppingCartItems = new();
            var redisShoppingCartItems = JsonConvert.DeserializeObject<List<ShoppingCartItem>>(shoppingCartItemsJson!) ?? new();
            redisShoppingCartItems.ForEach(redisShoppingCartItem =>
            {
                shoppingCartItems.Add(new()
                {
                    CustomerId = customerId,
                    ProductId = redisShoppingCartItem.ProductId,
                    ProductName = redisShoppingCartItem.ProductName,
                    ProductCode = redisShoppingCartItem.ProductCode,
                    ProductUnitPriceExcludeTax = redisShoppingCartItem.ProductUnitPriceExcludeTax,
                    ProductUnitPriceIncludeTax = redisShoppingCartItem.ProductUnitPriceIncludeTax,
                    ProductUnitDiscountAmountExcludeTax = redisShoppingCartItem.ProductUnitDiscountAmountExcludeTax,
                    ProductUnitDiscountAmountIncludeTax = redisShoppingCartItem.ProductUnitDiscountAmountIncludeTax,
                    Quantity = redisShoppingCartItem.Quantity,
                    Currency = redisShoppingCartItem.Currency
                });
            });

            return shoppingCartItems;
        }

        public async Task UpdateShoppingCartAsync(long customerId, List<ShoppingCartItemRedis> shoppingCartItemRedisList)
        {
            ArgumentException.ThrowIfNullOrEmpty(nameof(shoppingCartItemRedisList));

            var redisDb = _redis.GetDatabase();
            await redisDb.StringSetAsync($"{_hashSetName}:{customerId}", JsonConvert.SerializeObject(shoppingCartItemRedisList));
        }

        public async Task ClearShoppingCartAsync(long customerId)
        {
            var redisDb = _redis.GetDatabase();
            await redisDb.KeyDeleteAsync($"{_hashSetName}:{customerId}");
        }

        public async Task FlushAllShoppingCartsAsync()
        {
            // Get the Redis database
            var redisDb = _redis.GetDatabase();

            // Get the Redis server
            var server = _redis.GetServer(_redisConnectionString);

            // Get all shopping cart keys
            var keysToDelete = server.Keys(pattern: $"{_hashSetName}:*");

            // Delete all the matching keys
            redisDb.KeyDelete(keysToDelete.ToArray());
        }

    }
}
