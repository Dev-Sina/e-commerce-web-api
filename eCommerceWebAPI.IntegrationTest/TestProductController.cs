using AngleSharp.Common;
using eCommerceWebAPI.Api.SeedWork;
using eCommerceWebAPI.Application.Catalogs.Products;
using eCommerceWebAPI.Application.Catalogs.Products.Commands.AddProduct;
using eCommerceWebAPI.Application.Catalogs.Products.Commands.EditProduct;
using eCommerceWebAPI.Application.Extensions;
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
    public class TestProductController : BaseControllerTest
    {
        public TestProductController()
        {
            BaseRoute = "product";
        }

        #region Create

        [Test]
        public void TestCreate()
        {
            var categories = SampleTestEntities.GetSampleCategories().Result;
            Database.Categories.AddRange(categories);
            Database.SaveChanges();

            var specifications = SampleTestEntities.GetSampleSpecifications().Result;
            Database.Specifications.AddRange(specifications);
            Database.SaveChanges();

            var rnd = new Random();
            var booleanList = new List<bool>(2) { false, true };

            var insertingProductSample = SampleTestEntities.GetSampleProduct().Result;

            List<Category> categoriesToMap;
            do
            {
                categoriesToMap = categories.Where(s => booleanList[rnd.Next(0, 2)]).ToList();
            }
            while (categoriesToMap.Count <= 0);
            var insertingProductSampleCategories = categoriesToMap;

            List<Specification> specificationsToMap;
            List<SpecificationValue> specificationValuesToMap = new();
            do
            {
                specificationsToMap = specifications.Where(s => booleanList[rnd.Next(0, 2)]).ToList();
            }
            while (specificationsToMap.Count <= 0);
            foreach(var specificationToMap in specificationsToMap)
            {
                var specificationToMapSpecificationValues = specificationToMap.SpecificationValues;
                var specificationValueToMap = specificationToMapSpecificationValues.GetItemByIndex(rnd.Next(0, specificationToMapSpecificationValues.Count));
                specificationValuesToMap.Add(specificationValueToMap);
            }
            var insertingProductSampleSpecificationValues = specificationValuesToMap;

            AddProductDto addProductDto = new()
            {
                Name = insertingProductSample.Name,
                Code = insertingProductSample.Code,
                ShortDescription = insertingProductSample.ShortDescription,
                FullDescription = insertingProductSample.FullDescription,
                Price = insertingProductSample.Price,
                DiscountAmount = insertingProductSample.DiscountAmount,
                StockQuantity = insertingProductSample.StockQuantity
            };
            foreach (var insertingProductSampleCategory in insertingProductSampleCategories)
            {
                addProductDto.AddProductCategoryMappings.Add(new()
                {
                    CategoryId = insertingProductSampleCategory.Id,
                    DisplayOrder = rnd.Next(0, 1000)
                });
            }
            addProductDto.AddSpecificationValueIds = insertingProductSampleSpecificationValues.Select(sv => sv.Id).ToList();

            var response = Client.PostAsJsonAsync(GetBaseRoute(), addProductDto).Result;
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var responseContent = response.Content.Deserialize<Response>().Result;
            responseContent.Should().NotBeNull();
            responseContent.Status.Should().Be("Success");

            var foundProduct = Database.Products.AsNoTracking().FirstOrDefault(p => p.Code == insertingProductSample.Code);
            foundProduct.Should().NotBeNull();
            foundProduct.Id.Should().BeGreaterThanOrEqualTo(1);
            foundProduct.Name.Should().Be(insertingProductSample.Name);
            foundProduct.ShortDescription.Should().Be(insertingProductSample.ShortDescription);
            foundProduct.FullDescription.Should().Be(insertingProductSample.FullDescription);
            foundProduct.Price.Should().Be(insertingProductSample.Price);
            foundProduct.DiscountAmount.Should().Be(insertingProductSample.DiscountAmount);
            foundProduct.StockQuantity.Should().Be(insertingProductSample.StockQuantity);

            var foundProductCategoryMappings = Database.ProductCategoryMappings.AsNoTracking().Where(pcm => pcm.ProductId == foundProduct.Id).ToList();
            foundProductCategoryMappings.Should().NotBeNullOrEmpty();

            var foundProductSpecificationValueMappings = Database.ProductSpecificationValueMappings.AsNoTracking().Where(psvm => psvm.ProductId == foundProduct.Id).ToList();
            foundProductSpecificationValueMappings.Should().NotBeNullOrEmpty();
        }

        #endregion

        #region Edit

        [Test]
        public void TestEdit()
        {
            var categories = SampleTestEntities.GetSampleCategories().Result;
            var specifications = SampleTestEntities.GetSampleSpecifications().Result;
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

            var products = SampleTestEntities.GetSampleProducts(categories: categories, specifications: specificationsToMap).Result;
            Database.Products.AddRange(products);
            Database.SaveChanges();

            var editingProductSample = products.GetItemByIndex(rnd.Next(0, products.Count));
            editingProductSample.Id.Should().BeGreaterThanOrEqualTo(1);
            editingProductSample.Name = StringUtilities.GenerateRandomString(minNumberOfWords: 2, maxNumberOfWords: 10, includeNumerals: true, stringCase: StringCaseEnum.PascalCaseSpaceBetween);
            editingProductSample.Code = StringUtilities.GenerateRandomString(wordsMinLength: 8, wordsMaxLength: 20, includeNumerals: true, stringCase: StringCaseEnum.UpperCaseWithoutSpace);
            editingProductSample.ShortDescription = StringUtilities.GenerateRandomSentences(minNumberOfSentences: 3, maxNumberOfSentences: 7, minNumberOfSentenceWords: 3, maxNumberOfSentenceWords: 10);
            editingProductSample.FullDescription = StringUtilities.GenerateRandomSentences(minNumberOfSentences: 10, maxNumberOfSentences: 100, minNumberOfSentenceWords: 5, maxNumberOfSentenceWords: 20);
            editingProductSample.Price = rnd.Next(100, 90000) * 1000;
            editingProductSample.DiscountAmount = rnd.Next(0, (int)editingProductSample.Price);
            editingProductSample.StockQuantity = rnd.Next(0, 100);

            List<Category> newCategoriesToMap;
            do
            {
                newCategoriesToMap = categories.Where(s => booleanList[rnd.Next(0, 2)]).ToList();
            }
            while (newCategoriesToMap.Count <= 0);
            var editingProductSampleCategories = newCategoriesToMap;

            List<Specification> newSpecificationsToMap;
            List<SpecificationValue> newSpecificationValuesToMap = new();
            do
            {
                newSpecificationsToMap = specifications.Where(s => booleanList[rnd.Next(0, 2)]).ToList();
            }
            while (newSpecificationsToMap.Count <= 0);
            foreach (var newSpecificationToMap in newSpecificationsToMap)
            {
                var specificationToMapSpecificationValues = newSpecificationToMap.SpecificationValues;
                var specificationValueToMap = specificationToMapSpecificationValues.GetItemByIndex(rnd.Next(0, specificationToMapSpecificationValues.Count));
                newSpecificationValuesToMap.Add(specificationValueToMap);
            }
            var editingProductSampleSpecificationValues = newSpecificationValuesToMap;

            EditProductDto editProductDto = new()
            {
                Name = editingProductSample.Name,
                Code = editingProductSample.Code,
                ShortDescription = editingProductSample.ShortDescription,
                FullDescription = editingProductSample.FullDescription,
                Price = editingProductSample.Price,
                DiscountAmount = editingProductSample.DiscountAmount,
                StockQuantity = editingProductSample.StockQuantity
            };
            foreach (var editingProductSampleCategory in editingProductSampleCategories)
            {
                editProductDto.EditProductCategoryMappings.Add(new()
                {
                    CategoryId = editingProductSampleCategory.Id,
                    DisplayOrder = rnd.Next(0, 1000)
                });
            }
            editProductDto.EditSpecificationValueIds = editingProductSampleSpecificationValues.Select(sv => sv.Id).ToList();

            var response = Client.PutAsJsonAsync(GetBaseRoute($"{editingProductSample.Id}"), editProductDto).Result;
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseContent = response.Content.Deserialize<Response>().Result;
            responseContent.Should().NotBeNull();
            responseContent.Status.Should().Be("Success");

            var foundProduct = Database.Products.AsNoTracking().FirstOrDefault(p => p.Id == editingProductSample.Id);
            foundProduct.Should().NotBeNull();
            foundProduct.Code.Should().Be(editingProductSample.Code);
            foundProduct.Name.Should().Be(editingProductSample.Name);
            foundProduct.ShortDescription.Should().Be(editingProductSample.ShortDescription);
            foundProduct.FullDescription.Should().Be(editingProductSample.FullDescription);
            foundProduct.Price.Should().Be(editingProductSample.Price);
            foundProduct.DiscountAmount.Should().Be(editingProductSample.DiscountAmount);
            foundProduct.StockQuantity.Should().Be(editingProductSample.StockQuantity);

            var foundProductCategoryMappings = Database.ProductCategoryMappings.AsNoTracking().Where(pcm => pcm.ProductId == foundProduct.Id).ToList();
            foundProductCategoryMappings.Should().NotBeNullOrEmpty();
            foundProductCategoryMappings.Count.Should().Be(editingProductSampleCategories.Count);

            var foundProductSpecificationValueMappings = Database.ProductSpecificationValueMappings.AsNoTracking().Where(psvm => psvm.ProductId == foundProduct.Id).ToList();
            foundProductSpecificationValueMappings.Should().NotBeNullOrEmpty();
            foundProductSpecificationValueMappings.Count.Should().Be(editingProductSampleSpecificationValues.Count);
        }

        #endregion

        #region Delete

        [Test]
        public void TestDelete()
        {
            var categories = SampleTestEntities.GetSampleCategories().Result;
            var specifications = SampleTestEntities.GetSampleSpecifications().Result;
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

            var products = SampleTestEntities.GetSampleProducts(categories: categories, specifications: specificationsToMap).Result;
            Database.Products.AddRange(products);
            Database.SaveChanges();

            var deletingProductSample = products.GetItemByIndex(rnd.Next(0, products.Count));
            deletingProductSample.Should().NotBeNull();

            var response = Client.DeleteAsync(GetBaseRoute($"{deletingProductSample.Id}")).Result;
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseContent = response.Content.Deserialize<Response>().Result;
            responseContent.Should().NotBeNull();
            responseContent.Status.Should().Be("Success");

            var foundProduct = Database.Products.AsNoTracking().FirstOrDefault(p => p.Id == deletingProductSample.Id);
            foundProduct.Should().BeNull();
        }

        #endregion

        #region GetAll

        [Test]
        public void TestGetAll_WithoutQueryParameters()
        {
            var categories = SampleTestEntities.GetSampleCategories().Result;
            var specifications = SampleTestEntities.GetSampleSpecifications().Result;
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

            var products = SampleTestEntities.GetSampleProducts(categories: categories, specifications: specificationsToMap).Result;
            Database.Products.AddRange(products);
            Database.SaveChanges();

            var response = Client.GetAsync(GetBaseRoute("all")).Result;
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseContent = response.Content.Deserialize<CustomResponseList<ProductDto>>().Result;
            responseContent.Should().NotBeNull();
            responseContent.Status.Should().Be("Success");

            var data = responseContent.Data;
            data.Should().NotBeNull();
            data.Should().HaveCountGreaterThanOrEqualTo(products.Count);

            var chosenProduct = products.GetItemByIndex(rnd.Next(0, products.Count));
            var dataChosenProductDto = data.FirstOrDefault(pDto => pDto.Code == chosenProduct.Code);
            dataChosenProductDto.Should().NotBeNull();
            dataChosenProductDto.Id.Should().BeGreaterThanOrEqualTo(1);
            dataChosenProductDto.Name.Should().Be(chosenProduct.Name);
            dataChosenProductDto.ShortDescription.Should().Be(chosenProduct.ShortDescription);
            dataChosenProductDto.FullDescription.Should().Be(chosenProduct.FullDescription);
            dataChosenProductDto.Price.Should().Be(chosenProduct.Price);
            dataChosenProductDto.DiscountAmount.Should().Be(chosenProduct.DiscountAmount);
            dataChosenProductDto.StockQuantity.Should().Be(chosenProduct.StockQuantity);
        }

        [Test]
        public void TestGetAll_WithQueryParameters()
        {
            var categories = SampleTestEntities.GetSampleCategories().Result;
            var specifications = SampleTestEntities.GetSampleSpecifications().Result;
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

            var products = SampleTestEntities.GetSampleProducts(categories: categories, specifications: specificationsToMap).Result;
            Database.Products.AddRange(products);
            Database.SaveChanges();

            var queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString.Add("pageNumber", $"{2}");
            queryString.Add("pageSize", $"{4}");

            var response = Client.GetAsync(GetBaseRoute("all") + "?" + queryString).Result;
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseContent = response.Content.Deserialize<CustomResponseList<ProductDto>>().Result;
            responseContent.Should().NotBeNull();
            responseContent.Status.Should().Be("Success");

            var data = responseContent.Data;
            data.Should().NotBeNull();
            data.Should().HaveCountGreaterThanOrEqualTo(3);

            var dataChosenProductDto = data.GetItemByIndex(rnd.Next(0, 3));
            var chosenProduct = products.FirstOrDefault(p => p.Code == dataChosenProductDto.Code);
            chosenProduct.Should().NotBeNull();
            dataChosenProductDto.Id.Should().BeGreaterThanOrEqualTo(1);
            dataChosenProductDto.Name.Should().Be(chosenProduct.Name);
            dataChosenProductDto.ShortDescription.Should().Be(chosenProduct.ShortDescription);
            dataChosenProductDto.FullDescription.Should().Be(chosenProduct.FullDescription);
            dataChosenProductDto.Price.Should().Be(chosenProduct.Price);
            dataChosenProductDto.DiscountAmount.Should().Be(chosenProduct.DiscountAmount);
            dataChosenProductDto.StockQuantity.Should().Be(chosenProduct.StockQuantity);
        }

        [Test]
        public void TestGetAll_WithoutQueryParameters_NoProducts()
        {
            var response = Client.GetAsync(GetBaseRoute("all")).Result;
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseContent = response.Content.Deserialize<CustomResponseList<ProductDto>>().Result;
            responseContent.Should().NotBeNull();
            responseContent.Status.Should().Be("Failed");

            var data = responseContent.Data;
            data.Should().NotBeNull();
            data.Should().BeEmpty();
        }

        [Test]
        public void TestGetAll_WithQueryParameters_NoProducts()
        {
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString.Add("pageNumber", $"{2}");
            queryString.Add("pageSize", $"{4}");

            var response = Client.GetAsync(GetBaseRoute("all") + "?" + queryString).Result;
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseContent = response.Content.Deserialize<CustomResponseList<ProductDto>>().Result;
            responseContent.Should().NotBeNull();
            responseContent.Status.Should().Be("Failed");

            var data = responseContent.Data;
            data.Should().NotBeNull();
            data.Should().BeEmpty();
        }

        #endregion

        #region GetSimilarProductsByProductId

        [Test]
        public void TestGetSimilarProductsByProductId_WithoutQueryParameters()
        {
            var categories = SampleTestEntities.GetSampleCategories().Result;
            var specifications = SampleTestEntities.GetSampleSpecifications().Result;
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

            var products = SampleTestEntities.GetSampleProducts(categories: categories, specifications: specificationsToMap).Result;
            Database.Products.AddRange(products);
            Database.SaveChanges();

            var chosenProduct = products.GetItemByIndex(new Random().Next(0, products.Count));
            chosenProduct.Should().NotBeNull();
            chosenProduct.Id.Should().BeGreaterThanOrEqualTo(1);

            var response = Client.GetAsync(GetBaseRoute($"similar/{chosenProduct.Id}")).Result;
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseContent = response.Content.Deserialize<CustomResponseList<ProductDto>>().Result;
            responseContent.Should().NotBeNull();

            var data = responseContent.Data;
            data.Should().NotBeNull();

            if (data.Count > 0)
            {
                responseContent.Status.Should().Be("Success");
            }
            else
            {
                responseContent.Status.Should().Be("Failed");
            }
        }

        [Test]
        public void TestGetSimilarProductsByProductId_WithQueryParameters()
        {
            var categories = SampleTestEntities.GetSampleCategories().Result;
            var specifications = SampleTestEntities.GetSampleSpecifications().Result;
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

            var products = SampleTestEntities.GetSampleProducts(categories: categories, specifications: specificationsToMap).Result;
            Database.Products.AddRange(products);
            Database.SaveChanges();

            var chosenProduct = products.GetItemByIndex(new Random().Next(0, products.Count));
            chosenProduct.Should().NotBeNull();
            chosenProduct.Id.Should().BeGreaterThanOrEqualTo(1);

            var queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString.Add("pageNumber", $"{1}");
            queryString.Add("pageSize", $"{1}");

            var response = Client.GetAsync(GetBaseRoute($"similar/{chosenProduct.Id}") + "?" + queryString).Result;
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseContent = response.Content.Deserialize<CustomResponseList<ProductDto>>().Result;
            responseContent.Should().NotBeNull();

            var data = responseContent.Data;
            data.Should().NotBeNull();

            if (data.Count > 0)
            {
                responseContent.Status.Should().Be("Success");
            }
            else
            {
                responseContent.Status.Should().Be("Failed");
            }
        }

        #endregion
    }
}