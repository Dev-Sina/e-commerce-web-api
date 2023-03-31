namespace eCommerceWebAPI.Domain.Addresses
{
    /// <summary>
    /// Represents a country
    /// </summary>
    public class Country : BaseSqlEntity
    {
        private ICollection<Province> _provinces;

        /// <summary>
        /// Gets or sets the country name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the collection of Province
        /// </summary>
        public virtual ICollection<Province> Provinces
        {
            get => _provinces ?? (_provinces = new List<Province>());
            protected set => _provinces = value;
        }
    }
}
