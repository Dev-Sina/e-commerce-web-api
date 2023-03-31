using eCommerceWebAPI.Application.Configuration.Queries;
using AutoMapper;
using eCommerceWebAPI.Application.Configuration;
using Dapper;
using eCommerceWebAPI.Domain.Addresses;

namespace eCommerceWebAPI.Application.Addresses.Queries.GetCountry
{
    public class GetAllCountriesQueryHandler : IQueryHandler<GetAllCountriesQuery, IList<CountryDto>>
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;
        private readonly IMapper _mapper;

        public GetAllCountriesQueryHandler(ISqlConnectionFactory sqlConnectionFactory,
            IMapper mapper)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
            _mapper = mapper;
        }

        public async Task<IList<CountryDto>> Handle(GetAllCountriesQuery request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));

            try
            {
                var connection = _sqlConnectionFactory.GetOpenConnection();

                const string sqlCountries = $@"
                                    SELECT
                                       [Country].[Id],
                                       [Country].[Name],
                                       [Country].[DisplayOrder]
                                   FROM [Country]
                                   ORDER BY [Country].DisplayOrder
                                   OFFSET @Skip ROWS
                                   FETCH NEXT @PageSize ROWS ONLY";
                var countriesEnumerable = await connection.QueryAsync<Country>(sqlCountries, new { request.Skip, request.PageSize });
                var countriesList = countriesEnumerable.ToList();

                var countryDtos = _mapper.Map<List<CountryDto>>(countriesList);
                return countryDtos;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }
}
