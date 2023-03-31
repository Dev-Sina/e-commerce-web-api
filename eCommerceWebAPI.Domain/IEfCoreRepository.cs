using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace eCommerceWebAPI.Domain
{
    public interface IEfCoreRepository<TEntity> where TEntity : BaseEntity
    {
        IQueryable<TEntity> Search(Expression<Func<TEntity, bool>> condition);

        IQueryable<TEntity> AsQueryable();

        IQueryable<TEntity> Search(Expression<Func<TEntity, int, bool>> condition);

        Task<TEntity> Add(TEntity entity);
        Task<TEntity> Add(TEntity entity, bool saveChanges);

        Task<List<TEntity>> AddRange(List<TEntity> entity);
        Task<List<TEntity>> AddRange(List<TEntity> entity, bool saveChanges);

        Task<TEntity> Delete(int id);
        Task<TEntity> Delete(int id, bool saveChanges);

        Task<bool> DeleteRange(List<TEntity> delEntity);

        Task<TEntity> Delete(Guid id);
        Task<TEntity> Delete(Guid id, bool saveChanges);

        Task<TEntity> Delete(long id);
        Task<TEntity> Delete(long id, bool saveChanges);

        Task<EntityEntry<TEntity>> Delete(TEntity delEntity);
        Task<EntityEntry<TEntity>> Delete(TEntity delEntity, bool saveChanges);

        Task<TEntity> Get(long id);

        Task<TEntity> Get(int id);

        int ExecuteRawSql(string sql);

        Task<List<TEntity>> GetAll();

        Task<TEntity> Update(TEntity entity);
        Task<TEntity> Update(TEntity entity, bool saveChanges);

        Task<List<TEntity>> UpdateRange(List<TEntity> entities);
        Task<List<TEntity>> UpdateRange(List<TEntity> entities, bool saveChanges);

        Task<long> Count();

        Task<TEntity> AddOrUpdate(TEntity entity);
        Task<TEntity> AddOrUpdate(TEntity entity, bool saveChanges);

        Task<List<TEntity>> AddOrUpdateRange(List<TEntity> entities);
        Task<List<TEntity>> AddOrUpdateRange(List<TEntity> entities, bool saveChanges);

        Task SaveChangesAsync();
        Task SaveChangesAsync(bool saveChanges);
    }
}
