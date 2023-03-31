using eCommerceWebAPI.Application.Configuration.Commands;
using eCommerceWebAPI.Application.SeedWork;

namespace eCommerceWebAPI.Application.Catalogs.Products.Commands.AddProduct
{
    public class AddProductCommand : CommandBase<BaseResponse>
    {
        public AddProductDto AddProductDto { get; set; } = new();
    }
}
