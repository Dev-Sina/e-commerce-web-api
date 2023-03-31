using eCommerceWebAPI.Application.Configuration.Commands;
using eCommerceWebAPI.Application.SeedWork;

namespace eCommerceWebAPI.Application.Catalogs.Categories.Commands.DeleteCategory
{
    public class DeleteCategoryCommand : CommandBase<BaseResponse>
    {
        public long CategoryId { get; set; }
    }
}
