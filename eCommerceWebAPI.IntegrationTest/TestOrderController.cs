using AngleSharp.Common;
using AutoMapper;
using eCommerceWebAPI.Api.SeedWork;
using eCommerceWebAPI.Application.Addresses.Commands.AddCustomerAddress;
using eCommerceWebAPI.Application.Orders;
using eCommerceWebAPI.Application.Orders.Commands.AddOrder;
using eCommerceWebAPI.Domain;
using eCommerceWebAPI.Domain.Catalogs;
using eCommerceWebAPI.Domain.ShoppingCarts;
using eCommerceWebAPI.IntegrationTest.InfraStructure;
using eCommerceWebAPI.IntegrationTest.MockObjects;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Net;
using System.Web;

namespace eCommerceWebAPI.IntegrationTest
{
    [TestFixture]
    public class TestOrderController : BaseControllerTest
    {
        public TestOrderController()
        {
            BaseRoute = "order";
        }

        #region PlaceOrder

        [Test]
        public void TestPlaceOrder()
        {
            var countries = SampleTestEntities.GetSampleCountries().Result;
            var customer = SampleTestEntities.GetSampleCustomer().Result;
            var categories = SampleTestEntities.GetSampleCategories().Result;
            var specifications = SampleTestEntities.GetSampleSpecifications().Result;
            Database.Countries.AddRange(countries);
            Database.Customers.Add(customer);
            Database.Categories.AddRange(categories);
            Database.Specifications.AddRange(specifications);
            Database.SaveChanges();

            var rnd = new Random();
            var booleanList = new List<bool>(2) { false, true };

            List<Category> categoriesToMap;
            do
            {
                categoriesToMap = categories.Where(s => booleanList[rnd.Next(0, 2)]).ToList();
            }
            while (categoriesToMap.Count <= 0);

            List<Specification> specificationsToMap;
            do
            {
                specificationsToMap = specifications.Where(s => booleanList[rnd.Next(0, 2)]).ToList();
            }
            while (specificationsToMap.Count <= 0);

            List<Product> products;
            bool isAnyProductStockQuantityMoreThanTwo = false;
            do
            {
                products = SampleTestEntities.GetSampleProducts(categories: categories, specifications: specificationsToMap).Result;
                var productStockQuantityMoreThanTwo = products.Where(p => p.StockQuantity > 2).ToList();
                isAnyProductStockQuantityMoreThanTwo = productStockQuantityMoreThanTwo.Any();
            }
            while (!isAnyProductStockQuantityMoreThanTwo);
            Database.Products.AddRange(products);
            Database.SaveChanges();

            var shoppingCartItems = SampleTestEntities.GetSampleShoppingCartItems(customer.Id, products).Result;

            var mapper = Factory.Services.GetService<IMapper>();
            var shoppingCartRepository = Factory.Services.GetService<IShoppingCartRepository>();

            var shoppingCartItemRedisList = mapper!.Map<List<ShoppingCartItemRedis>>(shoppingCartItems);
            shoppingCartRepository!.UpdateShoppingCartAsync(customer.Id, shoppingCartItemRedisList).GetAwaiter().GetResult();

            var address = SampleTestEntities.GetSampleAddress().Result;
            var allCities = countries.SelectMany(c => c.Provinces).SelectMany(p => p.Cities).ToList();
            var city = allCities.GetItemByIndex(rnd.Next(0, allCities.Count));
            AddAddressDto addAddressDto = new()
            {
                Title = address.Title,
                FirstName = address.FirstName,
                LastName = address.LastName,
                CityId = city.Id,
                StreetAddress = address.StreetAddress,
                PostalCode = address.PostalCode,
                PhoneNumber = address.PhoneNumber
            };

            AddOrderDto addOrderDto = new()
            {
                ShippingCostExcludeTax = rnd.Next(0, 120) * 1000,
                OrderDiscountAmountExcludeTax = rnd.Next(0, 50) * 1000,
                AddOrderAddress = addAddressDto
            };

            var response = Client.PostAsJsonAsync(GetBaseRoute($"{customer.Id}"), addOrderDto).Result;
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var responseContent = response.Content.Deserialize<Response>().Result;
            responseContent.Should().NotBeNull();
            responseContent.Status.Should().Be("Success");

            // Check order has been created successfully
            var foundOrder = Database.Orders.AsNoTracking().FirstOrDefault(o => o.CustomerId == customer.Id);
            foundOrder.Should().NotBeNull();
            foundOrder.Id.Should().BeGreaterThanOrEqualTo(1);
            foundOrder.AddressId.Should().BeGreaterThanOrEqualTo(1);
            foundOrder.ShippingCostExcludeTax.Should().Be(addOrderDto.ShippingCostExcludeTax);
            foundOrder.ShippingCostIncludeTax.Should().Be(addOrderDto.ShippingCostExcludeTax * (decimal)1.09);
            foundOrder.OrderDiscountAmountExcludeTax.Should().Be(addOrderDto.OrderDiscountAmountExcludeTax);
            foundOrder.OrderDiscountAmountIncludeTax.Should().Be(addOrderDto.OrderDiscountAmountExcludeTax * (decimal)1.09);

            // Check order address has been created correctly
            var foundOrderAddress = Database.Addresses.AsNoTracking().FirstOrDefault(a => a.Id == foundOrder.AddressId);
            foundOrderAddress.Should().NotBeNull();
            foundOrderAddress.Title.Should().Be(addAddressDto.Title);
            foundOrderAddress.FirstName.Should().Be(addAddressDto.FirstName);
            foundOrderAddress.LastName.Should().Be(addAddressDto.LastName);
            foundOrderAddress.CityName.Should().Be(city.Name);
            foundOrderAddress.StreetAddress.Should().Be(addAddressDto.StreetAddress);
            foundOrderAddress.PostalCode.Should().Be(addAddressDto.PostalCode);
            foundOrderAddress.PhoneNumber.Should().Be(addAddressDto.PhoneNumber);

            // Check order items are migrated correctly
            var foundOrderItems = Database.OrderItems.AsNoTracking().Where(oi => oi.OrderId == foundOrder.Id).ToList();
            foundOrderItems.Should().NotBeNullOrEmpty();
            foundOrderItems.Count.Should().Be(shoppingCartItems.Count);
            foreach (var orderItem in foundOrderItems)
            {
                var relatedShoppingCartItem = shoppingCartItems.FirstOrDefault(sci => sci.ProductId == orderItem.ProductId);
                relatedShoppingCartItem.Should().NotBeNull();
                orderItem.ProductName.Should().Be(relatedShoppingCartItem.ProductName);
                orderItem.ProductCode.Should().Be(relatedShoppingCartItem.ProductCode);
                orderItem.ProductUnitPriceExcludeTax.Should().Be(relatedShoppingCartItem.ProductUnitPriceExcludeTax);
                orderItem.ProductUnitPriceIncludeTax.Should().Be(relatedShoppingCartItem.ProductUnitPriceIncludeTax);
                orderItem.ProductUnitDiscountAmountExcludeTax.Should().Be(relatedShoppingCartItem.ProductUnitDiscountAmountExcludeTax);
                orderItem.ProductUnitDiscountAmountIncludeTax.Should().Be(relatedShoppingCartItem.ProductUnitDiscountAmountIncludeTax);
                orderItem.Quantity.Should().Be(relatedShoppingCartItem.Quantity);
            }

            // Cart would be empty now
            var foundShoppingCartItems = shoppingCartRepository!.GetShoppingCartAsync(customer.Id).GetAwaiter().GetResult();
            foundShoppingCartItems.Should().BeNullOrEmpty();

            // Check the products stock quantities would be reduced correctly now
            var existingProductIds = products.Where(p => p.StockQuantity > 0).Select(p => p.Id).ToList();
            var newProducts = Database.Products.AsNoTracking().Where(p => existingProductIds.Contains(p.Id)).ToList();
            newProducts.Should().NotBeNullOrEmpty();
            newProducts.Count.Should().BeGreaterThanOrEqualTo(shoppingCartItems.Count);
            foreach (var newProduct in newProducts)
            {
                var relatedOldProduct = products.FirstOrDefault(p => p.Id == newProduct.Id);
                relatedOldProduct.Should().NotBeNull();

                var relatedShoppingCartItem = shoppingCartItems.FirstOrDefault(sci => sci.ProductId == newProduct.Id);
                if (relatedShoppingCartItem == null)
                {
                    continue;
                }

                newProduct.StockQuantity.Should().Be(relatedOldProduct.StockQuantity - relatedShoppingCartItem!.Quantity);
            }
        }

        #endregion

        #region GetAll

        [Test]
        public void TestGetAll_WithoutQueryParameters()
        {
            var customer = SampleTestEntities.GetSampleCustomer().Result;
            Database.Customers.AddRange(customer);
            Database.SaveChanges();

            var address = SampleTestEntities.GetSampleAddress().Result;

            var rnd = new Random();
            var booleanList = new List<bool>(2) { false, true };

            List<Product> products;
            bool isAnyProductStockQuantityMoreThanTwo = false;
            do
            {
                products = SampleTestEntities.GetSampleProducts().Result;
                var productStockQuantityMoreThanTwo = products.Where(p => p.StockQuantity > 2).ToList();
                isAnyProductStockQuantityMoreThanTwo = productStockQuantityMoreThanTwo.Any();
            }
            while (!isAnyProductStockQuantityMoreThanTwo);
            Database.Products.AddRange(products);
            Database.SaveChanges();

            var order = SampleTestEntities.GetSampleOrder(customer.Id, address, products).Result;
            Database.Orders.Add(order);
            Database.SaveChanges();

            var response = Client.GetAsync(GetBaseRoute($"{customer.Id}")).Result;
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseContent = response.Content.Deserialize<CustomResponseList<OrderDto>>().Result;
            responseContent.Should().NotBeNull();
            responseContent.Status.Should().Be("Success");

            var data = responseContent.Data;
            data.Should().NotBeNull();
            data.Should().HaveCountGreaterThanOrEqualTo(1);

            var dataOrderDto = data.FirstOrDefault(oDto => oDto.CustomerId == customer.Id);
            dataOrderDto.Should().NotBeNull();
            dataOrderDto.ShippingCostExcludeTax.Should().Be(order.ShippingCostExcludeTax);
            dataOrderDto.ShippingCostIncludeTax.Should().Be(order.ShippingCostIncludeTax);
            dataOrderDto.OrderDiscountAmountExcludeTax.Should().Be(order.OrderDiscountAmountExcludeTax);
            dataOrderDto.OrderDiscountAmountIncludeTax.Should().Be(order.OrderDiscountAmountIncludeTax);
            dataOrderDto.AddressId.Should().Be(address.Id);
            dataOrderDto.Currency.Should().Be(order.Currency);
        }

        [Test]
        public void TestGetAll_WithQueryParameters()
        {
            var customer = SampleTestEntities.GetSampleCustomer().Result;
            Database.Customers.AddRange(customer);
            Database.SaveChanges();

            var address = SampleTestEntities.GetSampleAddress().Result;

            var rnd = new Random();
            var booleanList = new List<bool>(2) { false, true };

            List<Product> products;
            bool isAnyProductStockQuantityMoreThanTwo = false;
            do
            {
                products = SampleTestEntities.GetSampleProducts().Result;
                var productStockQuantityMoreThanTwo = products.Where(p => p.StockQuantity > 2).ToList();
                isAnyProductStockQuantityMoreThanTwo = productStockQuantityMoreThanTwo.Any();
            }
            while (!isAnyProductStockQuantityMoreThanTwo);
            Database.Products.AddRange(products);
            Database.SaveChanges();

            var order = SampleTestEntities.GetSampleOrder(customer.Id, address, products).Result;
            Database.Orders.Add(order);
            Database.SaveChanges();

            var queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString.Add("pageNumber", $"{1}");
            queryString.Add("pageSize", $"{1}");

            var response = Client.GetAsync(GetBaseRoute($"{customer.Id}") + "?" + queryString).Result;
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseContent = response.Content.Deserialize<CustomResponseList<OrderDto>>().Result;
            responseContent.Should().NotBeNull();
            responseContent.Status.Should().Be("Success");

            var data = responseContent.Data;
            data.Should().NotBeNull();
            data.Should().HaveCountGreaterThanOrEqualTo(1);

            var dataOrderDto = data.FirstOrDefault(oDto => oDto.CustomerId == customer.Id);
            dataOrderDto.Should().NotBeNull();
            dataOrderDto.ShippingCostExcludeTax.Should().Be(order.ShippingCostExcludeTax);
            dataOrderDto.ShippingCostIncludeTax.Should().Be(order.ShippingCostIncludeTax);
            dataOrderDto.OrderDiscountAmountExcludeTax.Should().Be(order.OrderDiscountAmountExcludeTax);
            dataOrderDto.OrderDiscountAmountIncludeTax.Should().Be(order.OrderDiscountAmountIncludeTax);
            dataOrderDto.AddressId.Should().Be(address.Id);
            dataOrderDto.Currency.Should().Be(order.Currency);
        }

        #endregion
    }
}