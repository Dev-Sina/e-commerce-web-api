using eCommerceWebAPI.Domain;
using eCommerceWebAPI.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;

namespace eCommerceWebAPI.Infrastructure.Databases
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CommonDataContext _commonDataContext;

        public UnitOfWork(CommonDataContext commonDataContext)
        {
            _commonDataContext = commonDataContext;
        }

        public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
        {
            return await _commonDataContext.SaveChangesAsync(cancellationToken);
        }

        public void StartTransaction()
        {
            _commonDataContext.Database.BeginTransaction();
        }

        public void CommitTransaction()
        {
            _commonDataContext.Database.CommitTransaction();
        }

        public void RollbackTransaction()
        {
            _commonDataContext.Database.RollbackTransaction();
        }

        public DbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity
        {
            return _commonDataContext.Set<TEntity>();
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _commonDataContext.SaveChangesAsync(cancellationToken);
        }
    }
}
