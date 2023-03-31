using eCommerceWebAPI.Application.Configuration.Commands;
using eCommerceWebAPI.Application.SeedWork;

namespace eCommerceWebAPI.Application.Catalogs.Categories.Commands.AddCategory
{
    public class AddCategoryCommand : CommandBase<BaseResponse>
    {
        public AddCategoryDto AddCategoryDto { get; set; } = new();
    }
}
