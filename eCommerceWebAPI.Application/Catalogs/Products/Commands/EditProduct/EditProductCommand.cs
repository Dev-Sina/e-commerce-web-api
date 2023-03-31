using eCommerceWebAPI.Application.Configuration.Commands;
using eCommerceWebAPI.Application.SeedWork;

namespace eCommerceWebAPI.Application.Catalogs.Products.Commands.EditProduct
{
    public class EditProductCommand : CommandBase<BaseResponse>
    {
        public long ProductId { get; set; }
        public EditProductDto EditProductDto { get; set; } = new();
    }
}
