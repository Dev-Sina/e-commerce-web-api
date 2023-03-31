using eCommerceWebAPI.Domain.Addresses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eCommerceWebAPI.Infrastructure.Domain.Addresses
{
    internal sealed class AddressEntityConfiguraion : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.ToTable(nameof(Address));
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id).ValueGeneratedOnAdd();
            builder.Property(a => a.Title).HasMaxLength(200);
            builder.Property(a => a.FirstName).IsRequired().HasMaxLength(200);
            builder.Property(a => a.LastName).IsRequired().HasMaxLength(400);
            builder.Property(a => a.CountryName).IsRequired().HasMaxLength(100);
            builder.Property(a => a.ProvinceName).IsRequired().HasMaxLength(100);
            builder.Property(a => a.CityName).IsRequired().HasMaxLength(100);
            builder.Property(a => a.StreetAddress).IsRequired().HasMaxLength(400);
            builder.Property(a => a.PostalCode).HasMaxLength(20);
            builder.Property(a => a.PhoneNumber).IsRequired().HasMaxLength(20);

            builder
                .HasMany(c => c.CustomerAddressMappings)
                .WithOne(cam => cam.Address);

            builder
                .HasMany(c => c.Orders)
                .WithOne(o => o.Address);
        }
    }
}
