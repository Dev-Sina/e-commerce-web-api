using Autofac;
using eCommerceWebAPI.Application.Configuration;
using eCommerceWebAPI.Domain;
using eCommerceWebAPI.Domain.SeedWork;
using eCommerceWebAPI.Infrastructure.Domain;
using Microsoft.EntityFrameworkCore;

namespace eCommerceWebAPI.Infrastructure.Databases
{
    public class DataAccessModule : Module
    {
        private readonly string _databaseConnectionString;

        public DataAccessModule(string databaseConnectionString)
        {
            _databaseConnectionString = databaseConnectionString;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<SqlConnectionFactory>()
                .As<ISqlConnectionFactory>()
                .WithParameter("connectionString", _databaseConnectionString)
                .InstancePerLifetimeScope();

            builder.RegisterType<UnitOfWork>()
                .As<IUnitOfWork>()
                .InstancePerLifetimeScope();

            builder
                .Register(c =>
                {
                    var dbContextOptionsBuilder = new DbContextOptionsBuilder<CommonDataContext>();
                    dbContextOptionsBuilder.UseSqlServer(_databaseConnectionString);
                    //return new CommonDataContext(dbContextOptionsBuilder.Options);
                    return new CommonDataContext(_databaseConnectionString);
                })
                .AsSelf()
                .As<DbContext>()
                .InstancePerLifetimeScope();

            builder.RegisterGeneric(typeof(EfCoreRepository<,>))
                .As(typeof(IEfCoreRepository<>))
                .InstancePerLifetimeScope();

            builder.RegisterGeneric(typeof(Repository<>))
                .As(typeof(IRepository<>))
                .InstancePerLifetimeScope();

            //builder.RegisterGeneric(typeof(ShoppingCartRepository))
            //    .As(typeof(IShoppingCartRepository))
            //    .InstancePerLifetimeScope();
        }
    }
}
