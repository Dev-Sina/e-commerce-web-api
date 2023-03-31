using eCommerceWebAPI.Domain.Catalogs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eCommerceWebAPI.Infrastructure.Domain.Catalogs
{
    internal sealed class SpecificationValueEntityConfiguraion : IEntityTypeConfiguration<SpecificationValue>
    {
        public void Configure(EntityTypeBuilder<SpecificationValue> builder)
        {
            builder.ToTable(nameof(SpecificationValue));
            builder.HasKey(sv => sv.Id);
            builder.Property(sv => sv.Id).ValueGeneratedOnAdd();
            builder.Property(sv => sv.Name).IsRequired().HasMaxLength(200);

            builder
                .HasMany(sv => sv.ProductSpecificationValueMappings)
                .WithOne(psvm => psvm.SpecificationValue);

            builder
                .HasOne(sv => sv.Specification)
                .WithMany(s => s.SpecificationValues)
                .HasForeignKey(sv => sv.SpecificationId);
        }
    }
}
