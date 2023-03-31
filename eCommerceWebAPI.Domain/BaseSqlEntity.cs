namespace eCommerceWebAPI.Domain
{
    /// <summary>
    /// Represents the base sql entity
    /// </summary>
    public class BaseSqlEntity : BaseEntity
    {
        /// <summary>
        /// Gets or sets the entity identifier
        /// </summary>
        public virtual long Id { get; set; }
    }
}
