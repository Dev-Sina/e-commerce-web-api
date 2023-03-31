using eCommerceWebAPI.Application.Configuration.Commands;
using eCommerceWebAPI.Application.SeedWork;
using eCommerceWebAPI.Domain;
using eCommerceWebAPI.Domain.Catalogs;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace eCommerceWebAPI.Application.Catalogs.Products.Commands.AddProduct
{
    public class AddProductCommandHandler : ICommandHandler<AddProductCommand, BaseResponse>
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<SpecificationValue> _specificationValueRepository;

        public AddProductCommandHandler(IRepository<Product> productRepository,
            IRepository<Category> categoryRepository,
            IRepository<SpecificationValue> specificationValueRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _specificationValueRepository = specificationValueRepository;
        }

        public async Task<BaseResponse> Handle(AddProductCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(nameof(request));

            var addProductDto = request.AddProductDto;
            ArgumentNullException.ThrowIfNull(nameof(addProductDto));

            BaseResponse response = new();

            // Bad request parameters
            if (string.IsNullOrEmpty(addProductDto.Name?.Trim())) response.Errors.Add("Product name could not be empty!");
            if (string.IsNullOrEmpty(addProductDto.Code?.Trim())) response.Errors.Add("Product code could not be empty!");
            if (response.Errors.Any())
            {
                response.HttpStatusCode = HttpStatusCode.BadRequest;
                return response;
            };

            // Discount percent/amount
            decimal? discountPercent = addProductDto.DiscountPercent;
            decimal discountAmount = addProductDto.DiscountAmount;
            if (discountPercent != null && discountPercent > 0 && discountPercent <= 100)
            {
                discountAmount = addProductDto.Price * (discountPercent.Value / 100);
            }
            if (discountAmount > addProductDto.Price)
            {
                discountAmount= addProductDto.Price;
            }
            else if (discountAmount < 0)
            {
                discountAmount = 0;
            }

            // Get existing adding category ids
            var addProductCategoryMappings = addProductDto.AddProductCategoryMappings ?? new();
            if (addProductCategoryMappings != null && addProductCategoryMappings.Any())
            {
                var addingCategoryIds = addProductCategoryMappings.Select(pcm => pcm.CategoryId).DistinctBy(x => x).ToList();
                var existingCategoryIds = _categoryRepository.AsQueryable().AsNoTracking().Select(c => c.Id).Where(x => addingCategoryIds.Contains(x)).ToList();
                addProductCategoryMappings = addProductCategoryMappings.Where(pcm => existingCategoryIds.Contains(pcm.CategoryId)).ToList();
            }

            // Get existing adding specification value ids
            var addSpecificationValueIds = addProductDto.AddSpecificationValueIds ?? new();
            if (addSpecificationValueIds != null && addSpecificationValueIds.Any())
            {
                var existingSpecificationValueIds = _specificationValueRepository.AsQueryable().AsNoTracking().Select(sv => sv.Id).Where(x => addSpecificationValueIds.Contains(x)).ToList();
                addSpecificationValueIds = addSpecificationValueIds.Where(svId => existingSpecificationValueIds.Contains(svId)).ToList();
            }

            // Create product entity
            Product product = new()
            {
                Name = addProductDto.Name!,
                Code = addProductDto.Code!,
                ShortDescription = addProductDto.ShortDescription,
                FullDescription = addProductDto.FullDescription,
                Price = addProductDto.Price,
                DiscountAmount = discountAmount,
                StockQuantity = addProductDto.StockQuantity,
                Currency = "IRT",
                Deleted = false
            };
            addProductCategoryMappings!.ForEach(addProductCategoryMapping => product.ProductCategoryMappings.Add(new() { CategoryId = addProductCategoryMapping.CategoryId, DisplayOrder = addProductCategoryMapping.DisplayOrder }));
            addSpecificationValueIds!.ForEach(addSpecificationValueId => product.ProductSpecificationValueMappings.Add(new() { SpecificationValueId = addSpecificationValueId }));

            // Inserting product into the Db
            var insertedProduct = await _productRepository.Add(product);

            // Check it's inserted or not
            if (insertedProduct.Id <= 0)
            {
                response.HttpStatusCode = HttpStatusCode.InternalServerError;
                response.Errors.Add("An error has been occured while inserting product!");
                return response;
            }

            response.HttpStatusCode = HttpStatusCode.Created;
            response.Message = "The product has been inserted successfully!";
            return response;
        }
    }
}
