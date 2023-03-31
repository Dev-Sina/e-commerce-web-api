using eCommerceWebAPI.Infrastructure.Databases;
using Microsoft.EntityFrameworkCore;

namespace eCommerceWebAPI.IntegrationTest.InfraStructure
{
    public static class ApiDbContextExtensions
    {
        public static void Clear(this CommonDataContext db)
        {
            db.SaveChanges();
        }

        public static void Clear<T>(this DbSet<T> dbSet) where T : class
        {
            dbSet.RemoveRange(dbSet);
        }
    }
}
