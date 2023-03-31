using eCommerceWebAPI.Application.Catalogs.Categories;

namespace eCommerceWebAPI.Application.Catalogs.Products
{
    public class ProductCategoryMappingDto
    {
        public long Id { get; set; }

        public long ProductId { get; set; }

        public long CategoryId { get; set; }

        public int DisplayOrder { get; set; }

        public virtual ProductDto Product { get; set; } = new();

        public virtual CategoryDto Category { get; set; } = new();
    }
}
