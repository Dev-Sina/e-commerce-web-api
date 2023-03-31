namespace eCommerceWebAPI.Domain.Catalogs
{
    /// <summary>
    /// Represents a product specification value mapping
    /// </summary>
    public class ProductSpecificationValueMapping : BaseSqlEntity
    {
        /// <summary>
        /// Gets or sets the product identifier
        /// </summary>
        public long ProductId { get; set; }

        /// <summary>
        /// Gets or sets the specification value identifier
        /// </summary>
        public long SpecificationValueId { get; set; }

        /// <summary>
        /// Gets or sets the product
        /// </summary>
        public virtual Product Product { get; set; }

        /// <summary>
        /// Gets or sets the specification value
        /// </summary>
        public virtual SpecificationValue SpecificationValue { get; set; }

    }
}
