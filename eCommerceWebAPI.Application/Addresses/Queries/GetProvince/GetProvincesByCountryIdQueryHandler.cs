using eCommerceWebAPI.Application.Configuration.Queries;
using AutoMapper;
using eCommerceWebAPI.Application.Configuration;
using Dapper;
using eCommerceWebAPI.Domain.Addresses;

namespace eCommerceWebAPI.Application.Addresses.Queries.GetProvince
{
    public class GetProvincesByCountryIdQueryHandler : IQueryHandler<GetProvincesByCountryIdQuery, IList<ProvinceDto>>
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;
        private readonly IMapper _mapper;

        public GetProvincesByCountryIdQueryHandler(ISqlConnectionFactory sqlConnectionFactory,
            IMapper mapper)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
            _mapper = mapper;
        }

        public async Task<IList<ProvinceDto>> Handle(GetProvincesByCountryIdQuery request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));

            try
            {
                var connection = _sqlConnectionFactory.GetOpenConnection();

                const string sqlProvinces = $@"
                                    SELECT
                                       [Province].[Id],
                                       [Province].[CountryId],
                                       [Province].[Name],
                                       [Province].[DisplayOrder]
                                   FROM [Province]
                                   WHERE [Province].CountryId = @CountryId
                                   ORDER BY [Province].DisplayOrder
                                   OFFSET @Skip ROWS
                                   FETCH NEXT @PageSize ROWS ONLY";
                var provincesEnurable = await connection.QueryAsync<Province>(sqlProvinces, new { request.CountryId, request.Skip, request.PageSize });
                var provincesList = provincesEnurable.ToList();

                var provinceDtos = _mapper.Map<List<ProvinceDto>>(provincesList);
                return provinceDtos;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }
}
