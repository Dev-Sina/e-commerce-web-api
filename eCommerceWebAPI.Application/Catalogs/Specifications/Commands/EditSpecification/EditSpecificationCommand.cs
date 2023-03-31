using eCommerceWebAPI.Application.Configuration.Commands;
using eCommerceWebAPI.Application.SeedWork;

namespace eCommerceWebAPI.Application.Catalogs.Specifications.Commands.EditSpecification
{
    public class EditSpecificationCommand : CommandBase<BaseResponse>
    {
        public long SpecificationId { get; set; }

        public EditSpecificationDto EditSpecificationDto { get; set; } = new();
    }
}
