using eCommerceWebAPI.Domain;
using eCommerceWebAPI.Domain.Addresses;
using eCommerceWebAPI.Domain.Catalogs;
using eCommerceWebAPI.Domain.Customers;
using eCommerceWebAPI.Domain.Invoices;
using eCommerceWebAPI.Domain.Orders;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace eCommerceWebAPI.Infrastructure.Databases
{
    public class CommonDataContext : DbContext
    {
        private readonly string _connectionString;

        public CommonDataContext()
        {
        }

        public CommonDataContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DbSet<Country> Countries { get; set; }
        public DbSet<Province> Provinces { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Address> Addresses { get; set; }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerAddressMapping> CustomerAddressMappings { get; set; }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductCategoryMapping> ProductCategoryMappings { get; set; }
        public DbSet<Specification> Specifications { get; set; }
        public DbSet<SpecificationValue> SpecificationValues { get; set; }
        public DbSet<ProductSpecificationValueMapping> ProductSpecificationValueMappings { get; set; }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceAddress> InvoiceAddresses { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Exclude soft delete records from queries by default
            modelBuilder.Entity<Product>().HasQueryFilter(x => !x.Deleted);
            modelBuilder.Entity<Order>().HasQueryFilter(x => !x.Deleted);
            modelBuilder.Entity<Customer>().HasQueryFilter(x => !x.Deleted);

            // base creation and apply configurations
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CommonDataContext).Assembly);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            ChangeTracker.DetectChanges();

            foreach (var entry in ChangeTracker.Entries().Where(e => e.State == EntityState.Deleted))
            {
                if (entry.Entity is BaseSoftDeleteSqlEntity entity)
                {
                    entry.State = EntityState.Modified;
                    entity.Deleted = true;
                }
            }

            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default)
        {
            ChangeTracker.DetectChanges();
            
            foreach (var entry in ChangeTracker.Entries().Where(e => e.State == EntityState.Deleted))
            {
                if (entry.Entity is BaseSoftDeleteSqlEntity entity)
                {
                    entry.State = EntityState.Modified;
                    entity.Deleted = true;
                }
            }

            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
    }
}
