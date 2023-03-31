namespace eCommerceWebAPI.Domain.Catalogs
{
    /// <summary>
    /// Represents a product
    /// </summary>
    public class Product : BaseSoftDeleteSqlEntity
    {
        private ICollection<ProductCategoryMapping> _productCategoryMappings;
        private ICollection<ProductSpecificationValueMapping> _productSpecificationValues;

        /// <summary>
        /// Gets or sets the product name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the product code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the product short description
        /// </summary>
        public string? ShortDescription { get; set; }

        /// <summary>
        /// Gets or sets the product full description
        /// </summary>
        public string? FullDescription { get; set; }

        /// <summary>
        /// Gets or sets the product price
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Gets or sets the product discount amount
        /// </summary>
        public decimal DiscountAmount { get; set; }

        /// <summary>
        /// Gets or sets the product stock quantity
        /// </summary>
        public int StockQuantity { get; set; }
        
        /// <summary>
        /// Gets or sets the product prices currency
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// Gets or sets the collection of ProductCategoryMapping
        /// </summary>
        public virtual ICollection<ProductCategoryMapping> ProductCategoryMappings
        {
            get => _productCategoryMappings ?? (_productCategoryMappings = new List<ProductCategoryMapping>());
            protected set => _productCategoryMappings = value;
        }

        /// <summary>
        /// Gets or sets the collection of ProductSpecificationValueMapping
        /// </summary>
        public virtual ICollection<ProductSpecificationValueMapping> ProductSpecificationValueMappings
        {
            get => _productSpecificationValues ?? (_productSpecificationValues = new List<ProductSpecificationValueMapping>());
            protected set => _productSpecificationValues = value;
        }
    }
}
