using eCommerceWebAPI.Domain.Customers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eCommerceWebAPI.Infrastructure.Domain.Customers
{
    internal sealed class CustomerAddressMappingEntityConfiguraion : IEntityTypeConfiguration<CustomerAddressMapping>
    {
        public void Configure(EntityTypeBuilder<CustomerAddressMapping> builder)
        {
            builder.ToTable(nameof(CustomerAddressMapping));
            builder.HasKey(cam => cam.Id);

            builder
                .HasOne(cam => cam.Customer)
                .WithMany(c => c.CustomerAddressMappings)
                .HasForeignKey(cam => cam.CustomerId);

            builder
                .HasOne(cam => cam.Address)
                .WithMany(a => a.CustomerAddressMappings)
                .HasForeignKey(cam => cam.AddressId);
        }
    }
}
