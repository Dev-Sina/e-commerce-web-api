using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace eCommerceWebAPI.Infrastructure.Databases
{
    //public class CommonDataContextFactory : IDesignTimeDbContextFactory<CommonDataContext>
    //{
    //    public CommonDataContext CreateDbContext(string[] args)
    //    {
    //        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

    //        IConfigurationRoot configuration = new ConfigurationBuilder()
    //            .SetBasePath(Directory.GetCurrentDirectory())
    //            .AddJsonFile($"appsettings.{environment}.json")
    //            .Build();
            
    //        // Here we create the DbContextOptionsBuilder manually.        
    //        var builder = new DbContextOptionsBuilder<CommonDataContext>();

    //        // Build connection string. This requires that you have a connectionstring in the appsettings.environment.json
    //        var connectionString = configuration.GetConnectionString("DefaultConnection");
    //        builder.UseSqlServer(connectionString);
        
    //        // Create our DbContext.
    //        return new CommonDataContext(connectionString);
    //    }

    //}
}
