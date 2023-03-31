using eCommerceWebAPI.Application.Configuration.Queries;
using AutoMapper;
using eCommerceWebAPI.Application.Configuration;
using Dapper;
using eCommerceWebAPI.Domain.Addresses;

namespace eCommerceWebAPI.Application.Addresses.Queries.GetAddress
{
    public class GetCustomerAddressesByCustomerIdQueryHandler : IQueryHandler<GetCustomerAddressesByCustomerIdQuery, IList<AddressDto>>
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;
        private readonly IMapper _mapper;

        public GetCustomerAddressesByCustomerIdQueryHandler(ISqlConnectionFactory sqlConnectionFactory,
            IMapper mapper)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
            _mapper = mapper;
        }

        public async Task<IList<AddressDto>> Handle(GetCustomerAddressesByCustomerIdQuery request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));

            try
            {
                var connection = _sqlConnectionFactory.GetOpenConnection();

                const string sqlAddresses = $@"
                                    SELECT
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
                                   INNER JOIN [CustomerAddressMapping] ON [Address].Id = [CustomerAddressMapping].AddressId
                                   WHERE [CustomerAddressMapping].CustomerId = @CustomerId
                                   ORDER BY [Address].Id
                                   OFFSET @Skip ROWS
                                   FETCH NEXT @PageSize ROWS ONLY";
                var addressesEnurable = await connection.QueryAsync<Address>(sqlAddresses, new { request.CustomerId, request.Skip, request.PageSize });
                var addressesList = addressesEnurable.ToList();

                var addressDtos = _mapper.Map<List<AddressDto>>(addressesList);
                return addressDtos;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }
}
