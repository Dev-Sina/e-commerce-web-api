using eCommerceWebAPI.Application.Configuration.Queries;
using AutoMapper;
using eCommerceWebAPI.Application.Configuration;
using Dapper;
using eCommerceWebAPI.Domain.Addresses;

namespace eCommerceWebAPI.Application.Addresses.Queries.GetCity
{
    public class GetCitiesByProvinceIdQueryHandler : IQueryHandler<GetCitiesByProvinceIdQuery, IList<CityDto>>
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;
        private readonly IMapper _mapper;

        public GetCitiesByProvinceIdQueryHandler(ISqlConnectionFactory sqlConnectionFactory,
            IMapper mapper)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
            _mapper = mapper;
        }

        public async Task<IList<CityDto>> Handle(GetCitiesByProvinceIdQuery request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));

            try
            {
                var connection = _sqlConnectionFactory.GetOpenConnection();

                const string sqlCities = $@"
                                    SELECT
                                       [City].[Id],
                                       [City].[ProvinceId],
                                       [City].[Name],
                                       [City].[DisplayOrder]
                                   FROM [City]
                                   WHERE [City].ProvinceId = @ProvinceId
                                   ORDER BY [City].DisplayOrder
                                   OFFSET @Skip ROWS
                                   FETCH NEXT @PageSize ROWS ONLY";
                var citiesEnurable = await connection.QueryAsync<City>(sqlCities, new { request.ProvinceId, request.Skip, request.PageSize });
                var citiesList = citiesEnurable.ToList();

                var cityDtos = _mapper.Map<List<CityDto>>(citiesList);
                return cityDtos;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }
}
