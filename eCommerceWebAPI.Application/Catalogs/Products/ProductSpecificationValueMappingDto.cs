using eCommerceWebAPI.Application.Catalogs.Specifications;

namespace eCommerceWebAPI.Application.Catalogs.Products
{
    public class ProductSpecificationValueMappingDto
    {
        public long Id { get; set; }

        public long ProductId { get; set; }

        public long SpecificationValueId { get; set; }

        public virtual ProductDto Product { get; set; } = new();

        public virtual SpecificationValueDto SpecificationValue { get; set; } = new();
    }
}
