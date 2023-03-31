using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting.Internal;

namespace eCommerceWebAPI.IntegrationTest.InfraStructure
{
    public static class ApiWebApplicationFactoryExtensions
    {
        public static WebApplicationFactory<TStartup> WithAppSettings<TStartup>(
            this WebApplicationFactory<TStartup> factory,
            IEnumerable<KeyValuePair<string, string>> customSettings = null)
            where TStartup : class
        {
            return factory.WithWebHostBuilder(x =>
            {
                var projDir = Directory.GetCurrentDirectory();
                var configpath = Path.Combine(projDir, "appsettings.json");
                x.ConfigureServices(c => c.AddSingleton(new HostingEnvironment { EnvironmentName = "Test" }));
                x.ConfigureAppConfiguration((c, d) => d.AddJsonFile(configpath));
                if (customSettings != null)
                    x.ConfigureAppConfiguration((c, d) => d.AddInMemoryCollection(customSettings));
            });
        }

        public static WebApplicationFactory<TStartup> Seed<TStartup, TDbContext>(
            this WebApplicationFactory<TStartup> factory,
            Action<TDbContext> seed)
            where TStartup : class
            where TDbContext : DbContext
        {
            using (var scope = factory.Server.Services.CreateScope())
            {
                var service = scope.ServiceProvider.GetService<TDbContext>();
                seed(service);
                service.SaveChanges();
            }

            return factory;
        }

        public static TService GetScopedService<TStartup, TService>(
            this WebApplicationFactory<TStartup> factory)
            where TStartup : class
        {
            return factory.Server.Services.CreateScope().ServiceProvider.GetService<TService>();
        }
    }
}
