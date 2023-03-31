using AutoMapper;
using eCommerceWebAPI.Application.Configuration;
using eCommerceWebAPI.Application.Configuration.Queries;
using Dapper;
using eCommerceWebAPI.Domain.Catalogs;

namespace eCommerceWebAPI.Application.Catalogs.Specifications.Queries.GetSpecification
{
    public class GetAllSpecificationsQueryHandler : IQueryHandler<GetAllSpecificationsQuery, IList<SpecificationDto>>
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;
        private readonly IMapper _mapper;

        public GetAllSpecificationsQueryHandler(ISqlConnectionFactory sqlConnectionFactory,
            IMapper mapper)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
            _mapper = mapper;
        }

        public async Task<IList<SpecificationDto>> Handle(GetAllSpecificationsQuery request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));

            try
            {
                var connection = _sqlConnectionFactory.GetOpenConnection();

                const string sqlSpecifications = $@"
                                    SELECT
                                       [Specification].[Id],
                                       [Specification].[Name],
                                       [Specification].[DisplayOrder]
                                   FROM [Specification]
                                   ORDER BY [Specification].DisplayOrder
                                   OFFSET @Skip ROWS
                                   FETCH NEXT @PageSize ROWS ONLY";
                var specificationsEnumerable = await connection.QueryAsync<Specification>(sqlSpecifications, new { request.Skip, request.PageSize });
                var specificationsList = specificationsEnumerable.ToList();

                string sqlSpecificationValues = $@"
                                    SELECT DISTINCT
                                       [SpecificationValue].[Id],
                                       [SpecificationValue].[Name],
                                       [SpecificationValue].[DisplayOrder],
                                       [SpecificationValue].[SpecificationId]
                                   FROM [SpecificationValue]
                                   WHERE [SpecificationValue].SpecificationId IN (" + (specificationsList.Any() ? string.Join(",", specificationsList.Select(s => s.Id)) : 0.ToString()) + ")";
                var specificationValuesEnumerable = await connection.QueryAsync<SpecificationValue>(sqlSpecificationValues);
                var specificationValuesList = specificationValuesEnumerable.ToList();

                specificationsList.ForEach(s =>
                {
                    var relatedSpecificationValues = specificationValuesList.Where(sv => sv.SpecificationId == s.Id).ToList();
                    relatedSpecificationValues.ForEach(s.SpecificationValues.Add);
                });

                var specificationDtos = _mapper.Map<List<SpecificationDto>>(specificationsList);
                return specificationDtos;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }
}
