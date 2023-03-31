namespace eCommerceWebAPI.Domain.Addresses
{
    /// <summary>
    /// Represents a province
    /// </summary>
    public class Province : BaseSqlEntity
    {
        private ICollection<City> _cities;

        /// <summary>
        /// Gets or sets the province name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the country identifier
        /// </summary>
        public long CountryId { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the country
        /// </summary>
        public virtual Country Country { get; set; }

        /// <summary>
        /// Gets or sets the collection of City
        /// </summary>
        public virtual ICollection<City> Cities
        {
            get => _cities ?? (_cities = new List<City>());
            protected set => _cities = value;
        }
    }
}
