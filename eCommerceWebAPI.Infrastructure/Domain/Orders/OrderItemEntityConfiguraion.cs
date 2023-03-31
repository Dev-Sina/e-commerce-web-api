using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using eCommerceWebAPI.Domain.Orders;

namespace eCommerceWebAPI.Infrastructure.Domain.Orders
{
    internal sealed class OrderItemEntityConfiguraion : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable(nameof(OrderItem));
            builder.HasKey(oi => oi.Id);
            builder.Property(oi => oi.Id).ValueGeneratedOnAdd();
            builder.Property(oi => oi.ProductName).IsRequired().HasMaxLength(400);
            builder.Property(oi => oi.ProductCode).IsRequired().HasMaxLength(100);
            builder.Property(oi => oi.ProductUnitPriceExcludeTax).IsRequired().HasColumnType("decimal(18, 4)");
            builder.Property(oi => oi.ProductUnitPriceIncludeTax).IsRequired().HasColumnType("decimal(18, 4)");
            builder.Property(oi => oi.ProductUnitDiscountAmountExcludeTax).IsRequired().HasColumnType("decimal(18, 4)");
            builder.Property(oi => oi.ProductUnitDiscountAmountIncludeTax).IsRequired().HasColumnType("decimal(18, 4)");

            builder
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId);
        }
    }
}
