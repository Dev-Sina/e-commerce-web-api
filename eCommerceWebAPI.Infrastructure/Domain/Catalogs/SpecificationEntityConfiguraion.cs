using eCommerceWebAPI.Domain.Catalogs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eCommerceWebAPI.Infrastructure.Domain.Catalogs
{
    internal sealed class SpecificationEntityConfiguraion : IEntityTypeConfiguration<Specification>
    {
        public void Configure(EntityTypeBuilder<Specification> builder)
        {
            builder.ToTable(nameof(Specification));
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Id).ValueGeneratedOnAdd();
            builder.Property(s => s.Name).IsRequired().HasMaxLength(200);

            builder
                .HasMany(s => s.SpecificationValues)
                .WithOne(sv => sv.Specification);
        }
    }
}
