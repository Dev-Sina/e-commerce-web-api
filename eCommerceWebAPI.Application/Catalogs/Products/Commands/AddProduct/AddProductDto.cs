namespace eCommerceWebAPI.Application.Catalogs.Products.Commands.AddProduct
{
    public class AddProductDto
    {
        public string Name { get; set; } = string.Empty;

        public string Code { get; set; } = string.Empty;

        public string? ShortDescription { get; set; }

        public string? FullDescription { get; set; }

        public decimal Price { get; set; }

        public decimal? DiscountPercent { get; set; }

        public decimal DiscountAmount { get; set; }

        public int StockQuantity { get; set; }

        public virtual List<AddProductCategoryMappingDto> AddProductCategoryMappings { get; set; } = new();

        public virtual List<long> AddSpecificationValueIds { get; set; } = new();
    }
}
