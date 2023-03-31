using eCommerceWebAPI.Application.Configuration.Commands;
using eCommerceWebAPI.Application.SeedWork;

namespace eCommerceWebAPI.Application.Catalogs.Specifications.Commands.EditSpecificationValue
{
    public class EditSpecificationValueCommand : CommandBase<BaseResponse>
    {
        public long SpecificationValueId { get; set; }

        public EditSpecificationValueDto EditSpecificationValueDto { get; set; } = new();
    }
}
