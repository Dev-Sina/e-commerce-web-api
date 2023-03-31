namespace eCommerceWebAPI.Domain.Catalogs
{
    /// <summary>
    /// Represents a category
    /// </summary>
    public class Category : BaseSqlEntity
    {
        private ICollection<Category> _childCategories;
        private ICollection<ProductCategoryMapping> _productCategoryMappings;

        /// <summary>
        /// Gets or sets the category name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the category description
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the category display order
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the parent category identifier
        /// </summary>
        public long? ParentCategoryId { get; set; }

        /// <summary>
        /// Gets or sets the parent category
        /// </summary>
        public Category? ParentCategory { get; set; }

        /// <summary>
        /// Gets or sets the collection of child Category
        /// </summary>
        public virtual ICollection<Category> ChildCategories
        {
            get => _childCategories ?? (_childCategories = new List<Category>());
            protected set => _childCategories = value;
        }

        /// <summary>
        /// Gets or sets the collection of ProductCategoryMapping
        /// </summary>
        public virtual ICollection<ProductCategoryMapping> ProductCategoryMappings
        {
            get => _productCategoryMappings ?? (_productCategoryMappings = new List<ProductCategoryMapping>());
            protected set => _productCategoryMappings = value;
        }
    }
}
