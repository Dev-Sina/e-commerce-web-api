using AngleSharp.Common;
using eCommerceWebAPI.Api.SeedWork;
using eCommerceWebAPI.Application.Invoices;
using eCommerceWebAPI.Application.Invoices.Commands.AddManualInvoice;
using eCommerceWebAPI.Domain.Catalogs;
using eCommerceWebAPI.IntegrationTest.InfraStructure;
using eCommerceWebAPI.IntegrationTest.MockObjects;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Net;
using System.Web;

namespace eCommerceWebAPI.IntegrationTest
{
    [TestFixture]
    public class TestInvoiceController : BaseControllerTest
    {
        public TestInvoiceController()
        {
            BaseRoute = "invoice";
        }

        #region CreateInvoiceAutomatically

        [Test]
        public void TestCreateInvoiceAutomatically()
        {
            var customer = SampleTestEntities.GetSampleCustomer().Result;
            Database.Customers.Add(customer);
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

            var invoice = SampleTestEntities.GetSampleInvoice(order).Result;

            var response = Client.PostAsync(GetBaseRoute($"automatically/{order.Id}"), null).Result;
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var responseContent = response.Content.Deserialize<Response>().Result;
            responseContent.Should().NotBeNull();
            responseContent.Status.Should().Be("Success");

            // Check invoice has been created successfully
            var foundInvoice = Database.Invoices.AsNoTracking().FirstOrDefault(i => i.OrderId == order.Id);
            foundInvoice.Should().NotBeNull();
            foundInvoice.Id.Should().BeGreaterThanOrEqualTo(1);
            foundInvoice.OrderShippingCostExcludeTax.Should().Be(order.ShippingCostExcludeTax);
            foundInvoice.OrderShippingCostIncludeTax.Should().Be(order.ShippingCostIncludeTax);
            foundInvoice.OrderDiscountAmountExcludeTax.Should().Be(order.OrderDiscountAmountExcludeTax);
            foundInvoice.OrderDiscountAmountIncludeTax.Should().Be(order.OrderDiscountAmountIncludeTax);
            foundInvoice.Currency.Should().Be(order.Currency);

            // Check invoice address has been created correctly
            var foundInvoiceAddress = Database.InvoiceAddresses.AsNoTracking().FirstOrDefault(ia => ia.InvoiceId == foundInvoice.Id);
            foundInvoiceAddress.Should().NotBeNull();
            foundInvoiceAddress.Id.Should().BeGreaterThanOrEqualTo(1);
            foundInvoiceAddress.Title.Should().Be(address.Title);
            foundInvoiceAddress.FirstName.Should().Be(address.FirstName);
            foundInvoiceAddress.LastName.Should().Be(address.LastName);
            foundInvoiceAddress.CountryName.Should().Be(address.CountryName);
            foundInvoiceAddress.ProvinceName.Should().Be(address.ProvinceName);
            foundInvoiceAddress.CityName.Should().Be(address.CityName);
            foundInvoiceAddress.StreetAddress.Should().Be(address.StreetAddress);
            foundInvoiceAddress.PostalCode.Should().Be(address.PostalCode);
            foundInvoiceAddress.PhoneNumber.Should().Be(address.PhoneNumber);

            // Check invoice items are migrated correctly
            var orderItems = order.OrderItems.ToList();
            var foundInvoiceItems = Database.InvoiceItems.AsNoTracking().Where(ii => ii.InvoiceId == foundInvoice.Id).ToList();
            foundInvoiceItems.Should().NotBeNullOrEmpty();
            foundInvoiceItems.Count.Should().Be(orderItems.Count);
            foreach (var invoiceItem in foundInvoiceItems)
            {
                var relatedOrderItem = orderItems.FirstOrDefault(oi => oi.ProductId == invoiceItem.ProductId);
                relatedOrderItem.Should().NotBeNull();
                invoiceItem.ProductName.Should().Be(relatedOrderItem.ProductName);
                invoiceItem.ProductCode.Should().Be(relatedOrderItem.ProductCode);
                invoiceItem.ProductUnitPriceExcludeTax.Should().Be(relatedOrderItem.ProductUnitPriceExcludeTax);
                invoiceItem.ProductUnitPriceIncludeTax.Should().Be(relatedOrderItem.ProductUnitPriceIncludeTax);
                invoiceItem.ProductUnitDiscountAmountExcludeTax.Should().Be(relatedOrderItem.ProductUnitDiscountAmountExcludeTax);
                invoiceItem.ProductUnitDiscountAmountIncludeTax.Should().Be(relatedOrderItem.ProductUnitDiscountAmountIncludeTax);
                invoiceItem.Quantity.Should().Be(relatedOrderItem.Quantity);
                invoiceItem.InvoiceId.Should().Be(foundInvoice.Id);
            }
        }

        #endregion

        #region CreateInvoiceManually

        [Test]
        public void TestCreateInvoiceManually()
        {
            var customer = SampleTestEntities.GetSampleCustomer().Result;
            Database.Customers.Add(customer);
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

            var invoice = SampleTestEntities.GetSampleInvoice(order).Result;

            List<AddManualInvoiceItemDto> invoiceItems = new();
            var orderItems = order.OrderItems.ToList();
            orderItems.Should().NotBeNullOrEmpty();
            foreach (var orderItem in orderItems)
            {
                invoiceItems.Add(new()
                {
                    ProductId = orderItem.ProductId,
                    ProductName = orderItem.ProductName,
                    ProductCode = orderItem.ProductCode,
                    ProductUnitPriceExcludeTax = orderItem.ProductUnitPriceExcludeTax,
                    ProductUnitDiscountAmountExcludeTax = orderItem.ProductUnitDiscountAmountExcludeTax,
                    Quantity = orderItem.Quantity
                });
            }

            AddManualInvoiceAddressDto addAddressDto = new()
            {
                Title = address.Title,
                FirstName = address.FirstName,
                LastName = address.LastName,
                CountryName = address.CountryName,
                ProvinceName = address.ProvinceName,
                CityName = address.CityName,
                StreetAddress = address.StreetAddress,
                PostalCode = address.PostalCode,
                PhoneNumber = address.PhoneNumber
            };

            AddManualInvoiceDto addManualInvoiceDto = new()
            {
                OrderShippingCostExcludeTax = rnd.Next(0, 120) * 1000,
                OrderDiscountAmountExcludeTax = rnd.Next(0, 50) * 1000,
                InvoiceAddress = addAddressDto,
                InvoiceItems = invoiceItems
            };

            var response = Client.PostAsJsonAsync(GetBaseRoute($"manually/{order.Id}"), addManualInvoiceDto).Result;
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var responseContent = response.Content.Deserialize<Response>().Result;
            responseContent.Should().NotBeNull();
            responseContent.Status.Should().Be("Success");

            // Check invoice has been created successfully
            var foundInvoice = Database.Invoices.AsNoTracking().FirstOrDefault(i => i.OrderId == order.Id);
            foundInvoice.Should().NotBeNull();
            foundInvoice.Id.Should().BeGreaterThanOrEqualTo(1);
            foundInvoice.OrderShippingCostExcludeTax.Should().Be(addManualInvoiceDto.OrderShippingCostExcludeTax);
            foundInvoice.OrderShippingCostIncludeTax.Should().Be(addManualInvoiceDto.OrderShippingCostExcludeTax * (decimal)1.09);
            foundInvoice.OrderDiscountAmountExcludeTax.Should().Be(addManualInvoiceDto.OrderDiscountAmountExcludeTax);
            foundInvoice.OrderDiscountAmountIncludeTax.Should().Be(addManualInvoiceDto.OrderDiscountAmountExcludeTax * (decimal)1.09);
            foundInvoice.Currency.Should().Be(order.Currency);

            // Check invoice address has been created correctly
            var foundInvoiceAddress = Database.InvoiceAddresses.AsNoTracking().FirstOrDefault(ia => ia.InvoiceId == foundInvoice.Id);
            foundInvoiceAddress.Should().NotBeNull();
            foundInvoiceAddress.Id.Should().BeGreaterThanOrEqualTo(1);
            foundInvoiceAddress.Title.Should().Be(addAddressDto.Title);
            foundInvoiceAddress.FirstName.Should().Be(addAddressDto.FirstName);
            foundInvoiceAddress.LastName.Should().Be(addAddressDto.LastName);
            foundInvoiceAddress.CountryName.Should().Be(addAddressDto.CountryName);
            foundInvoiceAddress.ProvinceName.Should().Be(addAddressDto.ProvinceName);
            foundInvoiceAddress.CityName.Should().Be(addAddressDto.CityName);
            foundInvoiceAddress.StreetAddress.Should().Be(addAddressDto.StreetAddress);
            foundInvoiceAddress.PostalCode.Should().Be(addAddressDto.PostalCode);
            foundInvoiceAddress.PhoneNumber.Should().Be(addAddressDto.PhoneNumber);

            // Check invoice items are migrated correctly
            var foundInvoiceItems = Database.InvoiceItems.AsNoTracking().Where(ii => ii.InvoiceId == foundInvoice.Id).ToList();
            foundInvoiceItems.Should().NotBeNullOrEmpty();
            foundInvoiceItems.Count.Should().Be(orderItems.Count);
            foreach (var invoiceItem in foundInvoiceItems)
            {
                var relatedOrderItem = orderItems.FirstOrDefault(oi => oi.ProductId == invoiceItem.ProductId);
                relatedOrderItem.Should().NotBeNull();
                invoiceItem.ProductName.Should().Be(relatedOrderItem.ProductName);
                invoiceItem.ProductCode.Should().Be(relatedOrderItem.ProductCode);
                invoiceItem.ProductUnitPriceExcludeTax.Should().Be(relatedOrderItem.ProductUnitPriceExcludeTax);
                invoiceItem.ProductUnitPriceIncludeTax.Should().Be(relatedOrderItem.ProductUnitPriceIncludeTax);
                invoiceItem.ProductUnitDiscountAmountExcludeTax.Should().Be(relatedOrderItem.ProductUnitDiscountAmountExcludeTax);
                invoiceItem.ProductUnitDiscountAmountIncludeTax.Should().Be(relatedOrderItem.ProductUnitDiscountAmountIncludeTax);
                invoiceItem.Quantity.Should().Be(relatedOrderItem.Quantity);
                invoiceItem.InvoiceId.Should().Be(foundInvoice.Id);
            }
        }

        #endregion

        #region GetAllInvoices

        [Test]
        public void TestGetAllInvoices_WithoutQueryParameters()
        {
            var customers = SampleTestEntities.GetSampleCustomers().Result;
            Database.Customers.AddRange(customers);
            Database.SaveChanges();

            var addresses = SampleTestEntities.GetSampleAddresses().Result;

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

            var orders = SampleTestEntities.GetSampleOrders(customers.Select(c => c.Id).ToList(), addresses, products).Result;
            Database.Orders.AddRange(orders);
            Database.SaveChanges();

            var invoices = SampleTestEntities.GetSampleInvoices(orders).Result;
            Database.Invoices.AddRange(invoices);
            Database.SaveChanges();

            var response = Client.GetAsync(GetBaseRoute($"all")).Result;
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseContent = response.Content.Deserialize<CustomResponseList<InvoiceDto>>().Result;
            responseContent.Should().NotBeNull();
            responseContent.Status.Should().Be("Success");

            var data = responseContent.Data;
            data.Should().NotBeNull();
            data.Count.Should().Be(invoices.Count);

            var chosenOrder = orders.GetItemByIndex(rnd.Next(0, orders.Count));

            var dataInvoiceDto = data.FirstOrDefault(iDto => iDto.OrderId == chosenOrder.Id);
            dataInvoiceDto.Should().NotBeNull();
            dataInvoiceDto.OrderShippingCostExcludeTax.Should().Be(chosenOrder.ShippingCostExcludeTax);
            dataInvoiceDto.OrderShippingCostIncludeTax.Should().Be(chosenOrder.ShippingCostIncludeTax);
            dataInvoiceDto.OrderDiscountAmountExcludeTax.Should().Be(chosenOrder.OrderDiscountAmountExcludeTax);
            dataInvoiceDto.OrderDiscountAmountIncludeTax.Should().Be(chosenOrder.OrderDiscountAmountIncludeTax);
            dataInvoiceDto.InvoiceAddress.FirstName.Should().Be(chosenOrder.Address.FirstName);
            dataInvoiceDto.Currency.Should().Be(chosenOrder.Currency);
            dataInvoiceDto.InvoiceItems.Count.Should().Be(chosenOrder.OrderItems.Count);
        }

        [Test]
        public void TestGetAllInvoices_WithQueryParameters()
        {
            var customers = SampleTestEntities.GetSampleCustomers(7).Result;
            Database.Customers.AddRange(customers);
            Database.SaveChanges();

            var addresses = SampleTestEntities.GetSampleAddresses(7).Result;

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

            var orders = SampleTestEntities.GetSampleOrders(customers.Select(c => c.Id).ToList(), addresses, products).Result;
            Database.Orders.AddRange(orders);
            Database.SaveChanges();

            var invoices = SampleTestEntities.GetSampleInvoices(orders).Result;
            Database.Invoices.AddRange(invoices);
            Database.SaveChanges();

            var queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString.Add("pageNumber", $"{2}");
            queryString.Add("pageSize", $"{4}");

            var response = Client.GetAsync(GetBaseRoute($"all") + "?" + queryString).Result;
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseContent = response.Content.Deserialize<CustomResponseList<InvoiceDto>>().Result;
            responseContent.Should().NotBeNull();
            responseContent.Status.Should().Be("Success");

            var data = responseContent.Data;
            data.Should().NotBeNull();
            data.Count.Should().Be(3);

            var dataInvoiceDto = data.FirstOrDefault();
            var chosenOrder = orders.FirstOrDefault(o => o.Id == dataInvoiceDto.OrderId);
            chosenOrder.Should().NotBeNull();
            dataInvoiceDto.OrderShippingCostExcludeTax.Should().Be(chosenOrder.ShippingCostExcludeTax);
            dataInvoiceDto.OrderShippingCostIncludeTax.Should().Be(chosenOrder.ShippingCostIncludeTax);
            dataInvoiceDto.OrderDiscountAmountExcludeTax.Should().Be(chosenOrder.OrderDiscountAmountExcludeTax);
            dataInvoiceDto.OrderDiscountAmountIncludeTax.Should().Be(chosenOrder.OrderDiscountAmountIncludeTax);
            dataInvoiceDto.InvoiceAddress.FirstName.Should().Be(chosenOrder.Address.FirstName);
            dataInvoiceDto.Currency.Should().Be(chosenOrder.Currency);
            dataInvoiceDto.InvoiceItems.Count.Should().Be(chosenOrder.OrderItems.Count);
        }

        [Test]
        public void TestGetAllInvoices_WithoutQueryParameters_NoInvoices()
        {
            var customers = SampleTestEntities.GetSampleCustomers().Result;
            Database.Customers.AddRange(customers);
            Database.SaveChanges();

            var addresses = SampleTestEntities.GetSampleAddresses().Result;

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

            var orders = SampleTestEntities.GetSampleOrders(customers.Select(c => c.Id).ToList(), addresses, products).Result;
            Database.Orders.AddRange(orders);
            Database.SaveChanges();

            var invoices = SampleTestEntities.GetSampleInvoices(orders).Result;

            var response = Client.GetAsync(GetBaseRoute($"all")).Result;
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseContent = response.Content.Deserialize<CustomResponseList<InvoiceDto>>().Result;
            responseContent.Should().NotBeNull();
            responseContent.Status.Should().Be("Failed");

            var data = responseContent.Data;
            data.Should().BeNullOrEmpty();
        }

        [Test]
        public void TestGetAllInvoices_WithQueryParameters_NoInvoices()
        {
            var customers = SampleTestEntities.GetSampleCustomers(7).Result;
            Database.Customers.AddRange(customers);
            Database.SaveChanges();

            var addresses = SampleTestEntities.GetSampleAddresses(7).Result;

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

            var orders = SampleTestEntities.GetSampleOrders(customers.Select(c => c.Id).ToList(), addresses, products).Result;
            Database.Orders.AddRange(orders);
            Database.SaveChanges();

            var invoices = SampleTestEntities.GetSampleInvoices(orders).Result;

            var queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString.Add("pageNumber", $"{1}");
            queryString.Add("pageSize", $"{1}");

            var response = Client.GetAsync(GetBaseRoute($"all") + "?" + queryString).Result;
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseContent = response.Content.Deserialize<CustomResponseList<InvoiceDto>>().Result;
            responseContent.Should().NotBeNull();
            responseContent.Status.Should().Be("Failed");

            var data = responseContent.Data;
            data.Should().BeNullOrEmpty();
        }

        #endregion
    }
}