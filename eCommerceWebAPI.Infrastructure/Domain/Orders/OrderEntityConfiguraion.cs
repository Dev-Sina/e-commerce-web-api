using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using eCommerceWebAPI.Domain.Orders;

namespace eCommerceWebAPI.Infrastructure.Domain.Orders
{
    internal sealed class OrderEntityConfiguraion : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable(nameof(Order));
            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id).ValueGeneratedOnAdd();
            builder.Property(o => o.ShippingCostExcludeTax).IsRequired().HasColumnType("decimal(18, 4)");
            builder.Property(o => o.ShippingCostIncludeTax).IsRequired().HasColumnType("decimal(18, 4)");
            builder.Property(o => o.OrderDiscountAmountExcludeTax).IsRequired().HasColumnType("decimal(18, 4)");
            builder.Property(o => o.OrderDiscountAmountIncludeTax).IsRequired().HasColumnType("decimal(18, 4)");
            builder.Property(o => o.Currency).IsRequired().HasMaxLength(50);

            builder
                .HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order);

            builder
                .HasOne(o => o.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CustomerId)
                .IsRequired();

            builder
                .HasOne(o => o.Address)
                .WithMany(a => a.Orders)
                .HasForeignKey(o => o.AddressId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
