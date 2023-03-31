using eCommerceWebAPI.Application.Configuration.Queries;

namespace eCommerceWebAPI.Application.Addresses.Queries.GetCountry
{
    public class GetAllCountriesQuery : BasePaginationQuery, IQuery<IList<CountryDto>>
    {
    }
}
