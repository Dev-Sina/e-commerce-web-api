using eCommerceWebAPI.Application.Addresses;
using eCommerceWebAPI.Application.Customers;

namespace eCommerceWebAPI.Application.Orders
{
    public class OrderDto
    {
        public long Id { get; set; }

        public long CustomerId { get; set; }

        public DateTime CreatedOnUtc { get; set; }

        public decimal ShippingCostExcludeTax { get; set; }
        public string ShippingCostExcludeTaxNormalized => ShippingCostExcludeTax.ToString("N0") + " " + Currency;

        public decimal ShippingCostIncludeTax { get; set; }
        public string ShippingCostIncludeTaxNormalized => ShippingCostIncludeTax.ToString("N0") + " " + Currency;

        public decimal OrderDiscountAmountExcludeTax { get; set; }
        public string OrderDiscountAmountExcludeTaxNormalized => OrderDiscountAmountExcludeTax.ToString("N0") + " " + Currency;

        public decimal OrderDiscountAmountIncludeTax { get; set; }
        public string OrderDiscountAmountIncludeTaxNormalized => OrderDiscountAmountIncludeTax.ToString("N0") + " " + Currency;

        public decimal OrderTotalDiscountAmountExcludeTax => OrderDiscountAmountExcludeTax + OrderItems.Select(oiDto => oiDto.ProductUnitDiscountAmountExcludeTax * oiDto.Quantity).Sum();
        public string OrderTotalDiscountAmountExcludeTaxNormalized => OrderTotalDiscountAmountExcludeTax.ToString("N0") + " " + Currency;

        public decimal OrderTotalDiscountAmountIncludeTax => OrderDiscountAmountIncludeTax + OrderItems.Select(oiDto => oiDto.ProductUnitDiscountAmountIncludeTax * oiDto.Quantity).Sum();
        public string OrderTotalDiscountAmountIncludeTaxNormalized => OrderTotalDiscountAmountIncludeTax.ToString("N0") + " " + Currency;

        public decimal OrderPayableAmount => OrderItems.Select(oiDto => oiDto.ProductUnitPriceIncludeTax * oiDto.Quantity).Sum() - OrderTotalDiscountAmountIncludeTax + ShippingCostIncludeTax;
        public string OrderPayableAmountNormalized => OrderPayableAmount.ToString("N0") + " " + Currency;

        public string Currency { get; set; } = string.Empty;

        public long AddressId { get; set; }

        public bool Deleted { get; set; } = false;

        public virtual CustomerDto Customer { get; set; } = new();

        public virtual AddressDto Address { get; set; } = new();

        public virtual List<OrderItemDto> OrderItems { get; set; } = new();
    }
}
