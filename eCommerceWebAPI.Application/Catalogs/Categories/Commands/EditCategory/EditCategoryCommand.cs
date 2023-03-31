using eCommerceWebAPI.Application.Configuration.Commands;
using eCommerceWebAPI.Application.SeedWork;

namespace eCommerceWebAPI.Application.Catalogs.Categories.Commands.EditCategory
{
    public class EditCategoryCommand : CommandBase<BaseResponse>
    {
        public long CategoryId { get; set; }
        public EditCategoryDto EditCategoryDto { get; set; } = new();
    }
}
