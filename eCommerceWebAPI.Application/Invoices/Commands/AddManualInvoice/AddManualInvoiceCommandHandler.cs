using eCommerceWebAPI.Application.Configuration.Commands;
using eCommerceWebAPI.Application.SeedWork;
using eCommerceWebAPI.Domain;
using System.Net;
using eCommerceWebAPI.Domain.Invoices;
using eCommerceWebAPI.Domain.Orders;
using eCommerceWebAPI.Domain.Catalogs;

namespace eCommerceWebAPI.Application.Invoices.Commands.AddManualInvoice
{
    public class AddManualInvoiceCommandHandler : ICommandHandler<AddManualInvoiceCommand, BaseResponse>
    {
        private readonly IRepository<Invoice> _invoiceRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<Product> _productRepository;

        public AddManualInvoiceCommandHandler(IRepository<Invoice> invoiceRepository,
            IRepository<Order> orderRepository,
            IRepository<Product> productRepository)
        {
            _invoiceRepository = invoiceRepository;
            _orderRepository = orderRepository;
            _productRepository = productRepository;
        }

        public async Task<BaseResponse> Handle(AddManualInvoiceCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(nameof(request));

            var addManualInvoiceDto = request.AddManualInvoiceDto;
            ArgumentNullException.ThrowIfNull(nameof(addManualInvoiceDto));

            var addManualInvoiceAddressDto = addManualInvoiceDto.InvoiceAddress;
            ArgumentNullException.ThrowIfNull(nameof(addManualInvoiceAddressDto));

            var addManualInvoiceItemsDto = addManualInvoiceDto.InvoiceItems;
            ArgumentNullException.ThrowIfNullOrEmpty(nameof(addManualInvoiceItemsDto));

            BaseResponse response = new();

            // Check related order would be existed
            long orderId = request.OrderId;
            var order = await _orderRepository.Get(orderId);
            if (order == null)
            {
                response.Errors.Add($"Order with ID of {orderId} not found!");
                response.HttpStatusCode = HttpStatusCode.BadRequest;
                return response;
            }

            // Bad request parameters
            if (string.IsNullOrEmpty(addManualInvoiceAddressDto.FirstName?.Trim())) response.Errors.Add("Invoice address first name could not be empty!");
            if (string.IsNullOrEmpty(addManualInvoiceAddressDto.LastName?.Trim())) response.Errors.Add("Invoice address last name could not be empty!");
            if (string.IsNullOrEmpty(addManualInvoiceAddressDto.CountryName?.Trim())) response.Errors.Add("Invoice address country name not be empty!");
            if (string.IsNullOrEmpty(addManualInvoiceAddressDto.ProvinceName?.Trim())) response.Errors.Add("Invoice address province name could not be empty!");
            if (string.IsNullOrEmpty(addManualInvoiceAddressDto.CityName?.Trim())) response.Errors.Add("Invoice address city name could not be empty!");
            if (string.IsNullOrEmpty(addManualInvoiceAddressDto.StreetAddress?.Trim())) response.Errors.Add("Invoice address street could not be empty!");
            if (string.IsNullOrEmpty(addManualInvoiceAddressDto.PhoneNumber?.Trim())) response.Errors.Add("Invoice address phone number could not be empty!");
            foreach (var addManualInvoiceItemDto in addManualInvoiceItemsDto)
            {
                if (string.IsNullOrEmpty(addManualInvoiceItemDto.ProductName?.Trim())) response.Errors.Add("Invoice item product name could not be empty!");
                if (string.IsNullOrEmpty(addManualInvoiceItemDto.ProductCode?.Trim())) response.Errors.Add("Invoice item product code could not be empty!");
                if (addManualInvoiceItemDto.Quantity <= 0) response.Errors.Add($"Invoice item quantity could not be {addManualInvoiceItemDto.Quantity}!");

                var product = await _productRepository.Get(addManualInvoiceItemDto.ProductId);
                if (product == null) response.Errors.Add($"Invoice item product with ID of : {addManualInvoiceItemDto.Quantity} is not existed!");
            }
            if (response.Errors.Any())
            {
                response.HttpStatusCode = HttpStatusCode.BadRequest;
                return response;
            };

            // Evaluate prices, discounts and etc.
            decimal taxCoefficient = (decimal)1.09;

            decimal orderItemsTotalPriceExcludeTax = addManualInvoiceItemsDto.Select(iiDto => iiDto.ProductUnitPriceExcludeTax * iiDto.Quantity).Sum();
            decimal orderItemsTotalDiscountAmountsExcludeTax = addManualInvoiceItemsDto.Select(iiDto => iiDto.ProductUnitDiscountAmountExcludeTax * iiDto.Quantity).Sum();

            decimal orderShippingCostExcludeTax = addManualInvoiceDto.OrderShippingCostExcludeTax;
            decimal orderDiscountAmountExcludeTax = addManualInvoiceDto.OrderDiscountAmountExcludeTax;

            decimal orderTotalDiscountAmountExcludeTax = orderDiscountAmountExcludeTax + orderItemsTotalDiscountAmountsExcludeTax;

            decimal orderPayableAmountExcludeTax = orderItemsTotalPriceExcludeTax + orderShippingCostExcludeTax - orderTotalDiscountAmountExcludeTax;
            decimal orderPayableAmount = orderPayableAmountExcludeTax * taxCoefficient;

            // Create invoice entity
            Invoice invoice = new()
            {
                OrderId = orderId,
                OrderCreatedOnUtc = order.CreatedOnUtc,
                OrderShippingCostExcludeTax = orderShippingCostExcludeTax,
                OrderShippingCostIncludeTax = orderShippingCostExcludeTax * taxCoefficient,
                OrderDiscountAmountExcludeTax = orderDiscountAmountExcludeTax,
                OrderDiscountAmountIncludeTax = orderDiscountAmountExcludeTax * taxCoefficient,
                OrderTotalDiscountAmountExcludeTax = orderTotalDiscountAmountExcludeTax,
                OrderTotalDiscountAmountIncludeTax = orderTotalDiscountAmountExcludeTax * taxCoefficient,
                OrderPayableAmount = orderPayableAmount,
                Currency = "IRT",
                InvoiceCreatedOnUtc = DateTime.UtcNow
            };

            // Create invoice address entity
            InvoiceAddress invoiceAddress = new()
            {
                Title = addManualInvoiceAddressDto.Title?.Trim(),
                FirstName = addManualInvoiceAddressDto.FirstName!.Trim(),
                LastName = addManualInvoiceAddressDto.LastName!.Trim(),
                CountryName = addManualInvoiceAddressDto.CountryName!.Trim(),
                ProvinceName = addManualInvoiceAddressDto.ProvinceName!.Trim(),
                CityName = addManualInvoiceAddressDto.CityName!.Trim(),
                StreetAddress = addManualInvoiceAddressDto.StreetAddress!.Trim(),
                PostalCode = addManualInvoiceAddressDto.PostalCode?.Trim(),
                PhoneNumber = addManualInvoiceAddressDto.PhoneNumber!.Trim()
            };
            invoice.InvoiceAddress = invoiceAddress;

            // Create invoice items entities
            addManualInvoiceItemsDto.ForEach(addManualInvoiceItemDto =>
            {
                InvoiceItem invoiceItem = new()
                {
                    OrderId = orderId,
                    ProductId = addManualInvoiceItemDto.ProductId,
                    ProductName = addManualInvoiceItemDto.ProductName.Trim(),
                    ProductCode = addManualInvoiceItemDto.ProductCode.Trim(),
                    ProductUnitPriceExcludeTax = addManualInvoiceItemDto.ProductUnitPriceExcludeTax,
                    ProductUnitPriceIncludeTax = addManualInvoiceItemDto.ProductUnitPriceExcludeTax * taxCoefficient,
                    ProductUnitDiscountAmountExcludeTax = addManualInvoiceItemDto.ProductUnitDiscountAmountExcludeTax,
                    ProductUnitDiscountAmountIncludeTax = addManualInvoiceItemDto.ProductUnitDiscountAmountExcludeTax * taxCoefficient,
                    Quantity = addManualInvoiceItemDto.Quantity
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
