namespace eCommerceWebAPI.Domain.Catalogs
{
    /// <summary>
    /// Represents a product category mapping
    /// </summary>
    public class ProductCategoryMapping : BaseSqlEntity
    {
        /// <summary>
        /// Gets or sets the product identifier
        /// </summary>
        public long ProductId { get; set; }

        /// <summary>
        /// Gets or sets the category identifier
        /// </summary>
        public long CategoryId { get; set; }

        /// <summary>
        /// Gets or sets the product category mapping display order
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the product
        /// </summary>
        public virtual Product Product { get; set; }

        /// <summary>
        /// Gets or sets the category
        /// </summary>
        public virtual Category Category { get; set; }
    }
}
