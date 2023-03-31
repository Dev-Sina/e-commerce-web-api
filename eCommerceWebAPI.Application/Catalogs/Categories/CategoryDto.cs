using eCommerceWebAPI.Application.Catalogs.Products;

namespace eCommerceWebAPI.Application.Catalogs.Categories
{
    public partial class CategoryDto
    {
        public long Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int DisplayOrder { get; set; }

        public long? ParentCategoryId { get; set; }

        public CategoryDto? ParentCategory { get; set; }

        public virtual List<CategoryDto> ChildCategories { get; set; } = new();

        public virtual List<ProductCategoryMappingDto> ProductCategoryMappings { get; set; } = new();
    }
}
