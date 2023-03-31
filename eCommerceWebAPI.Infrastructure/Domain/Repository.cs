using eCommerceWebAPI.Domain;
using eCommerceWebAPI.Infrastructure.Databases;

namespace eCommerceWebAPI.Infrastructure.Domain
{
    public partial class Repository<TEntity> : EfCoreRepository<TEntity, CommonDataContext>, IRepository<TEntity> where TEntity : BaseEntity
    {
        private readonly CommonDataContext _context;

        public Repository(CommonDataContext context) : base(context)
        {
            _context = context;
        }
    }
}
