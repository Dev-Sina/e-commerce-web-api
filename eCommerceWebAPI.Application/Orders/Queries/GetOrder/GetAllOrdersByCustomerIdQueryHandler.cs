using AutoMapper;
using eCommerceWebAPI.Application.Configuration;
using eCommerceWebAPI.Application.Configuration.Queries;
using Dapper;
using eCommerceWebAPI.Domain.Addresses;
using eCommerceWebAPI.Domain.Orders;

namespace eCommerceWebAPI.Application.Orders.Queries.GetOrder
{
    public class GetAllOrdersByCustomerIdQueryHandler : IQueryHandler<GetAllOrdersByCustomerIdQuery, IList<OrderDto>>
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;
        private readonly IMapper _mapper;

        public GetAllOrdersByCustomerIdQueryHandler(ISqlConnectionFactory sqlConnectionFactory,
            IMapper mapper)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
            _mapper = mapper;
        }

        public async Task<IList<OrderDto>> Handle(GetAllOrdersByCustomerIdQuery request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));

            try
            {
                var connection = _sqlConnectionFactory.GetOpenConnection();

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
                                       [Order].CustomerId = @CustomerId
                                   ORDER BY [Order].CreatedOnUtc DESC
                                   OFFSET @Skip ROWS
                                   FETCH NEXT @PageSize ROWS ONLY";
                var ordersEnumerable = await connection.QueryAsync<Order>(sqlOrders, new { request.CustomerId, request.Skip, request.PageSize });
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

                ordersList.ForEach(o =>
                {
                    var relatedOrderAddress = orderAddressesList.FirstOrDefault(oa => oa.Id == o.AddressId);
                    if (relatedOrderAddress != null)
                    {
                        o.Address = relatedOrderAddress;
                    }

                    var relatedOrderItems = orderItemsList.Where(oi => oi.OrderId == o.Id).ToList();
                    relatedOrderItems.ForEach(oi => o.OrderItems.Add(oi));
                });

                var orderDtos = _mapper.Map<List<OrderDto>>(ordersList);
                return orderDtos;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }
}
