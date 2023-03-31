using eCommerceWebAPI.Domain.Catalogs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eCommerceWebAPI.Infrastructure.Domain.Catalogs
{
    internal sealed class ProductCategoryMappingEntityConfiguraion : IEntityTypeConfiguration<ProductCategoryMapping>
    {
        public void Configure(EntityTypeBuilder<ProductCategoryMapping> builder)
        {
            builder.ToTable(nameof(ProductCategoryMapping));
            builder.HasKey(pcm => pcm.Id);
            builder.Property(pcm => pcm.Id).ValueGeneratedOnAdd();

            builder
                .HasOne(pcm => pcm.Product)
                .WithMany(p => p.ProductCategoryMappings)
                .HasForeignKey(pcm => pcm.ProductId);

            builder
                .HasOne(pcm => pcm.Category)
                .WithMany(c => c.ProductCategoryMappings)
                .HasForeignKey(pcm => pcm.CategoryId);
        }
    }
}
