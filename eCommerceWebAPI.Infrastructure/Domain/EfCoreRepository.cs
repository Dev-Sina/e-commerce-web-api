using eCommerceWebAPI.Domain;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace eCommerceWebAPI.Infrastructure.Domain
{
    public abstract class EfCoreRepository<TEntity, TContext> : IEfCoreRepository<TEntity> where TEntity : BaseEntity where TContext : DbContext
    {
        private readonly bool defaultSaveChanges = true;

        private readonly TContext _context;

        protected EfCoreRepository(TContext context)
        {
            _context = context;
        }

        public IQueryable<TEntity> Search(Expression<Func<TEntity, bool>> condition)
        {
            return _context.Set<TEntity>().Where(condition);
        }

        public IQueryable<TEntity> AsQueryable()
        {
            return _context.Set<TEntity>().AsQueryable();
        }

        public IQueryable<TEntity> Search(Expression<Func<TEntity, int, bool>> condition)
        {
            return _context.Set<TEntity>().Where(condition);
        }

        public async Task<TEntity> Add(TEntity entity) => await Add(entity, defaultSaveChanges);
        public async Task<TEntity> Add(TEntity entity, bool saveChanges = true)
        {
            await _context.Set<TEntity>().AddAsync(entity);
            await SaveChangesAsync(saveChanges);
            return entity;
        }

        public async Task<List<TEntity>> AddRange(List<TEntity> entity) => await AddRange(entity, defaultSaveChanges);
        public async Task<List<TEntity>> AddRange(List<TEntity> entity, bool saveChanges)
        {
            await _context.Set<TEntity>().AddRangeAsync(entity);
            await SaveChangesAsync(saveChanges);
            return entity;
        }

        public async Task<TEntity> Delete(int id) => await Delete(id, defaultSaveChanges);
        public async Task<TEntity> Delete(int id, bool saveChanges)
        {
            var entity = await _context.Set<TEntity>().FindAsync(id);
            if (entity == null)
            {
                return null;
            }

            _context.Set<TEntity>().Remove(entity);
            await SaveChangesAsync(saveChanges);

            return entity;
        }

        public async Task<bool> DeleteRange(List<TEntity> delEntity)
        {
            try
            {
                _context.Set<TEntity>().RemoveRange(delEntity);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception e)
            {
                throw new Exception("An exception while DeleteRange has been occured", e);
            }
        }

        public async Task<TEntity> Delete(Guid id) => await Delete(id, defaultSaveChanges);
        public async Task<TEntity> Delete(Guid id, bool saveChanges)
        {
            var entity = await _context.Set<TEntity>().FindAsync(id);
            if (entity == null)
            {
                return null;
            }

            _context.Set<TEntity>().Remove(entity);
            await SaveChangesAsync(saveChanges);
            return entity;
        }

        public async Task<TEntity> Delete(long id) => await Delete(id, defaultSaveChanges);
        public async Task<TEntity> Delete(long id, bool saveChanges)
        {
            var entity = await _context.Set<TEntity>().FindAsync(id);
            if (entity == null)
            {
                return null;
            }

            _context.Set<TEntity>().Remove(entity);
            await SaveChangesAsync(saveChanges);

            return entity;
        }

        public async Task<EntityEntry<TEntity>> Delete(TEntity delEntity) => await Delete(delEntity, defaultSaveChanges);
        public async Task<EntityEntry<TEntity>> Delete(TEntity delEntity, bool saveChanges)
        {
            if (delEntity == null)
            {
                return null;
            }

            var entity = _context.Set<TEntity>().Remove(delEntity);
            if (entity == null)
            {
                return null;
            }

            await SaveChangesAsync(saveChanges);
            return entity;
        }

        public async Task<TEntity> Get(long id)
        {
            return await _context.Set<TEntity>().FindAsync(id);
        }

        public async Task<TEntity> Get(int id)
        {
            return await _context.Set<TEntity>().FindAsync(id);
        }

        public int ExecuteRawSql(string sql)
        {
            return _context.Database.ExecuteSqlRaw(sql);
        }

        public async Task<List<TEntity>> GetAll()
        {
            return await _context.Set<TEntity>().ToListAsync();
        }

        public async Task<TEntity> Update(TEntity entity) => await Update(entity, defaultSaveChanges);
        public async Task<TEntity> Update(TEntity entity, bool saveChanges)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await SaveChangesAsync(saveChanges);
            return entity;
        }

        public async Task<List<TEntity>> UpdateRange(List<TEntity> entities) => await UpdateRange(entities, defaultSaveChanges);
        public async Task<List<TEntity>> UpdateRange(List<TEntity> entities, bool saveChanges)
        {
            _context.Set<TEntity>().UpdateRange(entities);
            await SaveChangesAsync(saveChanges);
            return entities;
        }

        public async Task<long> Count()
        {
            return await _context.Set<TEntity>().CountAsync();
        }

        public async Task<TEntity> AddOrUpdate(TEntity entity) => await AddOrUpdate(entity, defaultSaveChanges);
        public async Task<TEntity> AddOrUpdate(TEntity entity, bool saveChanges)
        {
            _context.Set<TEntity>().Update(entity);
            await SaveChangesAsync(saveChanges);
            return entity;
        }

        public async Task<List<TEntity>> AddOrUpdateRange(List<TEntity> entities) => await AddOrUpdateRange(entities, defaultSaveChanges);
        public async Task<List<TEntity>> AddOrUpdateRange(List<TEntity> entities, bool saveChanges)
        {
            _context.Set<TEntity>().UpdateRange(entities);
            await SaveChangesAsync(saveChanges);
            return entities;
        }

        public async Task SaveChangesAsync() => await SaveChangesAsync(defaultSaveChanges);
        public async Task SaveChangesAsync(bool saveChanges)
        {
            if (saveChanges)
            {
                await _context.SaveChangesAsync();
            }
        }
    }
}
