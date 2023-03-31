using eCommerceWebAPI.Api;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace eCommerceWebAPI.IntegrationTest.InfraStructure
{
    public class BaseEventSubscriptionTest
    {
        protected internal WebApplicationFactory<Startup> Factory;

        [OneTimeSetUp]
        public void Setup()
        {
            Factory = new WebApplicationFactory<Startup>()
                    .WithWebHostBuilder(x =>
                    {
                        x.ConfigureAppConfiguration((c, b) =>
                        {
                            b.AddConfiguration(ConfigurationBuilderExtensions.GetConfigurationRoot());
                        });
                    })
                ;
        }

        [TearDown]
        public void TearDown()
        {
        }
    }
}
