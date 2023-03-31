using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using eCommerceWebAPI.Domain.Invoices;

namespace eCommerceWebAPI.Infrastructure.Domain.Invoices
{
    internal sealed class InvoiceEntityConfiguraion : IEntityTypeConfiguration<Invoice>
    {
        public void Configure(EntityTypeBuilder<Invoice> builder)
        {
            builder.ToTable(nameof(Invoice));
            builder.HasKey(i => i.Id);
            builder.Property(i => i.Id).ValueGeneratedOnAdd();
            builder.Property(i => i.OrderShippingCostExcludeTax).IsRequired().HasColumnType("decimal(18, 4)");
            builder.Property(i => i.OrderShippingCostIncludeTax).IsRequired().HasColumnType("decimal(18, 4)");
            builder.Property(i => i.OrderDiscountAmountExcludeTax).IsRequired().HasColumnType("decimal(18, 4)");
            builder.Property(i => i.OrderDiscountAmountIncludeTax).IsRequired().HasColumnType("decimal(18, 4)");
            builder.Property(i => i.OrderTotalDiscountAmountExcludeTax).IsRequired().HasColumnType("decimal(18, 4)");
            builder.Property(i => i.OrderTotalDiscountAmountIncludeTax).IsRequired().HasColumnType("decimal(18, 4)");
            builder.Property(i => i.OrderPayableAmount).IsRequired().HasColumnType("decimal(18, 4)");
            builder.Property(i => i.Currency).IsRequired().HasMaxLength(50);

            builder
                .HasMany(i => i.InvoiceItems)
                .WithOne(ii => ii.Invoice);

            builder
                .HasOne(i => i.InvoiceAddress)
                .WithOne(ia => ia.Invoice)
                .HasForeignKey<InvoiceAddress>(ia => ia.InvoiceId);
        }
    }
}
