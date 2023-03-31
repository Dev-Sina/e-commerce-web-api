namespace eCommerceWebAPI.Domain.Catalogs
{
    /// <summary>
    /// Represents a specification
    /// </summary>
    public class Specification : BaseSqlEntity
    {
        private ICollection<SpecificationValue> _specificationValues;

        /// <summary>
        /// Gets or sets the specification name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the specification display order
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the collection of SpecificationValue
        /// </summary>
        public virtual ICollection<SpecificationValue> SpecificationValues
        {
            get => _specificationValues ?? (_specificationValues = new List<SpecificationValue>());
            protected set => _specificationValues = value;
        }
    }
}
