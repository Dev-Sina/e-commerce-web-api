namespace eCommerceWebAPI.Domain.Catalogs
{
    /// <summary>
    /// Represents a specification value
    /// </summary>
    public class SpecificationValue : BaseSqlEntity
    {
        private ICollection<ProductSpecificationValueMapping> _productSpecificationValues;

        /// <summary>
        /// Gets or sets the specification value name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the specification value display order
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the specification value related specification identifier
        /// </summary>
        public long SpecificationId { get; set; }

        /// <summary>
        /// Gets or sets the related specification
        /// </summary>
        public virtual Specification Specification { get; set; }

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
