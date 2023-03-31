using AngleSharp.Common;
using AutoMapper;
using eCommerceWebAPI.Api.SeedWork;
using eCommerceWebAPI.Application.ShoppingCarts;
using eCommerceWebAPI.Application.ShoppingCarts.Commands.DeleteShoppingCartItem;
using eCommerceWebAPI.Application.ShoppingCarts.Commands.UpdateShoppingCartItem;
using eCommerceWebAPI.Domain;
using eCommerceWebAPI.Domain.Catalogs;
using eCommerceWebAPI.Domain.ShoppingCarts;
using eCommerceWebAPI.IntegrationTest.InfraStructure;
using eCommerceWebAPI.IntegrationTest.MockObjects;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Net;

namespace eCommerceWebAPI.IntegrationTest
{
    [TestFixture]
    public class TestShoppingCartController : BaseControllerTest
    {
        public TestShoppingCartController()
        {
            BaseRoute = "cart";
        }

        #region UpdateShoppingCartItem

        [Test]
        public void TestUpdateShoppingCartItem_AddNewItem()
        {
            var customer = SampleTestEntities.GetSampleCustomer().Result;
            var categories = SampleTestEntities.GetSampleCategories().Result;
            var specifications = SampleTestEntities.GetSampleSpecifications().Result;
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

            var shoppingCartItem = SampleTestEntities.GetSampleShoppingCartItem(customer.Id, products).Result;

            UpdateShoppingCartItemDto updateShoppingCartItemDto = new()
            {
                ProductId = shoppingCartItem.ProductId,
                Quantity = shoppingCartItem.Quantity
            };

            var response = Client.PostAsJsonAsync(GetBaseRoute($"{customer.Id}"), updateShoppingCartItemDto).Result;
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseContent = response.Content.Deserialize<Response>().Result;
            responseContent.Should().NotBeNull();
            responseContent.Status.Should().Be("Success");

            var shoppingCartRepository = Factory.Services.GetService<IShoppingCartRepository>();

            var foundShoppingCartItems = shoppingCartRepository!.GetShoppingCartAsync(customer.Id).GetAwaiter().GetResult();
            foundShoppingCartItems.Should().NotBeNull();
            foundShoppingCartItems.Should().HaveCount(1);

            var foundShoppingCartItem = foundShoppingCartItems.FirstOrDefault();
            foundShoppingCartItem.Should().NotBeNull();
            foundShoppingCartItem.ProductId.Should().Be(shoppingCartItem.ProductId);
            foundShoppingCartItem.ProductName.Should().Be(shoppingCartItem.ProductName);
            foundShoppingCartItem.ProductCode.Should().Be(shoppingCartItem.ProductCode);
            foundShoppingCartItem.ProductUnitPriceExcludeTax.Should().Be(shoppingCartItem.ProductUnitPriceExcludeTax);
            foundShoppingCartItem.ProductUnitPriceIncludeTax.Should().Be(shoppingCartItem.ProductUnitPriceIncludeTax);
            foundShoppingCartItem.ProductUnitDiscountAmountExcludeTax.Should().Be(shoppingCartItem.ProductUnitDiscountAmountExcludeTax);
            foundShoppingCartItem.ProductUnitDiscountAmountIncludeTax.Should().Be(shoppingCartItem.ProductUnitDiscountAmountIncludeTax);
            foundShoppingCartItem.Quantity.Should().Be(shoppingCartItem.Quantity);
            foundShoppingCartItem.Currency.Should().Be(shoppingCartItem.Currency);
        }

        [Test]
        public void TestUpdateShoppingCartItem_UpdateItem_IncreaseQuantity()
        {
            var customer = SampleTestEntities.GetSampleCustomer().Result;
            var categories = SampleTestEntities.GetSampleCategories().Result;
            var specifications = SampleTestEntities.GetSampleSpecifications().Result;
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

            var updatingShoppingCartItem = shoppingCartItems.FirstOrDefault(sci => sci.Quantity > 2);
            updatingShoppingCartItem.Should().NotBeNull();

            var relatedProduct = products.FirstOrDefault(p => p.Id == updatingShoppingCartItem!.ProductId);
            relatedProduct.Should().NotBeNull();
            if (relatedProduct!.StockQuantity <= updatingShoppingCartItem!.Quantity)
            {
                relatedProduct.StockQuantity = updatingShoppingCartItem.Quantity + 1;
                Database.Products.Update(relatedProduct);
            }

            UpdateShoppingCartItemDto updateShoppingCartItemDto = new()
            {
                ProductId = updatingShoppingCartItem.ProductId,
                Quantity = 1
            };

            var response = Client.PostAsJsonAsync(GetBaseRoute($"{customer.Id}"), updateShoppingCartItemDto).Result;
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseContent = response.Content.Deserialize<Response>().Result;
            responseContent.Should().NotBeNull();
            responseContent.Status.Should().Be("Success");

            var foundShoppingCartItems = shoppingCartRepository!.GetShoppingCartAsync(customer.Id).GetAwaiter().GetResult();
            foundShoppingCartItems.Should().NotBeNull();
            foundShoppingCartItems.Should().HaveCount(shoppingCartItems.Count);

            var foundShoppingCartItem = foundShoppingCartItems.FirstOrDefault(sci => sci.ProductId == relatedProduct.Id);
            foundShoppingCartItem.Should().NotBeNull();
            foundShoppingCartItem.ProductId.Should().Be(updatingShoppingCartItem.ProductId);
            foundShoppingCartItem.ProductName.Should().Be(updatingShoppingCartItem.ProductName);
            foundShoppingCartItem.ProductCode.Should().Be(updatingShoppingCartItem.ProductCode);
            foundShoppingCartItem.ProductUnitPriceExcludeTax.Should().Be(updatingShoppingCartItem.ProductUnitPriceExcludeTax);
            foundShoppingCartItem.ProductUnitPriceIncludeTax.Should().Be(updatingShoppingCartItem.ProductUnitPriceIncludeTax);
            foundShoppingCartItem.ProductUnitDiscountAmountExcludeTax.Should().Be(updatingShoppingCartItem.ProductUnitDiscountAmountExcludeTax);
            foundShoppingCartItem.ProductUnitDiscountAmountIncludeTax.Should().Be(updatingShoppingCartItem.ProductUnitDiscountAmountIncludeTax);
            foundShoppingCartItem.Quantity.Should().Be(updatingShoppingCartItem.Quantity + 1);
            foundShoppingCartItem.Currency.Should().Be(updatingShoppingCartItem.Currency);
        }

        [Test]
        public void TestUpdateShoppingCartItem_UpdateItem_DecreaseQuantity()
        {
            var customer = SampleTestEntities.GetSampleCustomer().Result;
            var categories = SampleTestEntities.GetSampleCategories().Result;
            var specifications = SampleTestEntities.GetSampleSpecifications().Result;
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

            var updatingShoppingCartItem = shoppingCartItems.FirstOrDefault(sci => sci.Quantity > 2);
            updatingShoppingCartItem.Should().NotBeNull();

            var relatedProduct = products.FirstOrDefault(p => p.Id == updatingShoppingCartItem!.ProductId);
            relatedProduct.Should().NotBeNull();

            UpdateShoppingCartItemDto updateShoppingCartItemDto = new()
            {
                ProductId = updatingShoppingCartItem.ProductId,
                Quantity = -1
            };

            var response = Client.PostAsJsonAsync(GetBaseRoute($"{customer.Id}"), updateShoppingCartItemDto).Result;
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseContent = response.Content.Deserialize<Response>().Result;
            responseContent.Should().NotBeNull();
            responseContent.Status.Should().Be("Success");

            var foundShoppingCartItems = shoppingCartRepository!.GetShoppingCartAsync(customer.Id).GetAwaiter().GetResult();
            foundShoppingCartItems.Should().NotBeNull();
            foundShoppingCartItems.Should().HaveCount(shoppingCartItems.Count);

            var foundShoppingCartItem = foundShoppingCartItems.FirstOrDefault(sci => sci.ProductId == relatedProduct.Id);
            foundShoppingCartItem.Should().NotBeNull();
            foundShoppingCartItem.ProductId.Should().Be(updatingShoppingCartItem.ProductId);
            foundShoppingCartItem.ProductName.Should().Be(updatingShoppingCartItem.ProductName);
            foundShoppingCartItem.ProductCode.Should().Be(updatingShoppingCartItem.ProductCode);
            foundShoppingCartItem.ProductUnitPriceExcludeTax.Should().Be(updatingShoppingCartItem.ProductUnitPriceExcludeTax);
            foundShoppingCartItem.ProductUnitPriceIncludeTax.Should().Be(updatingShoppingCartItem.ProductUnitPriceIncludeTax);
            foundShoppingCartItem.ProductUnitDiscountAmountExcludeTax.Should().Be(updatingShoppingCartItem.ProductUnitDiscountAmountExcludeTax);
            foundShoppingCartItem.ProductUnitDiscountAmountIncludeTax.Should().Be(updatingShoppingCartItem.ProductUnitDiscountAmountIncludeTax);
            foundShoppingCartItem.Quantity.Should().Be(updatingShoppingCartItem.Quantity - 1);
            foundShoppingCartItem.Currency.Should().Be(updatingShoppingCartItem.Currency);
        }

        [Test]
        public void TestUpdateShoppingCartItem_UpdateItem_IncreaseQuantity_MoreThanExisted()
        {
            var customer = SampleTestEntities.GetSampleCustomer().Result;
            var categories = SampleTestEntities.GetSampleCategories().Result;
            var specifications = SampleTestEntities.GetSampleSpecifications().Result;
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

            var updatingShoppingCartItem = shoppingCartItems.FirstOrDefault(sci => sci.Quantity > 2);
            updatingShoppingCartItem.Should().NotBeNull();

            var relatedProduct = products.FirstOrDefault(p => p.Id == updatingShoppingCartItem!.ProductId);
            relatedProduct.Should().NotBeNull();

            UpdateShoppingCartItemDto updateShoppingCartItemDto = new()
            {
                ProductId = updatingShoppingCartItem.ProductId,
                Quantity = 1000
            };

            var response = Client.PostAsJsonAsync(GetBaseRoute($"{customer.Id}"), updateShoppingCartItemDto).Result;
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var responseContent = response.Content.Deserialize<Response>().Result;
            responseContent.Should().NotBeNull();
            responseContent.Status.Should().Be("Failed");

            var foundShoppingCartItems = shoppingCartRepository!.GetShoppingCartAsync(customer.Id).GetAwaiter().GetResult();
            foundShoppingCartItems.Should().NotBeNull();
            foundShoppingCartItems.Should().HaveCount(shoppingCartItems.Count);

            var foundShoppingCartItem = foundShoppingCartItems.FirstOrDefault(sci => sci.ProductId == relatedProduct.Id);
            foundShoppingCartItem.Should().NotBeNull();
            foundShoppingCartItem.ProductId.Should().Be(updatingShoppingCartItem.ProductId);
            foundShoppingCartItem.ProductName.Should().Be(updatingShoppingCartItem.ProductName);
            foundShoppingCartItem.ProductCode.Should().Be(updatingShoppingCartItem.ProductCode);
            foundShoppingCartItem.ProductUnitPriceExcludeTax.Should().Be(updatingShoppingCartItem.ProductUnitPriceExcludeTax);
            foundShoppingCartItem.ProductUnitPriceIncludeTax.Should().Be(updatingShoppingCartItem.ProductUnitPriceIncludeTax);
            foundShoppingCartItem.ProductUnitDiscountAmountExcludeTax.Should().Be(updatingShoppingCartItem.ProductUnitDiscountAmountExcludeTax);
            foundShoppingCartItem.ProductUnitDiscountAmountIncludeTax.Should().Be(updatingShoppingCartItem.ProductUnitDiscountAmountIncludeTax);
            foundShoppingCartItem.Quantity.Should().Be(updatingShoppingCartItem.Quantity);
            foundShoppingCartItem.Currency.Should().Be(updatingShoppingCartItem.Currency);
        }

        [Test]
        public void TestUpdateShoppingCartItem_RemoveItem()
        {
            var customer = SampleTestEntities.GetSampleCustomer().Result;
            var categories = SampleTestEntities.GetSampleCategories().Result;
            var specifications = SampleTestEntities.GetSampleSpecifications().Result;
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

            var updatingShoppingCartItem = shoppingCartItems.FirstOrDefault(sci => sci.Quantity > 2);
            updatingShoppingCartItem.Should().NotBeNull();

            var relatedProduct = products.FirstOrDefault(p => p.Id == updatingShoppingCartItem!.ProductId);
            relatedProduct.Should().NotBeNull();

            UpdateShoppingCartItemDto updateShoppingCartItemDto = new()
            {
                ProductId = updatingShoppingCartItem.ProductId,
                Quantity = -updatingShoppingCartItem.Quantity
            };

            var response = Client.PostAsJsonAsync(GetBaseRoute($"{customer.Id}"), updateShoppingCartItemDto).Result;
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseContent = response.Content.Deserialize<Response>().Result;
            responseContent.Should().NotBeNull();
            responseContent.Status.Should().Be("Success");

            var foundShoppingCartItems = shoppingCartRepository!.GetShoppingCartAsync(customer.Id).GetAwaiter().GetResult();
            foundShoppingCartItems.Should().NotBeNull();
            foundShoppingCartItems.Should().HaveCount(shoppingCartItems.Count - 1);

            var foundShoppingCartItem = foundShoppingCartItems.FirstOrDefault(sci => sci.ProductId == relatedProduct.Id);
            foundShoppingCartItem.Should().BeNull();
        }

        #endregion

        #region DeleteShoppingCartItem

        [Test]
        public void TestDeleteShoppingCartItem()
        {
            var customer = SampleTestEntities.GetSampleCustomer().Result;
            var categories = SampleTestEntities.GetSampleCategories().Result;
            var specifications = SampleTestEntities.GetSampleSpecifications().Result;
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

            var deletingShoppingCartItem = shoppingCartItems.FirstOrDefault(sci => sci.Quantity > 2);
            deletingShoppingCartItem.Should().NotBeNull();

            var relatedProduct = products.FirstOrDefault(p => p.Id == deletingShoppingCartItem!.ProductId);
            relatedProduct.Should().NotBeNull();

            DeleteShoppingCartItemDto deleteShoppingCartItemDto = new()
            {
                ProductId = deletingShoppingCartItem.ProductId
            };

            var response = Client.DeleteAsJsonAsync(GetBaseRoute($"{customer.Id}"), deleteShoppingCartItemDto).Result;
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseContent = response.Content.Deserialize<Response>().Result;
            responseContent.Should().NotBeNull();
            responseContent.Status.Should().Be("Success");

            var foundShoppingCartItems = shoppingCartRepository!.GetShoppingCartAsync(customer.Id).GetAwaiter().GetResult();
            foundShoppingCartItems.Should().NotBeNull();
            foundShoppingCartItems.Should().HaveCount(shoppingCartItems.Count - 1);

            var foundShoppingCartItem = foundShoppingCartItems.FirstOrDefault(sci => sci.ProductId == relatedProduct.Id);
            foundShoppingCartItem.Should().BeNull();
        }

        #endregion

        #region GetAll

        [Test]
        public void TestGetAll_WithData()
        {
            var customer = SampleTestEntities.GetSampleCustomer().Result;
            var categories = SampleTestEntities.GetSampleCategories().Result;
            var specifications = SampleTestEntities.GetSampleSpecifications().Result;
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

            var response = Client.GetAsync(GetBaseRoute($"{customer.Id}")).Result;
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseContent = response.Content.Deserialize<CustomResponseList<ShoppingCartItemDto>>().Result;
            responseContent.Should().NotBeNull();
            responseContent.Status.Should().Be("Success");

            var data = responseContent.Data;
            data.Should().NotBeNull();
            data.Should().HaveCountGreaterThanOrEqualTo(shoppingCartItems.Count);

            var chosenShoppingCartItem = shoppingCartItems.GetItemByIndex(rnd.Next(0, shoppingCartItems.Count));
            var dataChosenShoppingCartItemsDto = data.FirstOrDefault(sciDto => sciDto.ProductId == chosenShoppingCartItem.ProductId);
            dataChosenShoppingCartItemsDto.Should().NotBeNull();
            dataChosenShoppingCartItemsDto.ProductName.Should().Be(chosenShoppingCartItem.ProductName);
            dataChosenShoppingCartItemsDto.ProductCode.Should().Be(chosenShoppingCartItem.ProductCode);
            dataChosenShoppingCartItemsDto.ProductUnitPriceExcludeTax.Should().Be(chosenShoppingCartItem.ProductUnitPriceExcludeTax);
            dataChosenShoppingCartItemsDto.ProductUnitPriceIncludeTax.Should().Be(chosenShoppingCartItem.ProductUnitPriceIncludeTax);
            dataChosenShoppingCartItemsDto.ProductUnitDiscountAmountExcludeTax.Should().Be(chosenShoppingCartItem.ProductUnitDiscountAmountExcludeTax);
            dataChosenShoppingCartItemsDto.ProductUnitDiscountAmountIncludeTax.Should().Be(chosenShoppingCartItem.ProductUnitDiscountAmountIncludeTax);
            dataChosenShoppingCartItemsDto.Quantity.Should().Be(chosenShoppingCartItem.Quantity);
            dataChosenShoppingCartItemsDto.Currency.Should().Be(chosenShoppingCartItem.Currency);
        }

        [Test]
        public void TestGetAll_WithoutData()
        {
            var customer = SampleTestEntities.GetSampleCustomer().Result;
            var categories = SampleTestEntities.GetSampleCategories().Result;
            var specifications = SampleTestEntities.GetSampleSpecifications().Result;
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

            var response = Client.GetAsync(GetBaseRoute($"{customer.Id}")).Result;
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseContent = response.Content.Deserialize<CustomResponseList<ShoppingCartItemDto>>().Result;
            responseContent.Should().NotBeNull();
            responseContent.Status.Should().Be("Success");

            var data = responseContent.Data;
            data.Should().NotBeNull();
            data.Should().BeEmpty();
        }

        #endregion
    }
}