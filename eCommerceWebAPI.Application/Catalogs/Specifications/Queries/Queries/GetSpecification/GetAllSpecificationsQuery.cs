using eCommerceWebAPI.Application.Configuration.Queries;

namespace eCommerceWebAPI.Application.Catalogs.Specifications.Queries.GetSpecification
{
    public class GetAllSpecificationsQuery : BasePaginationQuery, IQuery<IList<SpecificationDto>>
    {
    }
}
