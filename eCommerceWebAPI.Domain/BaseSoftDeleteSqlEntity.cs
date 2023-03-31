namespace eCommerceWebAPI.Domain
{
    /// <summary>
    /// Represents the base soft delete sql entity
    /// </summary>
    public class BaseSoftDeleteSqlEntity : BaseSqlEntity, ISoftDelete
    {
        /// <summary>
        /// Gets or sets the entity soft deleted property
        /// </summary>
        public virtual bool Deleted { get; set; } = false;
    }
}
