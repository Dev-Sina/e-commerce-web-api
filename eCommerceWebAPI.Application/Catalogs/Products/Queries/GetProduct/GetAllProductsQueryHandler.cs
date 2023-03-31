using AutoMapper;
using eCommerceWebAPI.Application.Configuration;
using eCommerceWebAPI.Application.Configuration.Queries;
using Dapper;
using eCommerceWebAPI.Domain.Catalogs;

namespace eCommerceWebAPI.Application.Catalogs.Products.Queries.GetProduct
{
    public class GetAllProductsQueryHandler : IQueryHandler<GetAllProductsQuery, IList<ProductDto>>
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;
        private readonly IMapper _mapper;

        public GetAllProductsQueryHandler(ISqlConnectionFactory sqlConnectionFactory,
            IMapper mapper)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
            _mapper = mapper;
        }

        public async Task<IList<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));

            try
            {
                var connection = _sqlConnectionFactory.GetOpenConnection();

                const string sqlProducts = $@"
                                    SELECT
                                       [Product].[Id],
                                       [Product].[Name],
                                       [Product].[Code],
                                       [Product].[ShortDescription],
                                       [Product].[FullDescription],
                                       [Product].[Price],
                                       [Product].[DiscountAmount],
                                       [Product].[StockQuantity],
                                       [Product].[Currency]
                                   FROM [Product]
                                   WHERE [Product].Deleted = 0
                                   ORDER BY [Product].Id
                                   OFFSET @Skip ROWS
                                   FETCH NEXT @PageSize ROWS ONLY";
                var productsEnumerable = await connection.QueryAsync<Product>(sqlProducts, new { request.Skip, request.PageSize });
                var productsList = productsEnumerable.ToList();

                string sqlProductCategoryMappings = $@"
                                    SELECT DISTINCT
                                       [ProductCategoryMapping].[Id],
                                       [ProductCategoryMapping].[ProductId],
                                       [ProductCategoryMapping].[CategoryId],
                                       [ProductCategoryMapping].[DisplayOrder]
                                   FROM [ProductCategoryMapping]
                                   WHERE [ProductCategoryMapping].ProductId IN (" + (productsList.Any() ? string.Join(",", productsList.Select(p => p.Id)) : 0.ToString()) + ")";
                var productCategoryMappingsEnumerable = await connection.QueryAsync<ProductCategoryMapping>(sqlProductCategoryMappings);
                var productCategoryMappingsList = productCategoryMappingsEnumerable.ToList();

                string sqlCategories = $@"
                                    SELECT DISTINCT
                                       [Category].[Id],
                                       [Category].[Name],
                                       [Category].[Description],
                                       [Category].[DisplayOrder],
                                       [Category].[ParentCategoryId]
                                   FROM [Category]
                                   WHERE [Category].Id IN (" + (productCategoryMappingsList.Any() ? string.Join(",", productCategoryMappingsList.Select(pcm => pcm.CategoryId)) : 0.ToString()) + ")";
                var categoriesEnumerable = await connection.QueryAsync<Category>(sqlCategories);
                var categoriesList = categoriesEnumerable.ToList();

                string sqlProductSpecificationValueMappings = $@"
                                    SELECT DISTINCT
                                       [ProductSpecificationValueMapping].[Id],
                                       [ProductSpecificationValueMapping].[ProductId],
                                       [ProductSpecificationValueMapping].[SpecificationValueId]
                                   FROM [ProductSpecificationValueMapping]
                                   WHERE [ProductSpecificationValueMapping].ProductId IN (" + (productsList.Any() ? string.Join(",", productsList.Select(p => p.Id)) : 0.ToString()) + ")";
                var productSpecificationValueMappingsEnumerable = await connection.QueryAsync<ProductSpecificationValueMapping>(sqlProductSpecificationValueMappings);
                var productSpecificationValueMappingsList = productSpecificationValueMappingsEnumerable.ToList();

                string sqlSpecificationValues = $@"
                                    SELECT DISTINCT
                                       [SpecificationValue].[Id],
                                       [SpecificationValue].[Name],
                                       [SpecificationValue].[DisplayOrder],
                                       [SpecificationValue].[SpecificationId]
                                   FROM [SpecificationValue]
                                   WHERE [SpecificationValue].Id IN (" + (productSpecificationValueMappingsList.Any() ? string.Join(",", productSpecificationValueMappingsList.Select(psvm => psvm.SpecificationValueId)) : 0.ToString()) + ")";
                var specificationValuesEnumerable = await connection.QueryAsync<SpecificationValue>(sqlSpecificationValues);
                var specificationValuesList = specificationValuesEnumerable.ToList();

                string sqlSpecifications = $@"
                                    SELECT DISTINCT
                                       [Specification].[Id],
                                       [Specification].[Name],
                                       [Specification].[DisplayOrder]
                                   FROM [Specification]
                                   WHERE [Specification].Id IN (" + (specificationValuesList.Any() ? string.Join(",", specificationValuesList.Select(sv => sv.SpecificationId)) : 0.ToString()) + ")";
                var specificationsEnumerable = await connection.QueryAsync<Specification>(sqlSpecifications);
                var specificationsList = specificationsEnumerable.ToList();

                productsList.ForEach(p =>
                {
                    List<ProductCategoryMapping> productCategoryMappings = new();
                    var relatedProductCategoryMappings = productCategoryMappingsList.Where(pcm => pcm.ProductId == p.Id).ToList();
                    var relatedCategories = categoriesList.Where(c => relatedProductCategoryMappings.Select(pcm => pcm.CategoryId).Contains(c.Id)).ToList();
                    relatedProductCategoryMappings.ForEach(pcm =>
                    {
                        var relatedCategory = relatedCategories.FirstOrDefault(c => pcm.CategoryId == c.Id);
                        if (relatedCategory != null)
                        {
                            productCategoryMappings.Add(new ProductCategoryMapping { Id = pcm.Id, ProductId = p.Id, CategoryId = relatedCategory.Id, DisplayOrder = pcm.DisplayOrder, Category = relatedCategory });
                        }
                    });
                    productCategoryMappings = productCategoryMappings.OrderBy(pcm => pcm.DisplayOrder).ToList();
                    productCategoryMappings.ForEach(p.ProductCategoryMappings.Add);

                    var relatedProductSpecificationValueMappings = productSpecificationValueMappingsList.Where(psvm => psvm.ProductId == p.Id).ToList();
                    var relatedSpecificationValues = specificationValuesList.Where(sv => relatedProductSpecificationValueMappings.Select(psvm => psvm.SpecificationValueId).Contains(sv.Id)).ToList();
                    relatedProductSpecificationValueMappings.ForEach(psvm =>
                    {
                        var relatedSpecificationValue = specificationValuesList.FirstOrDefault(sv => psvm.SpecificationValueId == sv.Id);
                        if (relatedSpecificationValue != null)
                        {
                            var relatedSpecification = specificationsList.FirstOrDefault(s => s.Id == relatedSpecificationValue.SpecificationId);
                            if (relatedSpecification != null)
                            {
                                relatedSpecificationValue.Specification = relatedSpecification;
                                p.ProductSpecificationValueMappings.Add(new ProductSpecificationValueMapping { Id = psvm.Id, ProductId = p.Id, SpecificationValueId = relatedSpecificationValue.Id, SpecificationValue = relatedSpecificationValue });
                            }
                        }
                    });
                });

                var productDtos = _mapper.Map<List<ProductDto>>(productsList);
                return productDtos;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }
}
