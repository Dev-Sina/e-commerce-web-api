namespace eCommerceWebAPI.Domain
{
    public interface IRepository<TEntity> : IEfCoreRepository<TEntity> where TEntity : BaseEntity
    {
    }
}
