using eCommerceWebAPI.Application.Configuration.Queries;

namespace eCommerceWebAPI.Application.Addresses.Queries.GetProvince
{
    public class GetProvincesByCountryIdQuery : BasePaginationQuery, IQuery<IList<ProvinceDto>>
    {
        public long CountryId { get; set; }
    }
}
