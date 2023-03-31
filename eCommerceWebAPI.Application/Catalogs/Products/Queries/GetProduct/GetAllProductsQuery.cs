using eCommerceWebAPI.Application.Configuration.Queries;

namespace eCommerceWebAPI.Application.Catalogs.Products.Queries.GetProduct
{
    public class GetAllProductsQuery : BasePaginationQuery, IQuery<IList<ProductDto>>
    {
    }
}
