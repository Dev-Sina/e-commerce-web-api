using AutoMapper;
using eCommerceWebAPI.Application.Configuration;
using eCommerceWebAPI.Application.Configuration.Queries;
using Dapper;
using eCommerceWebAPI.Domain.Invoices;

namespace eCommerceWebAPI.Application.Invoices.Queries.GetInvoice
{
    public class GetAllInvoicesQueryHandler : IQueryHandler<GetAllInvoicesQuery, IList<InvoiceDto>>
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;
        private readonly IMapper _mapper;

        public GetAllInvoicesQueryHandler(ISqlConnectionFactory sqlConnectionFactory,
            IMapper mapper)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
            _mapper = mapper;
        }

        public async Task<IList<InvoiceDto>> Handle(GetAllInvoicesQuery request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));

            try
            {
                var connection = _sqlConnectionFactory.GetOpenConnection();

                const string sqlInvoices = $@"
                                    SELECT
                                       [Invoice].[Id],
                                       [Invoice].[OrderId],
                                       [Invoice].[OrderCreatedOnUtc],
                                       [Invoice].[OrderShippingCostExcludeTax],
                                       [Invoice].[OrderShippingCostIncludeTax],
                                       [Invoice].[OrderDiscountAmountExcludeTax],
                                       [Invoice].[OrderDiscountAmountIncludeTax],
                                       [Invoice].[OrderTotalDiscountAmountExcludeTax],
                                       [Invoice].[OrderTotalDiscountAmountIncludeTax],
                                       [Invoice].[OrderPayableAmount],
                                       [Invoice].[Currency],
                                       [Invoice].[InvoiceCreatedOnUtc]
                                   FROM [Invoice]
                                   ORDER BY [Invoice].Id
                                   OFFSET @Skip ROWS
                                   FETCH NEXT @PageSize ROWS ONLY";
                var invoicesEnumerable = await connection.QueryAsync<Invoice>(sqlInvoices, new { request.Skip, request.PageSize });
                var invoicesList = invoicesEnumerable.ToList();

                string sqlInvoiceAddresses = $@"
                                    SELECT DISTINCT
                                       [InvoiceAddress].[Id],
                                       [InvoiceAddress].[Title],
                                       [InvoiceAddress].[FirstName],
                                       [InvoiceAddress].[LastName],
                                       [InvoiceAddress].[CountryName],
                                       [InvoiceAddress].[ProvinceName],
                                       [InvoiceAddress].[CityName],
                                       [InvoiceAddress].[StreetAddress],
                                       [InvoiceAddress].[PostalCode],
                                       [InvoiceAddress].[PhoneNumber],
                                       [InvoiceAddress].[InvoiceId]
                                   FROM [InvoiceAddress]
                                   WHERE [InvoiceAddress].InvoiceId IN (" + (invoicesList.Any() ? string.Join(",", invoicesList.Select(i => i.Id)) : 0.ToString()) + ")";
                var invoiceAddressesEnumerable = await connection.QueryAsync<InvoiceAddress>(sqlInvoiceAddresses);
                var invoiceAddressesList = invoiceAddressesEnumerable.ToList();

                string sqlInvoiceItems = $@"
                                    SELECT DISTINCT
                                       [InvoiceItem].[Id],
                                       [InvoiceItem].[OrderId],
                                       [InvoiceItem].[ProductId],
                                       [InvoiceItem].[ProductName],
                                       [InvoiceItem].[ProductCode],
                                       [InvoiceItem].[ProductUnitPriceExcludeTax],
                                       [InvoiceItem].[ProductUnitPriceIncludeTax],
                                       [InvoiceItem].[ProductUnitDiscountAmountExcludeTax],
                                       [InvoiceItem].[ProductUnitDiscountAmountIncludeTax],
                                       [InvoiceItem].[Quantity],
                                       [InvoiceItem].[InvoiceId]
                                   FROM [InvoiceItem]
                                   WHERE [InvoiceItem].InvoiceId IN (" + (invoicesList.Any() ? string.Join(",", invoicesList.Select(i => i.Id)) : 0.ToString()) + ")";
                var invoiceItemsEnumerable = await connection.QueryAsync<InvoiceItem>(sqlInvoiceItems);
                var invoiceItemsList = invoiceItemsEnumerable.ToList();

                invoicesList.ForEach(i =>
                {
                    var relatedInvoiceAddress = invoiceAddressesList.FirstOrDefault(ia => ia.InvoiceId == i.Id);
                    if (relatedInvoiceAddress != null)
                    {
                        i.InvoiceAddress = relatedInvoiceAddress;
                    }

                    var relatedInvoiceItems = invoiceItemsList.Where(ii => ii.InvoiceId == i.Id).ToList();
                    relatedInvoiceItems.ForEach(ii => i.InvoiceItems.Add(ii));
                });

                try
                {
                    var XXXX = _mapper.Map<List<InvoiceDto>>(invoicesList);
                }
                catch(Exception ex)
                {

                }

                var invoiceDtos = _mapper.Map<List<InvoiceDto>>(invoicesList);
                return invoiceDtos;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }
}
