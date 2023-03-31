using eCommerceWebAPI.Application.Configuration.Commands;
using eCommerceWebAPI.Application.SeedWork;

namespace eCommerceWebAPI.Application.Catalogs.Specifications.Commands.DeleteSpecification
{
    public class DeleteSpecificationCommand : CommandBase<BaseResponse>
    {
        public long SpecificationId { get; set; }
    }
}
