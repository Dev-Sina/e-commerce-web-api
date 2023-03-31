using eCommerceWebAPI.Application.Catalogs.Products;

namespace eCommerceWebAPI.Application.Catalogs.Specifications
{
    public partial class SpecificationValueDto
    {
        public long Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int DisplayOrder { get; set; }

        public long SpecificationId { get; set; }

        public virtual SpecificationDto Specification { get; set; } = new();

        public virtual List<ProductSpecificationValueMappingDto> ProductSpecificationValueMappings { get; set; } = new();
    }
}
