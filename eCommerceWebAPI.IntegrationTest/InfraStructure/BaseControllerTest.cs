using eCommerceWebAPI.Api;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using eCommerceWebAPI.Infrastructure.Databases;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Autofac.Extensions.DependencyInjection;
using Autofac;
using eCommerceWebAPI.Domain;
using eCommerceWebAPI.Infrastructure.Domain;
using eCommerceWebAPI.Infrastructure.Processing;
using StackExchange.Redis;
using CommonServiceLocator;
using Autofac.Extras.CommonServiceLocator;
using Microsoft.Data.SqlClient;
using System.Data;
using Dapper;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore;
using System.Reflection;

namespace eCommerceWebAPI.IntegrationTest.InfraStructure
{
    public class BaseControllerTest
    {
        protected internal WebApplicationFactory<Startup> Factory;

        private readonly string ConnectionStringForSqlServer = "Data Source=.;Initial Catalog=sinadbtest;Integrated Security=False;TrustServerCertificate=True;User ID=sa;Password=123";
        private readonly string ConnectionStringForRedis = "localhost:6379";
        private readonly string RabbitMQHostName = "localhost";
        private readonly string RabbitMQUserName = "guest";
        private readonly string RabbitMQPassword = "guest";

        public static bool databaseCreated;

        public HttpClient Client { get; set; }
        public CommonDataContext Database { get; set; }

        public string BaseRoute { get; set; }

        [OneTimeSetUp]
        public void Setup()
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Test");
            Environment.SetEnvironmentVariable("LD_DEBUG", "true");

            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var setting = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
            var envSetting = Path.Combine(AppContext.BaseDirectory, $"appsettings.{environment}.json");

            Factory = new WebApplicationFactory<Startup>()
                .WithWebHostBuilder(builder =>
                {
                    builder.UseEnvironment("ASPNETCORE_ENVIRONMENT");
                    builder.UseEnvironment("LD_DEBUG");
                    builder.ConfigureServices(services =>
                    {
                        builder.ConfigureAppConfiguration(configurationBuilder =>
                        {
                            configurationBuilder.AddJsonFile(setting, true, false);
                            configurationBuilder.AddJsonFile(envSetting, true, false);
                        });
                        builder.UseKestrel();
                        services.RemoveAll(typeof(DbContextOptions));
                        services.RemoveAll(typeof(CommonDataContext));

                        services.AddDbContext<CommonDataContext>(options =>
                            {
                                options.EnableSensitiveDataLogging();
                                options.UseSqlServer(ConnectionStringForSqlServer);
                            });

                        var sp = services.BuildServiceProvider();
                        using (var scope = sp.CreateScope())
                        {
                            var scopedServices = scope.ServiceProvider;
                            var db = scopedServices.GetRequiredService<CommonDataContext>();
                        }

                        var container = new ContainerBuilder();

                        container
                            .Register(c =>
                            {
                                var dbContextOptionsBuilder = new DbContextOptionsBuilder<CommonDataContext>();
                                dbContextOptionsBuilder.UseSqlServer(ConnectionStringForSqlServer);

                                return new CommonDataContext(ConnectionStringForSqlServer!);
                            })
                            .AsSelf()
                            .InstancePerLifetimeScope();

                        services.AddSingleton<IShoppingCartRepository>(sp =>
                        {
                            var redis = ConnectionMultiplexer.Connect(ConnectionStringForRedis ?? string.Empty);
                            return new ShoppingCartRepository(redis, ConnectionStringForRedis ?? string.Empty);
                        });

                        container.Populate(services);

                        container.RegisterModule(new DataAccessModule(ConnectionStringForSqlServer!));
                        container.RegisterModule(new MediatorModule());
                        container.RegisterModule(new MessageBrokerModule(RabbitMQHostName, RabbitMQUserName, RabbitMQPassword));

                        var buildContainer = container.Build();

                        ServiceLocator.SetLocatorProvider(() => new AutofacServiceLocator(buildContainer));

                        var autofacServiceProvider = new AutofacServiceProvider(buildContainer);

                        buildContainer.BeginLifetimeScope();

                        // DB Migration
                        var webHostBuilder = WebHost.CreateDefaultBuilder();
                        var webHostBuilderType = webHostBuilder.GetType();
                        webHostBuilder.UseStartup<Startup>();
                        var buildMethod = webHostBuilderType.GetMethod("Build", BindingFlags.Instance | BindingFlags.Public);
                        var webHost = (IWebHost)buildMethod.Invoke(webHostBuilder, null);
                        using (var scope = webHost.Services.CreateScope())
                        {
                            var serviceProvider = scope.ServiceProvider;

                            var db = serviceProvider.GetRequiredService<CommonDataContext>();

                            db.Database.Migrate();
                        }
                    });
                });

            Client = Factory.CreateClient();
            Database = Factory.Services.GetService<CommonDataContext>();
        }

        [SetUp]
        public async Task BeforeEachTest()
        {
            Database = Factory.Services.GetService<CommonDataContext>();
            var shoppingCartRepository = Factory.Services.GetService<IShoppingCartRepository>();
            await using var sqlConnection = new SqlConnection(ConnectionStringForSqlServer);
            await ClearDatabase(sqlConnection, shoppingCartRepository);
        }

        [TearDown]
        public async Task AfterEachTest()
        {
        }

        private static async Task ClearDatabase(IDbConnection connection, IShoppingCartRepository shoppingCartRepository)
        {
            // Clear all data in sql server
            var sql = "EXEC sys.sp_msforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL' " +
                      "DELETE FROM Country " +
                      "DELETE FROM Province " +
                      "DELETE FROM City " +
                      "DELETE FROM InvoiceAddress " +
                      "DELETE FROM InvoiceItem " +
                      "DELETE FROM CustomerAddressMapping " +
                      "DELETE FROM Invoice " +
                      "DELETE FROM OrderItem " +
                      "DELETE FROM [Order] " +
                      "DELETE FROM [Address] " +
                      "DELETE FROM Customer " +
                      "DELETE FROM ProductSpecificationValueMapping " +
                      "DELETE FROM SpecificationValue " +
                      "DELETE FROM Specification " +
                      "DELETE FROM ProductCategoryMapping " +
                      "DELETE FROM Category " +
                      "DELETE FROM Product " +
                      "EXEC sys.sp_MSForEachTable 'ALTER TABLE ? CHECK CONSTRAINT ALL' ";
            var result = connection.ExecuteScalarAsync(sql).Result;

            // Clear all data in shopping cart test redis
            await shoppingCartRepository.FlushAllShoppingCartsAsync();
        }

        public string GetBaseRoute(string endPoint = null)
        {
            string route = endPoint == null ? $"api/{BaseRoute}" : $"api/{BaseRoute}/{endPoint}";
            return route.Replace("//", "/");
        }

        public string GetBaseRouteWithParameter(string endPoint, string parameter, bool isAdminAction = false)
        {
            return $"api/{BaseRoute}/{parameter}/{endPoint}";
        }
    }
}
