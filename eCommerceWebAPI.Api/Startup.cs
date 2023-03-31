using eCommerceWebAPI.Api.Configurations;
using eCommerceWebAPI.Infrastructure.Databases;
using Hellang.Middleware.ProblemDetails;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Data;
using System.Data.SqlClient;
using System.Text.Json.Serialization;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using FluentValidation.AspNetCore;
using eCommerceWebAPI.Application.Configuration.Mapping;
using StackExchange.Redis;
using Autofac.Extensions.DependencyInjection;
using Autofac;
using CommonServiceLocator;
using Autofac.Extras.CommonServiceLocator;
using eCommerceWebAPI.Infrastructure.Processing;
using eCommerceWebAPI.Domain;
using eCommerceWebAPI.Infrastructure.Domain;
using eCommerceWebAPI.Domain.SeedWork;
using eCommerceWebAPI.Application.Validation;
using eCommerceWebAPI.Api.SeedWork;
using eCommerceWebAPI.Application.Services.MessageBrokers.RabbitMQ;

namespace eCommerceWebAPI.Api
{
    public class Startup
    {
        private const string _connectionStringForSqlServer = "DefaultConnection";
        private const string _connectionStringForRedis = "RedisConnection";
        private const string _rabbitMQHostName = "RabbitMQHostName";
        private const string _rabbitMQUserName = "RabbitMQUserName";
        private const string _rabbitMQPassword = "RabbitMQPassword";

        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            var isProduction = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")?.ToLower() == "production";

            services
                .AddFluentValidationAutoValidation()
                .AddFluentValidationClientsideAdapters()
                .AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                });

            services.AddProblemDetails(x =>
            {
                x.Map<InvalidCommandException>(ex => new InvalidCommandProblemDetails(ex));
            });

            #region mapper

            var mapperConfig = new MapperConfiguration(mc => { mc.AddProfile(new AutoMapping()); });
            var mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);
            services.AddAutoMapper(typeof(Startup));

            #endregion

            #region Cors

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAnyOrigin",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });

            #endregion

            #region ApiVersion

            services.AddApiVersioning(config =>
            {
                config.DefaultApiVersion = new ApiVersion(1, 0);
                config.AssumeDefaultVersionWhenUnspecified = true;
                config.ReportApiVersions = true;
                config.ApiVersionReader = ApiVersionReader
                    .Combine(new HeaderApiVersionReader("X-version"), new QueryStringApiVersionReader("api-version"));
            });

            #endregion

            #region DbContext

            var connectionStringForSqlServer = _configuration.GetSection(_connectionStringForSqlServer).Value;
            var connectionStringForRedis = _configuration.GetSection(_connectionStringForRedis).Value;
            string rabbitMQHostName = _configuration.GetSection(_rabbitMQHostName).Value ?? string.Empty;
            string rabbitMQUserName = _configuration.GetSection(_rabbitMQUserName).Value ?? string.Empty;
            string rabbitMQPassword = _configuration.GetSection(_rabbitMQPassword).Value ?? string.Empty;

            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("MSSQL_SA_PASSWORD")))
            {
                var builder = new SqlConnectionStringBuilder(connectionStringForSqlServer)
                {
                    Password = Environment.GetEnvironmentVariable("MSSQL_SA_PASSWORD")
                };
                connectionStringForSqlServer = builder.ConnectionString;
            }

            // Configure the SQL Server database context
            services.AddDbContext<CommonDataContext>(options =>
                {
                    if (!isProduction)
                    {
                        options.EnableSensitiveDataLogging();
                    }

                    options.UseSqlServer(connectionStringForSqlServer);
                });

            // Configure the Redis database connection
            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(connectionStringForRedis ?? string.Empty));

            services.AddTransient<IDbConnection>((sp) => new SqlConnection(connectionStringForSqlServer));

            services.AddTransient<CommonDataContext>();

            #endregion

            #region Swagger

            RegisterSwagger(services);

            #endregion

            #region services

            RegisterServices(services);

            #endregion

            #region Rabbit MQ publishers/consumers

            RegisterRabbitMQConsumers(services, rabbitMQHostName, rabbitMQUserName, rabbitMQPassword);

            #endregion

            #region Modules

            var container = new ContainerBuilder();

            container
                .Register(c =>
                    {
                        var dbContextOptionsBuilder = new DbContextOptionsBuilder<CommonDataContext>();
                        dbContextOptionsBuilder.UseSqlServer(connectionStringForSqlServer);

                        return new CommonDataContext(connectionStringForSqlServer!);
                    })
                .AsSelf()
                .InstancePerLifetimeScope();

            services.AddSingleton<IShoppingCartRepository>(sp =>
            {
                var redis = ConnectionMultiplexer.Connect(connectionStringForRedis ?? string.Empty);
                return new ShoppingCartRepository(redis, connectionStringForRedis);
            });

            container.Populate(services);

            container.RegisterModule(new DataAccessModule(connectionStringForSqlServer!));
            container.RegisterModule(new MediatorModule());
            container.RegisterModule(new MessageBrokerModule(rabbitMQHostName, rabbitMQUserName, rabbitMQPassword));

            var buildContainer = container.Build();

            #endregion

            ServiceLocator.SetLocatorProvider(() => new AutofacServiceLocator(buildContainer));

            var autofacServiceProvider = new AutofacServiceProvider(buildContainer);

            buildContainer.BeginLifetimeScope();

            return autofacServiceProvider;
        }

        private static void RegisterSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "eCommerce Web API App",
                    Version = "v1",
                    Description = "This is eCommerce Web API App!"
                });
                options.EnableAnnotations();
                options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Scheme = "bearer"
                });
                options.OperationFilter<AddRequiredHeaderParameter>();

                //var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                //var commentsFileName = Assembly.GetExecutingAssembly().GetName().Name + ".xml";
                //var commentsFile = Path.Combine(baseDirectory, commentsFileName);
                //options.IncludeXmlComments(commentsFile);
            });
        }

        private static void RegisterServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<IDbInitializer, DbInitializer>();
        }

        private static void RegisterRabbitMQConsumers(IServiceCollection services,
            string rabbitMQHostName,
            string rabbitMQUserName,
            string rabbitMQPassword)
        {
            services.AddSingleton<IHostedService>(provider => new OrderPlacedRabbitMQConsumerHostedService(new OrderPlacedRabbitMQMessageConfig(), rabbitMQHostName, rabbitMQUserName, rabbitMQPassword, provider));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment() || env.IsEnvironment("LD_DEBUG"))
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseProblemDetails();
                app.UseResponseCompression();
            }

            app.UseCors("AllowAnyOrigin");

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            app.UseSwagger();

            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "eCommerce Web API App"); });
        }
    }
}