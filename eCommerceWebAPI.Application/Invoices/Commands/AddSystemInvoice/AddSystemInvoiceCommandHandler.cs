using eCommerceWebAPI.Application.Configuration.Commands;
using eCommerceWebAPI.Application.SeedWork;
using eCommerceWebAPI.Domain;
using System.Net;
using eCommerceWebAPI.Domain.Invoices;
using eCommerceWebAPI.Domain.Orders;
using Microsoft.EntityFrameworkCore;

namespace eCommerceWebAPI.Application.Invoices.Commands.AddSystemInvoice
{
    public class AddSystemInvoiceCommandHandler : ICommandHandler<AddSystemInvoiceCommand, BaseResponse>
    {
        private readonly IRepository<Invoice> _invoiceRepository;
        private readonly IRepository<Order> _orderRepository;

        public AddSystemInvoiceCommandHandler(IRepository<Invoice> invoiceRepository,
            IRepository<Order> orderRepository)
        {
            _invoiceRepository = invoiceRepository;
            _orderRepository = orderRepository;
        }

        public async Task<BaseResponse> Handle(AddSystemInvoiceCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(nameof(request));

            BaseResponse response = new();

            // Check related order would be existed
            long orderId = request.OrderId;
            var order = await _orderRepository
                .AsQueryable()
                .Include(o => o.Address)
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null)
            {
                response.Errors.Add($"Order with ID of {orderId} not found!");
                response.HttpStatusCode = HttpStatusCode.BadRequest;
                return response;
            }

            ArgumentNullException.ThrowIfNull(nameof(order.Address));

            ArgumentException.ThrowIfNullOrEmpty(nameof(order.OrderItems));

            // Evaluate prices, discounts and etc.
            var orderItems = order.OrderItems.ToList();

            decimal taxCoefficient = (decimal)1.09;

            decimal orderItemsTotalPriceExcludeTax = orderItems.Select(oi => oi.ProductUnitPriceExcludeTax * oi.Quantity).Sum();
            decimal orderItemsTotalPriceIncludeTax = orderItems.Select(oi => oi.ProductUnitPriceIncludeTax * oi.Quantity).Sum();
            decimal orderItemsTotalDiscountAmountsExcludeTax = orderItems.Select(oi => oi.ProductUnitDiscountAmountExcludeTax * oi.Quantity).Sum();
            decimal orderItemsTotalDiscountAmountsIncludeTax = orderItems.Select(oi => oi.ProductUnitDiscountAmountIncludeTax * oi.Quantity).Sum();

            decimal orderTotalDiscountAmountExcludeTax = order.OrderDiscountAmountExcludeTax + orderItemsTotalDiscountAmountsExcludeTax;
            decimal orderTotalDiscountAmountIncludeTax = order.OrderDiscountAmountExcludeTax + orderItemsTotalDiscountAmountsIncludeTax;

            decimal orderPayableAmountExcludeTax = orderItemsTotalPriceExcludeTax + order.ShippingCostExcludeTax - orderTotalDiscountAmountExcludeTax;
            decimal orderPayableAmount = orderItemsTotalPriceIncludeTax + order.ShippingCostIncludeTax - orderTotalDiscountAmountIncludeTax;

            // Create invoice entity
            Invoice invoice = new()
            {
                OrderId = orderId,
                OrderCreatedOnUtc = order.CreatedOnUtc,
                OrderShippingCostExcludeTax = order.ShippingCostExcludeTax,
                OrderShippingCostIncludeTax = order.ShippingCostIncludeTax,
                OrderDiscountAmountExcludeTax = order.OrderDiscountAmountExcludeTax,
                OrderDiscountAmountIncludeTax = order.OrderDiscountAmountIncludeTax,
                OrderTotalDiscountAmountExcludeTax = orderTotalDiscountAmountExcludeTax,
                OrderTotalDiscountAmountIncludeTax = orderTotalDiscountAmountIncludeTax,
                OrderPayableAmount = orderPayableAmount,
                Currency = "IRT",
                InvoiceCreatedOnUtc = DateTime.UtcNow
            };

            // Create invoice address entity
            var orderAddress = order.Address;
            InvoiceAddress invoiceAddress = new()
            {
                Title = orderAddress.Title?.Trim(),
                FirstName = orderAddress.FirstName!.Trim(),
                LastName = orderAddress.LastName!.Trim(),
                CountryName = orderAddress.CountryName!.Trim(),
                ProvinceName = orderAddress.ProvinceName!.Trim(),
                CityName = orderAddress.CityName!.Trim(),
                StreetAddress = orderAddress.StreetAddress!.Trim(),
                PostalCode = orderAddress.PostalCode?.Trim(),
                PhoneNumber = orderAddress.PhoneNumber!.Trim()
            };
            invoice.InvoiceAddress = invoiceAddress;

            // Create invoice items entities
            orderItems.ForEach(orderItem =>
            {
                InvoiceItem invoiceItem = new()
                {
                    OrderId = orderId,
                    ProductId = orderItem.ProductId,
                    ProductName = orderItem.ProductName.Trim(),
                    ProductCode = orderItem.ProductCode.Trim(),
                    ProductUnitPriceExcludeTax = orderItem.ProductUnitPriceExcludeTax,
                    ProductUnitPriceIncludeTax = orderItem.ProductUnitPriceIncludeTax,
                    ProductUnitDiscountAmountExcludeTax = orderItem.ProductUnitDiscountAmountExcludeTax,
                    ProductUnitDiscountAmountIncludeTax = orderItem.ProductUnitDiscountAmountIncludeTax,
                    Quantity = orderItem.Quantity
                };
                invoice.InvoiceItems.Add(invoiceItem);
            });

            // Inserting invoice into the Db
            var insertedInvoice = await _invoiceRepository.Add(invoice);

            // Check it's inserted or not
            if (insertedInvoice.Id <= 0)
            {
                response.HttpStatusCode = HttpStatusCode.InternalServerError;
                response.Errors.Add("An error has been occured while inserting invoice!");
                return response;
            }

            // Return response
            response.HttpStatusCode = HttpStatusCode.Created;
            response.Message = "The invoice has been inserted successfully!";
            return response;
        }
    }
}
