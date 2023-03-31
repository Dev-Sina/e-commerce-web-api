using eCommerceWebAPI.Application.Configuration.Commands;
using eCommerceWebAPI.Application.SeedWork;

namespace eCommerceWebAPI.Application.Catalogs.Products.Commands.DeleteProduct
{
    public class DeleteProductCommand : CommandBase<BaseResponse>
    {
        public long ProductId { get; set; }
    }
}
