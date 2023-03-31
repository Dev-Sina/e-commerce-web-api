using eCommerceWebAPI.Application.Configuration.Commands;
using eCommerceWebAPI.Application.SeedWork;

namespace eCommerceWebAPI.Application.Catalogs.Specifications.Commands.AddSpecificationValue
{
    public class AddSpecificationValueCommand : CommandBase<BaseResponse>
    {
        public AddSpecificationValueDto AddSpecificationValueDto { get; set; } = new();
    }
}
