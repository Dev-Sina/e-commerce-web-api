using DNTCommon.Web.Core;
using eCommerceWebAPI.Domain.SeedWork;
using eCommerceWebAPI.Infrastructure.Databases;
using Microsoft.AspNetCore;
using Microsoft.EntityFrameworkCore;

namespace eCommerceWebAPI.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var hostBuilder = CreateWebHostBuilder(args);
            var host = hostBuilder.Build();

            // DB Migration
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var db = services.GetRequiredService<CommonDataContext>();

                var m = db.Database.GetMigrations();
                var applied = db.Database.GetAppliedMigrations();
                var pending = db.Database.GetPendingMigrations();

                db.Database.Migrate();
            }

            // First run Db data
            host.Services.RunScopedService<IDbInitializer>(iDbInitializer =>
            {
                iDbInitializer.SeedData();
            });

            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost
                .CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.Sources.Clear();
                    config.AddEnvironmentVariables();
                    var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                    var setting = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
                    var envSetting = Path.Combine(AppContext.BaseDirectory, $"appsettings.{environment}.json");
                    config.AddJsonFile(setting, true, true).AddJsonFile(envSetting, true, true);
                    if (args != null && args.Any())
                    {
                        config.AddCommandLine(args);
                    }
                })
                .UseKestrel()
                .UseWebRoot("Static")
                .UseStartup<Startup>();
        }
    }
}