namespace eCommerceWebAPI.Domain.Addresses
{
    /// <summary>
    /// Represents a city
    /// </summary>
    public class City : BaseSqlEntity
    {
        /// <summary>
        /// Gets or sets the city name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the province identifier
        /// </summary>
        public long ProvinceId { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the province
        /// </summary>
        public virtual Province Province { get; set; }
    }
}
