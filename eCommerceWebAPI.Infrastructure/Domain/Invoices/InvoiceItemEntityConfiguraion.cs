using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using eCommerceWebAPI.Domain.Invoices;

namespace eCommerceWebAPI.Infrastructure.Domain.Invoices
{
    internal sealed class InvoiceItemAddressEntityConfiguraion : IEntityTypeConfiguration<InvoiceItem>
    {
        public void Configure(EntityTypeBuilder<InvoiceItem> builder)
        {
            builder.ToTable(nameof(InvoiceItem));
            builder.HasKey(ii => ii.Id);
            builder.Property(ii => ii.Id).ValueGeneratedOnAdd();
            builder.Property(ii => ii.ProductName).IsRequired().HasMaxLength(400);
            builder.Property(ii => ii.ProductCode).IsRequired().HasMaxLength(100);
            builder.Property(ii => ii.ProductUnitPriceExcludeTax).IsRequired().HasColumnType("decimal(18, 4)");
            builder.Property(ii => ii.ProductUnitPriceIncludeTax).IsRequired().HasColumnType("decimal(18, 4)");
            builder.Property(ii => ii.ProductUnitDiscountAmountExcludeTax).IsRequired().HasColumnType("decimal(18, 4)");
            builder.Property(ii => ii.ProductUnitDiscountAmountIncludeTax).IsRequired().HasColumnType("decimal(18, 4)");

            builder
                .HasOne(ii => ii.Invoice)
                .WithMany(i => i.InvoiceItems)
                .HasForeignKey(ii => ii.InvoiceId);
        }
    }
}
