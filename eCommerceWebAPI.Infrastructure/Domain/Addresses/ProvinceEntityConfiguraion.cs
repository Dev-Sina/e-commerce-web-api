using eCommerceWebAPI.Domain.Addresses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eCommerceWebAPI.Infrastructure.Domain.Addresses
{
    internal sealed class ProvinceEntityConfiguraion : IEntityTypeConfiguration<Province>
    {
        public void Configure(EntityTypeBuilder<Province> builder)
        {
            builder.ToTable(nameof(Province));
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).ValueGeneratedOnAdd();
            builder.Property(p => p.Name).IsRequired().HasMaxLength(100);

            builder
                .HasMany(p => p.Cities)
                .WithOne(c => c.Province);

            builder
                .HasOne(p => p.Country)
                .WithMany(c => c.Provinces)
                .HasForeignKey(p => p.CountryId);
        }
    }
}
