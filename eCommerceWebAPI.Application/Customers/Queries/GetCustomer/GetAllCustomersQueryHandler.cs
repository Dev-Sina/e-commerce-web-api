using AutoMapper;
using eCommerceWebAPI.Application.Configuration;
using eCommerceWebAPI.Application.Configuration.Queries;
using Dapper;
using eCommerceWebAPI.Domain.Customers;
using eCommerceWebAPI.Domain.Addresses;
using eCommerceWebAPI.Domain.Orders;

namespace eCommerceWebAPI.Application.Customers.Queries.GetCustomer
{
    public class GetAllCustomersQueryHandler : IQueryHandler<GetAllCustomersQuery, IList<CustomerDto>>
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;
        private readonly IMapper _mapper;

        public GetAllCustomersQueryHandler(ISqlConnectionFactory sqlConnectionFactory,
            IMapper mapper)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
            _mapper = mapper;
        }

        public async Task<IList<CustomerDto>> Handle(GetAllCustomersQuery request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));

            try
            {
                var connection = _sqlConnectionFactory.GetOpenConnection();

                const string sqlCustomers = $@"
                                    SELECT
                                       [Customer].[Id],
                                       [Customer].[FirstName],
                                       [Customer].[LastName],
                                       [Customer].[NationalCode],
                                       [Customer].[CreatedOnUtc]
                                   FROM [Customer]
                                   WHERE [Customer].Deleted = 0
                                   ORDER BY [Customer].Id
                                   OFFSET @Skip ROWS
                                   FETCH NEXT @PageSize ROWS ONLY";
                var customersEnumerable = await connection.QueryAsync<Customer>(sqlCustomers, new { request.Skip, request.PageSize });
                var customersList = customersEnumerable.ToList();

                string sqlCustomerAddressMappings = $@"
                                    SELECT DISTINCT
                                       [CustomerAddressMapping].[Id],
                                       [CustomerAddressMapping].[CustomerId],
                                       [CustomerAddressMapping].[AddressId]
                                   FROM [CustomerAddressMapping]
                                   WHERE [CustomerAddressMapping].CustomerId IN (" + (customersList.Any() ? string.Join(",", customersList.Select(c => c.Id)) : 0.ToString()) + ")";
                var customerAddressMappingsEnumerable = await connection.QueryAsync<CustomerAddressMapping>(sqlCustomerAddressMappings);
                var customerAddressMappingsList = customerAddressMappingsEnumerable.ToList();

                string sqlAddresses = $@"
                                    SELECT DISTINCT
                                       [Address].[Id],
                                       [Address].[Title],
                                       [Address].[FirstName],
                                       [Address].[LastName],
                                       [Address].[CountryName],
                                       [Address].[ProvinceName],
                                       [Address].[CityName],
                                       [Address].[StreetAddress],
                                       [Address].[PostalCode],
                                       [Address].[PhoneNumber]
                                   FROM [Address]
                                   WHERE [Address].Id IN (" + (customerAddressMappingsList.Any() ? string.Join(",", customerAddressMappingsList.Select(cam => cam.AddressId)) : 0.ToString()) + ")";
                var addressesEnumerable = await connection.QueryAsync<Address>(sqlAddresses);
                var addressesList = addressesEnumerable.ToList();

                string sqlOrders = $@"
                                    SELECT DISTINCT
                                       [Order].[Id],
                                       [Order].[CustomerId],
                                       [Order].[CreatedOnUtc],
                                       [Order].[ShippingCostExcludeTax],
                                       [Order].[ShippingCostIncludeTax],
                                       [Order].[OrderDiscountAmountExcludeTax],
                                       [Order].[OrderDiscountAmountIncludeTax],
                                       [Order].[Currency],
                                       [Order].[AddressId]
                                   FROM [Order]
                                   WHERE
                                       [Order].Deleted = 0 AND
                                       [Order].CustomerId IN (" + (customersList.Any() ? string.Join(",", customersList.Select(c => c.Id)) : 0.ToString()) + ")";
                var ordersEnumerable = await connection.QueryAsync<Order>(sqlOrders);
                var ordersList = ordersEnumerable.ToList();

                string sqlOrderAddressess = $@"
                                    SELECT DISTINCT
                                       [Address].[Id],
                                       [Address].[Title],
                                       [Address].[FirstName],
                                       [Address].[LastName],
                                       [Address].[CountryName],
                                       [Address].[ProvinceName],
                                       [Address].[CityName],
                                       [Address].[StreetAddress],
                                       [Address].[PostalCode],
                                       [Address].[PhoneNumber]
                                   FROM [Address]
                                   WHERE [Address].Id IN (" + (ordersList.Any() ? string.Join(",", ordersList.Select(o => o.AddressId)) : 0.ToString()) + ")";
                var orderAddressesEnumerable = await connection.QueryAsync<Address>(sqlOrderAddressess);
                var orderAddressesList = orderAddressesEnumerable.ToList();

                string sqlOrderItems = $@"
                                    SELECT DISTINCT
                                       [OrderItem].[Id],
                                       [OrderItem].[OrderId],
                                       [OrderItem].[ProductId],
                                       [OrderItem].[ProductName],
                                       [OrderItem].[ProductCode],
                                       [OrderItem].[ProductUnitPriceExcludeTax],
                                       [OrderItem].[ProductUnitPriceIncludeTax],
                                       [OrderItem].[ProductUnitDiscountAmountExcludeTax],
                                       [OrderItem].[ProductUnitDiscountAmountIncludeTax],
                                       [OrderItem].[Quantity]
                                   FROM [OrderItem]
                                   WHERE [OrderItem].OrderId IN (" + (ordersList.Any() ? string.Join(",", ordersList.Select(o => o.Id)) : 0.ToString()) + ")";
                var orderItemsEnumerable = await connection.QueryAsync<OrderItem>(sqlOrderItems);
                var orderItemsList = orderItemsEnumerable.ToList();

                customersList.ForEach(c =>
                {
                    var relatedCustomerAddressMappings = customerAddressMappingsList.Where(cam => cam.CustomerId == c.Id).ToList();
                    var relatedAddresses = addressesList.Where(a => relatedCustomerAddressMappings.Select(cam => cam.AddressId).Contains(a.Id)).ToList();
                    relatedCustomerAddressMappings.ForEach(cam =>
                    {
                        var relatedAddress = relatedAddresses.FirstOrDefault(a => cam.AddressId == a.Id);
                        if (relatedAddress != null)
                        {
                            c.CustomerAddressMappings.Add(new CustomerAddressMapping { Id = cam.Id, CustomerId = c.Id, AddressId = relatedAddress.Id, Address = relatedAddress });
                        }
                    });

                    var relatedCustomerOrders = ordersList.Where(o => o.CustomerId == c.Id).ToList();
                    relatedCustomerOrders.ForEach(o =>
                    {
                        var relatedOrderAddress = orderAddressesList.FirstOrDefault(oa => oa.Id == o.AddressId);
                        if (relatedOrderAddress != null)
                        {
                            o.Address = relatedOrderAddress;
                        }

                        var relatedOrderItems = orderItemsList.Where(oi => oi.OrderId == o.Id).ToList();
                        relatedOrderItems.ForEach(oi => o.OrderItems.Add(oi));

                        c.Orders.Add(o);
                    });
                });

                var customerDtos = _mapper.Map<List<CustomerDto>>(customersList);
                return customerDtos;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }
}
