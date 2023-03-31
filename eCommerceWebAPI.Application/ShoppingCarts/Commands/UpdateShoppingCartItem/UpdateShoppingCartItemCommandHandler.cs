using AutoMapper;
using eCommerceWebAPI.Application.Configuration.Commands;
using eCommerceWebAPI.Application.SeedWork;
using eCommerceWebAPI.Domain;
using eCommerceWebAPI.Domain.Catalogs;
using eCommerceWebAPI.Domain.Customers;
using eCommerceWebAPI.Domain.ShoppingCarts;
using System.Net;

namespace eCommerceWebAPI.Application.ShoppingCarts.Commands.UpdateShoppingCartItem
{
    public class UpdateShoppingCartItemCommandHandler : ICommandHandler<UpdateShoppingCartItemCommand, BaseResponse>
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IShoppingCartRepository _shoppingCartRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IMapper _mapper;

        public UpdateShoppingCartItemCommandHandler(IRepository<Product> productRepository,
            IShoppingCartRepository shoppingCartRepository,
            IRepository<Customer> customerRepository,
            IMapper mapper)
        {
            _productRepository = productRepository;
            _shoppingCartRepository = shoppingCartRepository;
            _customerRepository = customerRepository;
            _mapper = mapper;
        }

        public async Task<BaseResponse> Handle(UpdateShoppingCartItemCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(nameof(request));

            var updateShoppingCartItemDto = request.UpdateShoppingCartItemDto;
            ArgumentNullException.ThrowIfNull(nameof(updateShoppingCartItemDto));

            BaseResponse response = new();

            // Check customer would be existede
            long customerId = request.CustomerId;
            var customer = await _customerRepository.Get(customerId);
            if (customer == null || customer.Deleted)
            {
                response.Errors.Add($"No customer with the ID of : {customerId} is existed!");
                response.HttpStatusCode = HttpStatusCode.BadRequest;
                return response;
            }

            // Check updating item's product would be existed
            long productId = updateShoppingCartItemDto.ProductId;
            var product = await _productRepository.Get(productId);
            if (product == null || product.Deleted)
            {
                response.Errors.Add($"No product with the ID of : {productId} is existed!");
                response.HttpStatusCode = HttpStatusCode.BadRequest;
                return response;
            }

            // Bad request parameters
            if (updateShoppingCartItemDto.Quantity == 0) response.Errors.Add("Quantity Can not be zero!");
            if (response.Errors.Any())
            {
                response.HttpStatusCode = HttpStatusCode.BadRequest;
                return response;
            };

            // Check requested product is in-stock or not
            if (product.StockQuantity <= 0 && updateShoppingCartItemDto.Quantity > 0)
            {
                response.Errors.Add($"This product is not available right now and is ran out!");
                response.HttpStatusCode = HttpStatusCode.BadRequest;
                return response;
            }

            // Check requested product is in-stock or not
            if (updateShoppingCartItemDto.Quantity > product.StockQuantity)
            {
                response.Errors.Add($"You can add at last {product.StockQuantity} of this product to the cart.");
                response.HttpStatusCode = HttpStatusCode.BadRequest;
                return response;
            }

            // Get product in customer's shopping cart
            var customerShoppingCartItems = await _shoppingCartRepository.GetShoppingCartAsync(customerId);
            var productExistingInShoppingCart = customerShoppingCartItems.Where(sci => sci.ProductId == productId).ToList();
            int productQuantityInShoppingCart = productExistingInShoppingCart.Any() ? productExistingInShoppingCart.Select(sci => sci.Quantity).ToList().Sum() : 0;
            int finalQuantity = updateShoppingCartItemDto.Quantity + productQuantityInShoppingCart;
            if (finalQuantity < 0)
            {
                finalQuantity = 0;
            }
            if (finalQuantity > product.StockQuantity)
            {
                response.Errors.Add($"You can add at last {product.StockQuantity} of this product to the cart.");
                response.HttpStatusCode = HttpStatusCode.BadRequest;
                return response;
            }
            var otherShoppingCartItemsExisting = customerShoppingCartItems.Where(sci => sci.ProductId != productId).ToList();

            // Create shopping cart item redis list entities
            List<ShoppingCartItemRedis> shoppingCartItemRedisList = new();
            var otherShoppingCartItemRedisListExisting = _mapper.Map<List<ShoppingCartItemRedis>>(otherShoppingCartItemsExisting);
            shoppingCartItemRedisList.AddRange(otherShoppingCartItemRedisListExisting);
            if (finalQuantity > 0)
            {
                shoppingCartItemRedisList.Add(new()
                {
                    ProductId = productId,
                    ProductName = product.Name,
                    ProductCode = product.Code,
                    ProductUnitPriceExcludeTax = product.Price,
                    ProductUnitPriceIncludeTax = product.Price * (decimal)1.09,
                    ProductUnitDiscountAmountExcludeTax = product.DiscountAmount,
                    ProductUnitDiscountAmountIncludeTax = product.DiscountAmount * (decimal)1.09,
                    Quantity = finalQuantity,
                    Currency = product.Currency
                });
            }

            // Set shopping cart item redis in the Redis Db
            await _shoppingCartRepository.UpdateShoppingCartAsync(customerId, shoppingCartItemRedisList);

            // Return the result
            response.HttpStatusCode = HttpStatusCode.OK;
            response.Message = "The customer cart has been updated successfully!";
            return response;
        }
    }
}
