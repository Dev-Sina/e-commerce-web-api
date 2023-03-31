using eCommerceWebAPI.Domain.Catalogs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eCommerceWebAPI.Infrastructure.Domain.Catalogs
{
    internal sealed class ProductSpecificationValueMappingEntityConfiguraion : IEntityTypeConfiguration<ProductSpecificationValueMapping>
    {
        public void Configure(EntityTypeBuilder<ProductSpecificationValueMapping> builder)
        {
            builder.ToTable(nameof(ProductSpecificationValueMapping));
            builder.HasKey(psvm => psvm.Id);
            //builder.HasKey(psvm => new { psvm.ProductId, psvm.SpecificationValueId});

            builder
                .HasOne(psvm => psvm.Product)
                .WithMany(p => p.ProductSpecificationValueMappings)
                .HasForeignKey(psvm => psvm.ProductId);

            builder
                .HasOne(psvm => psvm.SpecificationValue)
                .WithMany(sv => sv.ProductSpecificationValueMappings)
                .HasForeignKey(psvm => psvm.SpecificationValueId);
        }
    }
}
