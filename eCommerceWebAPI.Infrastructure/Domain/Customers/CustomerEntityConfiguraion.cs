using eCommerceWebAPI.Domain.Customers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eCommerceWebAPI.Infrastructure.Domain.Customers
{
    internal sealed class CustomerEntityConfiguraion : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable(nameof(Customer));
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).ValueGeneratedOnAdd();
            builder.Property(c => c.FirstName).IsRequired().HasMaxLength(200);
            builder.Property(c => c.LastName).IsRequired().HasMaxLength(400);
            builder.Property(c => c.NationalCode).HasMaxLength(20);

            builder
                .HasMany(c => c.CustomerAddressMappings)
                .WithOne(cam => cam.Customer);

            builder
                .HasMany(c => c.Orders)
                .WithOne(o => o.Customer);
        }
    }
}
