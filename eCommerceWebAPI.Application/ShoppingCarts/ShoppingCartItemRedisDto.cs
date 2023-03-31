using eCommerceWebAPI.Domain.Orders;

namespace eCommerceWebAPI.Application.ShoppingCarts
{
    public class ShoppingCartItemRedisDto
    {
        public long ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public string ProductCode { get; set; } = string.Empty;

        public decimal ProductUnitPriceExcludeTax { get; set; }
        public string ProductUnitPriceExcludeTaxNormalized => ProductUnitPriceExcludeTax.ToString("N0") + " " + Currency;

        public decimal ProductUnitPriceIncludeTax { get; set; }
        public string ProductUnitPriceIncludeTaxNormalized => ProductUnitPriceIncludeTax.ToString("N0") + " " + Currency;

        public decimal ProductUnitDiscountAmountExcludeTax { get; set; }
        public string ProductUnitDiscountAmountExcludeTaxNormalized => ProductUnitDiscountAmountExcludeTax.ToString("N0") + " " + Currency;

        public decimal ProductUnitDiscountAmountIncludeTax { get; set; }
        public string ProductUnitDiscountAmountIncludeTaxNormalized => ProductUnitDiscountAmountIncludeTax.ToString("N0") + " " + Currency;

        public decimal ProductUnitPayablePriceExcludeTax => ProductUnitPriceExcludeTax - ProductUnitDiscountAmountExcludeTax;
        public string ProductUnitPayablePriceExcludeTaxNormalized => ProductUnitPayablePriceExcludeTax.ToString("N0") + " " + Currency;

        public decimal ProductUnitPayablePriceIncludeTax => ProductUnitPriceIncludeTax - ProductUnitDiscountAmountIncludeTax;
        public string ProductUnitPayablePriceIncludeTaxNormalized => ProductUnitPayablePriceIncludeTax.ToString("N0") + " " + Currency;

        public decimal ProductTotalPayablePriceExcludeTax => ProductUnitPayablePriceExcludeTax * Quantity;
        public string ProductTotalPayablePriceExcludeTaxNormalized => ProductTotalPayablePriceExcludeTax.ToString("N0") + " " + Currency;

        public decimal ProductTotalPayablePriceIncludeTax => ProductUnitPayablePriceIncludeTax * Quantity;
        public string ProductTotalPayablePriceIncludeTaxNormalized => ProductTotalPayablePriceExcludeTax.ToString("N0") + " " + Currency;

        public int Quantity { get; set; }

        public string Currency { get; set; } = string.Empty;
    }
}
