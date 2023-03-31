using AutoMapper;
using eCommerceWebAPI.Application.Configuration.Commands;
using eCommerceWebAPI.Application.SeedWork;
using eCommerceWebAPI.Domain;
using eCommerceWebAPI.Domain.Addresses;
using eCommerceWebAPI.Domain.Catalogs;
using eCommerceWebAPI.Domain.Customers;
using eCommerceWebAPI.Domain.Orders;
using eCommerceWebAPI.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace eCommerceWebAPI.Application.Orders.Commands.AddOrder
{
    public class AddOrderCommandHandler : ICommandHandler<AddOrderCommand, BaseResponse>
    {
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<City> _cityRepository;
        private readonly IShoppingCartRepository _shoppingCartRepository;
        private readonly IRepository<Product> _productRepository;
        public readonly IRabbitMQPublisher<OrderDto> _rabbitMQPublisher;
        public readonly IMapper _mapper;

        public AddOrderCommandHandler(IRepository<Customer> customerRepository,
            IRepository<Order> orderRepository,
            IRepository<City> cityRepository,
            IShoppingCartRepository shoppingCartRepository,
            IRepository<Product> productRepository,
            IRabbitMQPublisher<OrderDto> rabbitMQPublisher,
            IMapper mapper)
        {
            _customerRepository = customerRepository;
            _orderRepository = orderRepository;
            _cityRepository = cityRepository;
            _shoppingCartRepository = shoppingCartRepository;
            _productRepository = productRepository;
            _rabbitMQPublisher = rabbitMQPublisher;
            _mapper = mapper;
        }

        public async Task<BaseResponse> Handle(AddOrderCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(nameof(request));

            var addOrderDto = request.AddOrderDto;
            ArgumentNullException.ThrowIfNull(nameof(addOrderDto));

            var addAddressDto = addOrderDto.AddOrderAddress;
            ArgumentNullException.ThrowIfNull(nameof(addAddressDto));

            BaseResponse response = new();

            // Check customer would be existed
            long customerId = request.CustomerId;
            Customer customer = await _customerRepository.Get(customerId);
            if (customer == null || customer.Deleted)
            {
                response.Errors.Add($"No customer found with ID of : {customerId}");
                response.HttpStatusCode = HttpStatusCode.BadRequest;
                return response;
            }

            // Bad request parameters
            if (addOrderDto.ShippingCostExcludeTax < 0) response.Errors.Add("Shipping cost could not be less than zero!");
            if (addOrderDto.OrderDiscountAmountExcludeTax < 0) response.Errors.Add("Discount amount could not be less than zero!");
            if (string.IsNullOrEmpty(addAddressDto.FirstName?.Trim())) response.Errors.Add("Address first name could not be empty!");
            if (string.IsNullOrEmpty(addAddressDto.LastName?.Trim())) response.Errors.Add("Address last could not be empty!");
            if (string.IsNullOrEmpty(addAddressDto.StreetAddress?.Trim())) response.Errors.Add("Address's street address could not be empty!");
            if (string.IsNullOrEmpty(addAddressDto.PhoneNumber?.Trim())) response.Errors.Add("Address phone number could not be empty!");
            if (response.Errors.Any())
            {
                response.HttpStatusCode = HttpStatusCode.BadRequest;
                return response;
            }

            // Check chosen city would be existed
            var city = await _cityRepository
                .AsQueryable()
                .AsNoTracking()
                .Include(c => c.Province)
                .ThenInclude(p => p.Country)
                .FirstOrDefaultAsync(c => c.Id == addAddressDto.CityId);
            if (city == null)
            {
                response.Errors.Add($"No city found with ID of : {addAddressDto.CityId}");
                response.HttpStatusCode = HttpStatusCode.BadRequest;
                return response;
            }

            // Get shopping cart items
            var shoppingCartItems = await _shoppingCartRepository.GetShoppingCartAsync(customerId);
            if (!shoppingCartItems.Any())
            {
                response.Errors.Add($"You have nothing in your cart!");
                response.HttpStatusCode = HttpStatusCode.BadRequest;
                return response;
            }

            // Check shpping cart items quantities
            var wrongShoppingCartItems = shoppingCartItems.Where(sci => sci.Quantity <= 0).ToList();
            if (wrongShoppingCartItems.Any()) wrongShoppingCartItems.ForEach(wrongShoppingCartItem => response.Errors.Add($"It's not possible to order any product with quantity of Zero or less! please remove the product with ID of : {wrongShoppingCartItem.ProductId} from your cart!"));
            if (response.Errors.Any())
            {
                response.HttpStatusCode = HttpStatusCode.BadRequest;
                return response;
            }

            // Check poroducts info
            var shoppingCartProductIds = shoppingCartItems.Select(sci => sci.ProductId).DistinctBy(pId => pId).ToList();
            var shoppingCartProducts = _productRepository
                .AsQueryable()
                .AsNoTracking()
                .Where(p =>
                    !p.Deleted &&
                    shoppingCartProductIds.Contains(p.Id))
                .ToList();
            var notExistingProductIds = shoppingCartProducts.Select(p => p.Id).Where(pId => !shoppingCartProductIds.Contains(pId)).ToList();
            if (notExistingProductIds.Any()) notExistingProductIds.ForEach(notExistingProductId => response.Errors.Add($"Product with ID of : {notExistingProductId} is not existed! please remove it from your cart!"));
            foreach (var product in shoppingCartProducts)
            {
                var relatedShoppingCartItem = shoppingCartItems.FirstOrDefault(sci => sci.ProductId == product.Id);
                if (product.Name != relatedShoppingCartItem!.ProductName) response.Errors.Add($"Now, the code of product with ID of : {product.Id} is {product.Name} but this code in your shopping cart is {relatedShoppingCartItem!.ProductName}; So your cart should be updated, first.");
                if (product.Code != relatedShoppingCartItem!.ProductCode) response.Errors.Add($"Now, the code of product with ID of : {product.Id} is {product.Code} but this code in your shopping cart is {relatedShoppingCartItem!.ProductCode}; So your cart should be updated, first.");
                if (product.Price != relatedShoppingCartItem!.ProductUnitPriceExcludeTax) response.Errors.Add($"Now, the price of product with ID of : {product.Id} is {product.Price} but this price in your shopping cart is {relatedShoppingCartItem!.ProductUnitPriceExcludeTax}; So your cart should be updated, first.");
                if (product.DiscountAmount != relatedShoppingCartItem!.ProductUnitDiscountAmountExcludeTax) response.Errors.Add($"Now, the discount amount of product with ID of : {product.Id} is {product.DiscountAmount} but this discount amount in your shopping cart is {relatedShoppingCartItem!.ProductUnitDiscountAmountExcludeTax}; So your cart should be updated, first.");
                if (product.Currency != relatedShoppingCartItem!.Currency) response.Errors.Add($"Now, the currency of product with ID of : {product.Id} is {product.Currency} but this currency in your shopping cart is {relatedShoppingCartItem!.Currency}; So your cart should be updated, first.");
            }
            if (response.Errors.Any())
            {
                response.HttpStatusCode = HttpStatusCode.BadRequest;
                return response;
            }

            // Evaluate prices, discounts and etc.
            decimal taxCoefficient = (decimal)1.09;

            // Create order entity
            Order order = new()
            {
                CustomerId = customerId,
                CreatedOnUtc = DateTime.UtcNow,
                ShippingCostExcludeTax = addOrderDto.ShippingCostExcludeTax,
                ShippingCostIncludeTax = addOrderDto.ShippingCostExcludeTax * taxCoefficient,
                OrderDiscountAmountExcludeTax = addOrderDto.OrderDiscountAmountExcludeTax,
                OrderDiscountAmountIncludeTax = addOrderDto.OrderDiscountAmountExcludeTax * taxCoefficient,
                Currency = "IRT",
                Deleted = false
            };
            order.Address = new()
            {
                Title = addAddressDto.Title?.Trim(),
                FirstName = addAddressDto.FirstName!.Trim(),
                LastName = addAddressDto.LastName!.Trim(),
                CountryName = city.Province?.Country?.Name ?? string.Empty,
                ProvinceName = city.Province?.Name ?? string.Empty,
                CityName = city.Name,
                StreetAddress = addAddressDto.StreetAddress!.Trim(),
                PostalCode = addAddressDto.PostalCode?.Trim(),
                PhoneNumber = addAddressDto.PhoneNumber!.Trim()
            };
            shoppingCartItems.ForEach(shoppingCartItem =>
                order.OrderItems.Add(new()
                {
                    ProductId = shoppingCartItem.ProductId,
                    ProductName = shoppingCartItem.ProductName,
                    ProductCode = shoppingCartItem.ProductCode,
                    ProductUnitPriceExcludeTax = shoppingCartItem.ProductUnitPriceExcludeTax,
                    ProductUnitPriceIncludeTax = shoppingCartItem.ProductUnitPriceExcludeTax * taxCoefficient,
                    ProductUnitDiscountAmountExcludeTax = shoppingCartItem.ProductUnitDiscountAmountExcludeTax,
                    ProductUnitDiscountAmountIncludeTax = shoppingCartItem.ProductUnitDiscountAmountExcludeTax * taxCoefficient,
                    Quantity = shoppingCartItem.Quantity
                })
            );

            var insertedOrder = await _orderRepository.Add(order);

            // Check it's inserted or not
            if (insertedOrder.Id <= 0)
            {
                response.HttpStatusCode = HttpStatusCode.InternalServerError;
                response.Errors.Add("An error has been occured while placing an order!");
                return response;
            }

            // Flush customer's shopping cart after order placed
            _shoppingCartRepository.ClearShoppingCartAsync(customerId);

            // Publish message
            var orderDto = _mapper.Map<OrderDto>(order);
            _rabbitMQPublisher.Publish(orderDto);

            // Return the result
            response.HttpStatusCode = HttpStatusCode.Created;
            response.Message = "The order has been placed successfully!";
            return response;
        }
    }
}
