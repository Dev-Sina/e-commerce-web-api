using eCommerceWebAPI.Application.Configuration.Commands;
using eCommerceWebAPI.Application.SeedWork;

namespace eCommerceWebAPI.Application.Catalogs.Specifications.Commands.AddSpecification
{
    public class AddSpecificationCommand : CommandBase<BaseResponse>
    {
        public AddSpecificationDto AddSpecificationDto { get; set; } = new();
    }
}
