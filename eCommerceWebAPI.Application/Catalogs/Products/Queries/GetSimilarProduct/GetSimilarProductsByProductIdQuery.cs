using eCommerceWebAPI.Application.Configuration.Queries;

namespace eCommerceWebAPI.Application.Catalogs.Products.Queries.GetSimilarProduct
{
    public class GetSimilarProductsByProductIdQuery : BasePaginationQuery, IQuery<IList<ProductDto>>
    {
        public long ProductId { get; set; }
    }
}
