using eCommerceWebAPI.Application.Configuration.Queries;

namespace eCommerceWebAPI.Application.Catalogs.Categories.Queries.GetCategory
{
    public class GetAllCategoriesQuery : BasePaginationQuery, IQuery<IList<CategoryDto>>
    {
    }
}
