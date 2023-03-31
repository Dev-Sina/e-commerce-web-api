using eCommerceWebAPI.Application.Configuration.Commands;
using eCommerceWebAPI.Application.SeedWork;

namespace eCommerceWebAPI.Application.Catalogs.Specifications.Commands.DeleteSpecificationValue
{
    public class DeleteSpecificationValueCommand : CommandBase<BaseResponse>
    {
        public long SpecificationValueId { get; set; }
    }
}
