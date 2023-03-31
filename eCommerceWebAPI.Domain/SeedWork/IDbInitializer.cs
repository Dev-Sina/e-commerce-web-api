namespace eCommerceWebAPI.Domain.SeedWork
{
    /// <summary>
    /// Represents the DB initializer interface
    /// </summary>
    public interface IDbInitializer
    {
        /// <summary>
        /// Adds some default values to the IdentityDb
        /// </summary>
        void SeedData();

        Task SeedCountries();

        Task SeedProvinces();

        Task SeedCities();

        Task SeedAddresses();

        Task Customers();

        Task SeedCategories();

        Task SeedSpecifications();

        Task SeedProducts();
    }
}
