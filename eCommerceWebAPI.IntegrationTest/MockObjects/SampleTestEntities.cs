using AngleSharp.Common;
using eCommerceWebAPI.Application.Extensions;
using eCommerceWebAPI.Domain.Addresses;
using eCommerceWebAPI.Domain.Catalogs;
using eCommerceWebAPI.Domain.Customers;
using eCommerceWebAPI.Domain.Invoices;
using eCommerceWebAPI.Domain.Orders;
using eCommerceWebAPI.Domain.ShoppingCarts;

namespace eCommerceWebAPI.IntegrationTest.MockObjects
{
    public class SampleTestEntities
    {
        #region Catalog

        public static async Task<Category> GetSampleCategory() => (await GetSampleCategories(1)).FirstOrDefault()!;
        public static async Task<List<Category>> GetSampleCategories(int count = 0)
        {
            var rnd = new Random();

            if (count <= 0)
            {
                count = rnd.Next(1, 11);
            }

            List<Category> categories = new();

            for (int i = 0; i < count; i++)
            {
                Category category = new()
                {
                    Name = StringUtilities.GenerateRandomString(minNumberOfWords: 1, maxNumberOfWords: 3, stringCase: StringCaseEnum.LowerCaseSpaceBetweenFirstCharUpperCase),
                    Description = StringUtilities.GenerateRandomSentences(minNumberOfSentences: 3, maxNumberOfSentences: 7, minNumberOfSentenceWords: 3, maxNumberOfSentenceWords: 10),
                    DisplayOrder = rnd.Next(0, 1000)
                };

                bool hasChildCategory;
                do
                {
                    hasChildCategory = new List<bool> { false, true }[rnd.Next(0, 2)];
                    if (hasChildCategory)
                    {
                        category.ChildCategories.Add(new()
                        {
                            Name = StringUtilities.GenerateRandomString(minNumberOfWords: 1, maxNumberOfWords: 3, stringCase: StringCaseEnum.LowerCaseSpaceBetweenFirstCharUpperCase),
                            Description = StringUtilities.GenerateRandomSentences(minNumberOfSentences: 3, maxNumberOfSentences: 7, minNumberOfSentenceWords: 3, maxNumberOfSentenceWords: 10),
                            DisplayOrder = rnd.Next(0, 1000)
                        });
                    }
                }
                while (hasChildCategory);

                categories.Add(category);
            }

            return await Task.FromResult(categories);
        }

        public static async Task<Specification> GetSampleSpecification(bool includeSpecificationValues = true) => (await GetSampleSpecifications(1, includeSpecificationValues)).FirstOrDefault()!;
        public static async Task<List<Specification>> GetSampleSpecifications(int count = 0, bool includeSpecificationValues = true)
        {
            var rnd = new Random();

            if (count <= 0)
            {
                count = rnd.Next(1, 11);
            }

            List<Specification> specifications = new();

            for (int i = 0; i < count; i++)
            {
                Specification specification = new()
                {
                    Name = StringUtilities.GenerateRandomString(minNumberOfWords: 1, maxNumberOfWords: 3, stringCase: StringCaseEnum.LowerCaseSpaceBetweenFirstCharUpperCase),
                    DisplayOrder = rnd.Next(0, 1000)
                };

                if (includeSpecificationValues)
                {
                    var specificationValues = await GetSampleSpecificationValues();
                    foreach (var specificationValue in specificationValues)
                    {
                        specification.SpecificationValues.Add(specificationValue);
                    }
                }

                specifications.Add(specification);
            }

            return await Task.FromResult(specifications);
        }

        public static async Task<SpecificationValue> GetSampleSpecificationValue() => (await GetSampleSpecificationValues(1)).FirstOrDefault()!;
        public static async Task<List<SpecificationValue>> GetSampleSpecificationValues(int count = 0)
        {
            var rnd = new Random();

            if (count <= 0)
            {
                count = rnd.Next(1, 11);
            }

            List<SpecificationValue> specificationValues = new();

            for (int i = 0; i < count; i++)
            {
                SpecificationValue specificationValue = new()
                {
                    Name = StringUtilities.GenerateRandomString(minNumberOfWords: 1, maxNumberOfWords: 3, stringCase: StringCaseEnum.LowerCaseSpaceBetweenFirstCharUpperCase),
                    DisplayOrder = rnd.Next(0, 1000)
                };

                specificationValues.Add(specificationValue);
            }

            return await Task.FromResult(specificationValues);
        }

        public static async Task<Product> GetSampleProduct(List<Category>? categories = null, List<Specification>? specifications = null) => (await GetSampleProducts(1, categories, specifications)).FirstOrDefault()!;
        public static async Task<List<Product>> GetSampleProducts(int count = 0, List<Category>? categories = null, List<Specification>? specifications = null)
        {
            var rnd = new Random();

            if (count <= 0)
            {
                count = rnd.Next(1, 21);
            }

            List<Product> products = new();

            for (int i = 0; i < count; i++)
            {
                decimal randomPrice = rnd.Next(100, 90000) * 1000;
                decimal randomDiscountAmount = rnd.Next(0, (int)randomPrice);
                Product product = new()
                {
                    Name = StringUtilities.GenerateRandomString(minNumberOfWords: 2, maxNumberOfWords: 10, includeNumerals: true, stringCase: StringCaseEnum.PascalCaseSpaceBetween),
                    Code = StringUtilities.GenerateRandomString(wordsMinLength: 8, wordsMaxLength: 20, includeNumerals: true, stringCase: StringCaseEnum.UpperCaseWithoutSpace),
                    ShortDescription = StringUtilities.GenerateRandomSentences(minNumberOfSentences: 3, maxNumberOfSentences: 7, minNumberOfSentenceWords: 3, maxNumberOfSentenceWords: 10),
                    FullDescription = StringUtilities.GenerateRandomSentences(minNumberOfSentences: 10, maxNumberOfSentences: 100, minNumberOfSentenceWords: 5, maxNumberOfSentenceWords: 20),
                    Price = randomPrice,
                    DiscountAmount = randomDiscountAmount,
                    StockQuantity = rnd.Next(0, 100),
                    Currency = "IRT"
                };

                if (categories != null && categories.Any())
                {
                    foreach (var category in categories)
                    {
                        product.ProductCategoryMappings.Add(new()
                        {
                            Category = category,
                            DisplayOrder = rnd.Next(0, 1000)
                        });
                    }
                }

                if (specifications != null && specifications.Any())
                {
                    foreach (var specification in specifications)
                    {
                        var specificationValues = specification.SpecificationValues;
                        int specificationValuesCount = specificationValues.Count;
                        if (specificationValuesCount <= 0)
                        {
                            continue;
                        }

                        var selectedSpecificationValue = specificationValues.GetItemByIndex(rnd.Next(0, specificationValuesCount));
                        product.ProductSpecificationValueMappings.Add(new()
                        {
                            SpecificationValue = selectedSpecificationValue
                        });
                    }
                }

                products.Add(product);
            }

            return await Task.FromResult(products);
        }

        #endregion

        #region Address

        public static async Task<Country> GetSampleCountry() => (await GetSampleCountries(1)).FirstOrDefault()!;
        public static async Task<List<Country>> GetSampleCountries(int count = 0)
        {
            var rnd = new Random();

            if (count <= 0)
            {
                count = rnd.Next(1, 31);
            }

            List<Country> countries = new();

            for (int i = 0; i < count; i++)
            {
                Country country = new()
                {
                    Name = StringUtilities.GenerateRandomName(),
                    DisplayOrder = rnd.Next(0, 1000)
                };

                int provincesCount = rnd.Next(1, 50);
                List<Province> provinces = new(provincesCount);
                for (int j = 0; j < provincesCount; j++)
                {
                    Province province = new()
                    {
                        Name = StringUtilities.GenerateRandomName(),
                        DisplayOrder = rnd.Next(0, 1000)
                    };

                    int citiesCount = rnd.Next(1, 100);
                    List<City> cities = new(citiesCount);
                    for (int k = 0; k < provincesCount; k++)
                    {
                        City city = new()
                        {
                            Name = StringUtilities.GenerateRandomName(),
                            DisplayOrder = rnd.Next(0, 1000)
                        };

                        province.Cities.Add(city);
                    }

                    country.Provinces.Add(province);
                }

                countries.Add(country);
            }

            return await Task.FromResult(countries);
        }

        public static async Task<Address> GetSampleAddress() => (await GetSampleAddresses(1)).FirstOrDefault()!;
        public static async Task<List<Address>> GetSampleAddresses(int count = 0)
        {
            var rnd = new Random();

            if (count <= 0)
            {
                count = rnd.Next(1, 11);
            }

            var countries = await GetSampleCountries(count);

            List<Address> addresses = new();

            for (int i = 0; i < count; i++)
            {
                var chosenCountry = countries.GetItemByIndex(rnd.Next(0, countries.Count));
                var chosenProvince = chosenCountry.Provinces.GetItemByIndex(rnd.Next(0, chosenCountry.Provinces.Count));
                var chosenCity = chosenProvince.Cities.GetItemByIndex(rnd.Next(0, chosenProvince.Cities.Count));
                var streetAddress = StringUtilities.GenerateRandomSentences(minNumberOfSentences: 3, maxNumberOfSentences: 6, minNumberOfSentenceWords: 2, maxNumberOfSentenceWords: 4, endingChars: ",");
                streetAddress = streetAddress.Substring(0, streetAddress.Length - 1);

                Address address = new()
                {
                    Title = StringUtilities.GenerateRandomString(minNumberOfWords: 1, maxNumberOfWords: 3, stringCase: StringCaseEnum.PascalCaseSpaceBetween),
                    FirstName = StringUtilities.GenerateRandomName(),
                    LastName = StringUtilities.GenerateRandomName(),
                    CountryName = chosenCountry.Name,
                    ProvinceName = chosenProvince.Name,
                    CityName = chosenCity.Name,
                    StreetAddress = streetAddress,
                    PostalCode = StringUtilities.GenerateRandomPostalCode(),
                    PhoneNumber = StringUtilities.GenerateRandomPhoneNumber()
                };

                addresses.Add(address);
            }

            return await Task.FromResult(addresses);
        }

        #endregion

        #region Customer

        public static async Task<Customer> GetSampleCustomer(List<Address>? addresses = null) => (await GetSampleCustomers(1, addresses)).FirstOrDefault()!;
        public static async Task<List<Customer>> GetSampleCustomers(int count = 0, List<Address>? addresses = null)
        {
            var rnd = new Random();
            var booleanList = new List<bool>(2) { false, true };

            if (count <= 0)
            {
                count = rnd.Next(1, 11);
            }

            List<Customer> customers = new();

            for (int i = 0; i < count; i++)
            {
                var minutesAddedToDateTimeUtc = rnd.Next(0, 60 * 24 * 365) * -1;
                Customer customer = new()
                {
                    FirstName = StringUtilities.GenerateRandomName(),
                    LastName = StringUtilities.GenerateRandomName(),
                    NationalCode = StringUtilities.GenerateRandomNationalCode(),
                    CreatedOnUtc = DateTime.UtcNow.AddMinutes(minutesAddedToDateTimeUtc)
                };

                if (addresses != null && addresses.Any())
                {
                    var customerAddresses = addresses.Where(a => booleanList[rnd.Next(0, 2)]).ToList();
                    if (!customerAddresses.Any())
                    {
                        customerAddresses = new() { addresses.GetItemByIndex(rnd.Next(0, addresses.Count)) };
                    }

                    foreach (var address in addresses)
                    {
                        customer.CustomerAddressMappings.Add(new()
                        {
                            Address = address
                        });
                    }
                }

                customers.Add(customer);
            }

            return await Task.FromResult(customers);
        }

        #endregion

        #region Shopping cart

        public static async Task<ShoppingCartItem> GetSampleShoppingCartItem(long customerId, List<Product> products) => (await GetSampleShoppingCartItems(new List<long> { customerId }, products)).FirstOrDefault()!;
        public static async Task<List<ShoppingCartItem>> GetSampleShoppingCartItems(long customerId, List<Product> products) => await GetSampleShoppingCartItems(new List<long> { customerId }, products);
        public static async Task<List<ShoppingCartItem>> GetSampleShoppingCartItems(List<long> customerIds, List<Product> products)
        {
            var rnd = new Random();
            var booleanList = new List<bool>(2) { false, true };

            customerIds = customerIds.Where(x => x > 0).ToList();
            if (!customerIds.Any())
            {
                return new();
            }

            products = products.Where(p => !p.Deleted && p.StockQuantity > 0 && p.Id > 0).ToList();
            if (!products.Any())
            {
                return new();
            }

            List<ShoppingCartItem> shoppingCartItems = new();

            foreach (int customerId in customerIds)
            {
                List<Product> chosenProducts;
                int chosenProductsCount = 0;

                do
                {
                    chosenProducts = products.Where(p => booleanList[rnd.Next(0, 2)]).ToList();
                    chosenProductsCount = chosenProducts.Count;
                }
                while (chosenProductsCount <= 0);

                int shoppingCartItemsCount = rnd.Next(1, products.Count);

                for (int i = 0; i < chosenProductsCount; i++)
                {
                    var chosenProduct = chosenProducts[i];

                    ShoppingCartItem shoppingCartItem = new()
                    {
                        CustomerId = customerId,
                        ProductId = chosenProduct.Id,
                        ProductName = chosenProduct.Name,
                        ProductCode = chosenProduct.Code,
                        ProductUnitPriceExcludeTax = chosenProduct.Price,
                        ProductUnitPriceIncludeTax = chosenProduct.Price * (decimal)1.09,
                        ProductUnitDiscountAmountExcludeTax = chosenProduct.DiscountAmount,
                        ProductUnitDiscountAmountIncludeTax = chosenProduct.DiscountAmount * (decimal)1.09,
                        Quantity = rnd.Next(1, chosenProduct.StockQuantity),
                        Currency = chosenProduct.Currency
                    };

                    shoppingCartItems.Add(shoppingCartItem);
                }
            }

            return await Task.FromResult(shoppingCartItems);
        }

        #endregion

        #region Order

        public static async Task<Order> GetSampleOrder(long customerId, Address address, List<Product> products) => (await GetSampleOrders(new List<long> { customerId }, new List<Address> { address }, products)).FirstOrDefault()!;
        public static async Task<List<Order>> GetSampleOrders(List<long> customerIds, List<Address> addresses, List<Product> products)
        {
            var rnd = new Random();
            var booleanList = new List<bool>(2) { false, true };

            customerIds = customerIds.Where(x => x > 0).ToList();
            if (!customerIds.Any())
            {
                return new();
            }

            addresses = addresses.Where(a => a.Id <= 0).ToList();
            if (!addresses.Any())
            {
                return new();
            }

            products = products.Where(p => !p.Deleted && p.Id > 0).ToList();
            if (!products.Any())
            {
                return new();
            }

            int count = Math.Min(customerIds.Count, addresses.Count);

            List<Order> orders = new();

            for (int i = 0; i < count; i++)
            {
                var customerId = customerIds[i];
                var address = addresses[i];
                var minutesAddedToDateTimeUtc = rnd.Next(0, 60 * 24 * 365) * -1;
                decimal randomShippingCost = rnd.Next(0, 120) * 1000;
                decimal orderDiscountAmount = rnd.Next(0, 50) * 1000;

                Order order = new()
                {
                    CustomerId = customerId,
                    CreatedOnUtc = DateTime.UtcNow.AddMinutes(minutesAddedToDateTimeUtc),
                    ShippingCostExcludeTax = randomShippingCost,
                    ShippingCostIncludeTax = randomShippingCost * (decimal)1.09,
                    OrderDiscountAmountExcludeTax = orderDiscountAmount,
                    OrderDiscountAmountIncludeTax = orderDiscountAmount * (decimal)1.09,
                    Currency = "IRT",
                    Address = address
                };

                List<Product> chosenProducts;
                int chosenProductsCount = 0;

                do
                {
                    chosenProducts = products.Where(p => booleanList[rnd.Next(0, 2)]).ToList();
                    chosenProductsCount = chosenProducts.Count;
                }
                while (chosenProductsCount <= 0);

                int ordertItemsCount = rnd.Next(1, products.Count);

                for (int j = 0; j < chosenProductsCount; j++)
                {
                    var chosenProduct = chosenProducts[j];

                    OrderItem orderItem = new()
                    {
                        ProductId = chosenProduct.Id,
                        ProductName = chosenProduct.Name,
                        ProductCode = chosenProduct.Code,
                        ProductUnitPriceExcludeTax = chosenProduct.Price,
                        ProductUnitPriceIncludeTax = chosenProduct.Price * (decimal)1.09,
                        ProductUnitDiscountAmountExcludeTax = chosenProduct.DiscountAmount,
                        ProductUnitDiscountAmountIncludeTax = chosenProduct.DiscountAmount * (decimal)1.09,
                        Quantity = rnd.Next(1, 6)
                    };

                    order.OrderItems.Add(orderItem);
                }

                orders.Add(order);
            }

            return await Task.FromResult(orders);
        }

        #endregion

        #region Invoice

        public static async Task<Invoice> GetSampleInvoice(Order order) => (await GetSampleInvoices(new List<Order> { order })).FirstOrDefault()!;
        public static async Task<List<Invoice>> GetSampleInvoices(List<Order> orders)
        {
            var rnd = new Random();

            orders = orders
                .Where(o =>
                    !o.Deleted &&
                    o.Id > 0 &&
                    o.OrderItems != null &&
                    o.OrderItems.Any(oi => oi.Id > 0) &&
                    o.AddressId > 0)
                .ToList();
            if (!orders.Any())
            {
                return new();
            }

            List<Invoice> invoices = new();

            foreach (var order in orders)
            {
                var orderAddress = order.Address;
                var orderItems = order.OrderItems.ToList();

                decimal taxCoefficient = (decimal)1.09;

                decimal orderItemsTotalPriceExcludeTax = orderItems.Select(oi => oi.ProductUnitPriceExcludeTax * oi.Quantity).Sum();
                decimal orderItemsTotalPriceIncludeTax = orderItems.Select(oi => oi.ProductUnitPriceIncludeTax * oi.Quantity).Sum();
                decimal orderItemsTotalDiscountAmountsExcludeTax = orderItems.Select(oi => oi.ProductUnitDiscountAmountExcludeTax * oi.Quantity).Sum();
                decimal orderItemsTotalDiscountAmountsIncludeTax = orderItems.Select(oi => oi.ProductUnitDiscountAmountIncludeTax * oi.Quantity).Sum();

                decimal orderTotalDiscountAmountExcludeTax = order.OrderDiscountAmountExcludeTax + orderItemsTotalDiscountAmountsExcludeTax;
                decimal orderTotalDiscountAmountIncludeTax = order.OrderDiscountAmountExcludeTax + orderItemsTotalDiscountAmountsIncludeTax;

                decimal orderPayableAmountExcludeTax = orderItemsTotalPriceExcludeTax + order.ShippingCostExcludeTax - orderTotalDiscountAmountExcludeTax;
                decimal orderPayableAmount = orderItemsTotalPriceIncludeTax + order.ShippingCostIncludeTax - orderTotalDiscountAmountIncludeTax;

                Invoice invoice = new()
                {
                    OrderId = order.Id,
                    OrderCreatedOnUtc = order.CreatedOnUtc,
                    OrderShippingCostExcludeTax = order.ShippingCostExcludeTax,
                    OrderShippingCostIncludeTax = order.ShippingCostIncludeTax,
                    OrderDiscountAmountExcludeTax = order.OrderDiscountAmountExcludeTax,
                    OrderDiscountAmountIncludeTax = order.OrderDiscountAmountIncludeTax,
                    OrderTotalDiscountAmountExcludeTax = orderTotalDiscountAmountExcludeTax,
                    OrderTotalDiscountAmountIncludeTax = orderTotalDiscountAmountIncludeTax,
                    OrderPayableAmount = orderPayableAmount,
                    Currency = "IRT",
                    InvoiceCreatedOnUtc = order.CreatedOnUtc.AddMinutes(rnd.Next(0, 60))
                };

                invoice.InvoiceAddress = new()
                {
                    Title = order.Address.Title,
                    FirstName = order.Address.FirstName,
                    LastName = order.Address.LastName,
                    CountryName = order.Address.CountryName,
                    ProvinceName = order.Address.ProvinceName,
                    CityName = order.Address.CityName,
                    StreetAddress = order.Address.StreetAddress,
                    PostalCode = order.Address.PostalCode,
                    PhoneNumber = order.Address.PhoneNumber
                };

                foreach (var orderItem in orderItems)
                {
                    invoice.InvoiceItems.Add(new()
                    {
                        OrderId = orderItem.OrderId,
                        ProductId = orderItem.Id,
                        ProductName = orderItem.ProductName,
                        ProductCode = orderItem.ProductCode,
                        ProductUnitPriceExcludeTax = orderItem.ProductUnitPriceExcludeTax,
                        ProductUnitPriceIncludeTax = orderItem.ProductUnitPriceIncludeTax,
                        ProductUnitDiscountAmountExcludeTax = orderItem.ProductUnitDiscountAmountExcludeTax,
                        ProductUnitDiscountAmountIncludeTax = orderItem.ProductUnitDiscountAmountIncludeTax,
                        Quantity = orderItem.Quantity
                    });
                }

                invoices.Add(invoice);
            }

            return await Task.FromResult(invoices);
        }

        #endregion
    }
}
