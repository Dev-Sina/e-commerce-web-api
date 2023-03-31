using eCommerceWebAPI.Application.Catalogs.Products.Commands.AddProduct;
using eCommerceWebAPI.Application.Configuration.Commands;
using eCommerceWebAPI.Application.SeedWork;
using eCommerceWebAPI.Domain;
using eCommerceWebAPI.Domain.Catalogs;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace eCommerceWebAPI.Application.Catalogs.Products.Commands.EditProduct
{
    public class EditProductCommandHandler : ICommandHandler<EditProductCommand, BaseResponse>
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<SpecificationValue> _specificationValueRepository;
        private readonly IRepository<ProductCategoryMapping> _productCategoryMappingRepository;
        private readonly IRepository<ProductSpecificationValueMapping> _productSpecificationValueMappingRepository;

        public EditProductCommandHandler(IRepository<Product> productRepository,
            IRepository<Category> categoryRepository,
            IRepository<SpecificationValue> specificationValueRepository,
            IRepository<ProductCategoryMapping> productCategoryMappingRepository,
            IRepository<ProductSpecificationValueMapping> productSpecificationValueMappingRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _specificationValueRepository = specificationValueRepository;
            _productCategoryMappingRepository = productCategoryMappingRepository;
            _productSpecificationValueMappingRepository = productSpecificationValueMappingRepository;
        }

        public async Task<BaseResponse> Handle(EditProductCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(nameof(request));

            var editProductDto = request.EditProductDto;
            ArgumentNullException.ThrowIfNull(nameof(editProductDto));

            BaseResponse response = new();

            // Check editing product would be existed
            var productId = request.ProductId;
            var product = await _productRepository.Get(productId);
            if (product == null || product.Deleted)
            {
                response.Errors.Add($"Product with ID of {productId} is not found!");
                response.HttpStatusCode = HttpStatusCode.BadRequest;
                return response;
            }

            // Bad request parameters
            if (string.IsNullOrEmpty(editProductDto.Name?.Trim())) response.Errors.Add("Product name could not be empty!");
            if (string.IsNullOrEmpty(editProductDto.Code?.Trim())) response.Errors.Add("Product code could not be empty!");
            if (response.Errors.Any())
            {
                response.HttpStatusCode = HttpStatusCode.BadRequest;
                return response;
            };

            // Discount percent/amount
            decimal? discountPercent = editProductDto.DiscountPercent;
            decimal discountAmount = editProductDto.DiscountAmount;
            if (discountPercent != null && discountPercent > 0 && discountPercent <= 100)
            {
                discountAmount = editProductDto.Price * (discountPercent.Value / 100);
            }
            if (discountAmount > editProductDto.Price)
            {
                discountAmount = editProductDto.Price;
            }
            else if (discountAmount < 0)
            {
                discountAmount = 0;
            }

            // Edit product entity properties
            product.Name = editProductDto.Name!;
            product.Code = editProductDto.Code!;
            product.ShortDescription = editProductDto.ShortDescription;
            product.FullDescription = editProductDto.FullDescription;
            product.Price = editProductDto.Price;
            product.DiscountAmount = discountAmount;
            product.StockQuantity = editProductDto.StockQuantity;
            product.Currency = "IRT";
            product.Deleted = false;

            // Editing product into the Db
            await _productRepository.Update(product);

            // Product categories
            var existingProductCategoryMappings = _productCategoryMappingRepository.AsQueryable().Where(pcm => pcm.ProductId == productId).ToList() ?? new();
            var existingProductCategoryMappingCategoryIds = existingProductCategoryMappings.Select(pcm => pcm.CategoryId).ToList();
            var editProductCategoryMappings = editProductDto.EditProductCategoryMappings ?? new();
            if (editProductCategoryMappings != null && editProductCategoryMappings.Any())
            {
                // Remove not existed category ids from list
                var editingCategoryIds = editProductCategoryMappings.Select(pcm => pcm.CategoryId).DistinctBy(x => x).ToList();
                var existingCategoryIds = _categoryRepository.AsQueryable().AsNoTracking().Select(c => c.Id).Where(x => editingCategoryIds.Contains(x)).ToList();
                editProductCategoryMappings = editProductCategoryMappings.Where(pcm => existingCategoryIds.Contains(pcm.CategoryId)).ToList();
                var editProductCategoryMappingCaegoryIds = editProductCategoryMappings.Select(pcm => pcm.CategoryId).ToList();

                // Add new product categories found
                var addingEditProductCategoryMappings = editProductCategoryMappings.Where(pcm => !existingProductCategoryMappingCategoryIds.Contains(pcm.CategoryId)).ToList();
                if (addingEditProductCategoryMappings.Any())
                {
                    List<ProductCategoryMapping> addingProductCategoryMappings = new();
                    addingEditProductCategoryMappings.ForEach(addingEditProductCategoryMapping => addingProductCategoryMappings.Add(new() { ProductId = productId, CategoryId = addingEditProductCategoryMapping.CategoryId, DisplayOrder = addingEditProductCategoryMapping.DisplayOrder }));
                    await _productCategoryMappingRepository.AddRange(addingProductCategoryMappings, false);
                }

                // Remove product categories which are not requested now
                var removingProductCategoryMappings = existingProductCategoryMappings.Where(pcm => !editProductCategoryMappingCaegoryIds.Contains(pcm.CategoryId)).ToList();
                if (removingProductCategoryMappings.Any())
                {
                    removingProductCategoryMappings.ForEach(async removingProductCategoryMapping => await _productCategoryMappingRepository.Delete(removingProductCategoryMapping, false));
                }

                // Edit existing product categories (Display order or etc.)
                var editingEditProductCategoryMappings = existingProductCategoryMappings.Where(pcm => editProductCategoryMappingCaegoryIds.Contains(pcm.CategoryId)).ToList();
                if (editingEditProductCategoryMappings.Any())
                {
                    foreach (var editingEditProductCategoryMapping in editingEditProductCategoryMappings)
                    {
                        var editProductCategoryMapping = editProductCategoryMappings.FirstOrDefault(pcm => pcm.CategoryId == editingEditProductCategoryMapping.CategoryId);
                        editingEditProductCategoryMapping.DisplayOrder = editProductCategoryMapping!.DisplayOrder;
                        await _productCategoryMappingRepository.Update(editingEditProductCategoryMapping, false);
                    }
                }
            }
            else if (existingProductCategoryMappings.Any())
            {
                // Remove all assigned categories, but do not save changes
                existingProductCategoryMappings.ForEach(async existingProductCategoryMapping => await _productCategoryMappingRepository.Delete(existingProductCategoryMapping, false));
            }

            // Product specification values
            var existingProductSpecificationValueMappings = _productSpecificationValueMappingRepository.AsQueryable().Where(psvm => psvm.ProductId == productId).ToList() ?? new();
            var existingProductSpecificationValueMappingSpecificationValueIds = existingProductSpecificationValueMappings.Select(psvm => psvm.SpecificationValueId).ToList();
            var editProductSpecificationValueMappingIds = editProductDto.EditSpecificationValueIds ?? new();
            if (editProductSpecificationValueMappingIds != null && editProductSpecificationValueMappingIds.Any())
            {
                // Remove not existed specificatin value ids from list
                var existingSpecificationValueIds = _specificationValueRepository.AsQueryable().AsNoTracking().Select(sv => sv.Id).Where(x => editProductSpecificationValueMappingIds.Contains(x)).ToList();
                editProductSpecificationValueMappingIds = editProductSpecificationValueMappingIds.Where(svId => existingSpecificationValueIds.Contains(svId)).ToList();

                // Add new product specificatin values found
                var addingEditProductSpecificationValueMappingSpecificationValueIds = editProductSpecificationValueMappingIds.Where(psvmId => !existingProductSpecificationValueMappingSpecificationValueIds.Contains(psvmId)).ToList();
                if (addingEditProductSpecificationValueMappingSpecificationValueIds.Any())
                {
                    List<ProductSpecificationValueMapping> addingProductSpecificationValueMappings = new();
                    addingEditProductSpecificationValueMappingSpecificationValueIds.ForEach(addingEditProductSpecificationValueMappingSpecificationValueId => addingProductSpecificationValueMappings.Add(new() { ProductId = productId, SpecificationValueId = addingEditProductSpecificationValueMappingSpecificationValueId }));
                    await _productSpecificationValueMappingRepository.AddRange(addingProductSpecificationValueMappings, false);
                }

                // Remove product specificatin values which are not requested now
                var removingProductSpecificationValueMappings = existingProductSpecificationValueMappings.Where(psvm => !editProductSpecificationValueMappingIds.Contains(psvm.SpecificationValueId)).ToList();
                if (removingProductSpecificationValueMappings.Any())
                {
                    removingProductSpecificationValueMappings.ForEach(async removingProductSpecificationValueMapping => await _productSpecificationValueMappingRepository.Delete(removingProductSpecificationValueMapping, false));
                }
            }
            else if (existingProductSpecificationValueMappings.Any())
            {
                // Remove all assigned categories, but do not save changes
                existingProductSpecificationValueMappings.ForEach(async existingProductSpecificationValueMapping => await _productSpecificationValueMappingRepository.Delete(existingProductSpecificationValueMapping, false));
            }

            // Edit product navigation properties
            await _productCategoryMappingRepository.SaveChangesAsync();
            await _productSpecificationValueMappingRepository.SaveChangesAsync();

            response.HttpStatusCode = HttpStatusCode.OK;
            response.Message = "The product has been modified successfully!";
            return response;
        }
    }
}
