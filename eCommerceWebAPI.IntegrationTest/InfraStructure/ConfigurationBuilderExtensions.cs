using Microsoft.Extensions.Configuration;

namespace eCommerceWebAPI.IntegrationTest.InfraStructure
{
    public static class ConfigurationBuilderExtensions
    {
        public static IConfigurationSection GetCustomSection(string section)
        {
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true);
            var config = configBuilder.Build();

            return config.GetSection(section);
        }

        public static IConfigurationRoot GetConfigurationRoot()
        {
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true);

            return configBuilder.Build();
        }
    }
}
