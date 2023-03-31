namespace eCommerceWebAPI.Application.Catalogs.Products
{
    public partial class ProductDto
    {
        public long Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Code { get; set; } = string.Empty;

        public string? ShortDescription { get; set; }

        public string? FullDescription { get; set; }

        public decimal Price { get; set; }
        public string PriceNormalized => Price.ToString("N0") + " " + Currency;

        public decimal DiscountPercent => (DiscountAmount / Price) * 100;
        public string DiscountPerentNormalized => $"%{DiscountPercent}";
        public decimal DiscountAmount { get; set; }
        public string DiscountAmountNormalized => DiscountAmount.ToString("N0") + " " + Currency;

        public int StockQuantity { get; set; }
        public bool IsInStock => StockQuantity > 0;

        public string Currency { get; set; } = string.Empty;

        public bool Deleted { get; set; } = false;

        public virtual List<ProductCategoryMappingDto> ProductCategoryMappings { get; set; } = new();

        public virtual List<ProductSpecificationValueMappingDto> ProductSpecificationValueMappings { get; set; } = new();
    }
}
