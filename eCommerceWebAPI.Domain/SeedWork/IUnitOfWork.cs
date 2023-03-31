using Microsoft.EntityFrameworkCore;

namespace eCommerceWebAPI.Domain.SeedWork
{
    public interface IUnitOfWork
    {
        Task<int> CommitAsync(CancellationToken cancellationToken = default);
        public void StartTransaction();
        public void CommitTransaction();
        public void RollbackTransaction();
        DbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity;
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
