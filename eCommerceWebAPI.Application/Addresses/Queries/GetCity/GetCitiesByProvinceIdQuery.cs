using eCommerceWebAPI.Application.Configuration.Queries;

namespace eCommerceWebAPI.Application.Addresses.Queries.GetCity
{
    public class GetCitiesByProvinceIdQuery : BasePaginationQuery, IQuery<IList<CityDto>>
    {
        public long ProvinceId { get; set; }
    }
}
