using eCommerceWebAPI.Domain.Catalogs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eCommerceWebAPI.Infrastructure.Domain.Catalogs
{
    internal sealed class ProductEntityConfiguraion : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable(nameof(Product));
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).ValueGeneratedOnAdd();
            builder.Property(p => p.Name).IsRequired().HasMaxLength(400);
            builder.Property(p => p.Code).IsRequired().HasMaxLength(100);
            builder.Property(p => p.Price).IsRequired().HasColumnType("decimal(18, 4)");
            builder.Property(p => p.DiscountAmount).IsRequired().HasColumnType("decimal(18, 4)");
            builder.Property(p => p.Currency).IsRequired().HasMaxLength(50);

            builder
                .HasMany(p => p.ProductSpecificationValueMappings)
                .WithOne(psvm => psvm.Product);

            builder
                .HasMany(p => p.ProductCategoryMappings)
                .WithOne(pcm => pcm.Product);
        }
    }
}
