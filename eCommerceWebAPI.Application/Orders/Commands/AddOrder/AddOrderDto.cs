using eCommerceWebAPI.Application.Addresses.Commands.AddCustomerAddress;

namespace eCommerceWebAPI.Application.Orders.Commands.AddOrder
{
    public class AddOrderDto
    {
        public decimal ShippingCostExcludeTax { get; set; }

        public decimal OrderDiscountAmountExcludeTax { get; set; }

        public virtual AddAddressDto AddOrderAddress { get; set; } = new();
    }
}
