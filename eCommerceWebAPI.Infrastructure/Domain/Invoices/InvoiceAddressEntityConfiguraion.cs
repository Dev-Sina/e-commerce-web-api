using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using eCommerceWebAPI.Domain.Invoices;

namespace eCommerceWebAPI.Infrastructure.Domain.Invoices
{
    internal sealed class InvoiceAddressEntityConfiguraion : IEntityTypeConfiguration<InvoiceAddress>
    {
        public void Configure(EntityTypeBuilder<InvoiceAddress> builder)
        {
            builder.ToTable(nameof(InvoiceAddress));
            builder.HasKey(ia => ia.Id);
            builder.Property(ia => ia.Id).ValueGeneratedOnAdd();
            builder.Property(ia => ia.Title).HasMaxLength(200);
            builder.Property(ia => ia.FirstName).IsRequired().HasMaxLength(200);
            builder.Property(ia => ia.LastName).IsRequired().HasMaxLength(400);
            builder.Property(ia => ia.CountryName).IsRequired().HasMaxLength(100);
            builder.Property(ia => ia.ProvinceName).IsRequired().HasMaxLength(100);
            builder.Property(ia => ia.CityName).IsRequired().HasMaxLength(100);
            builder.Property(ia => ia.StreetAddress).IsRequired().HasMaxLength(400);
            builder.Property(ia => ia.PostalCode).HasMaxLength(20);
            builder.Property(ia => ia.PhoneNumber).IsRequired().HasMaxLength(20);

            //builder
            //    .HasOne(ia => ia.Invoice)
            //    .WithOne(i => i.InvoiceAddress)
            //    .HasForeignKey<Invoice>(i => i.InvoiceAddressId);
        }
    }
}
