using eCommerceWebAPI.Application.Configuration.Commands;
using eCommerceWebAPI.Application.SeedWork;
using eCommerceWebAPI.Domain;
using eCommerceWebAPI.Domain.Catalogs;
using System.Net;

namespace eCommerceWebAPI.Application.Catalogs.Specifications.Commands.DeleteSpecificationValue
{
    public class DeleteSpecificationValueCommandHandler : ICommandHandler<DeleteSpecificationValueCommand, BaseResponse>
    {
        private readonly IRepository<SpecificationValue> _specificationValueRepository;
        private readonly IRepository<ProductSpecificationValueMapping> _productSpecificationValueMappingRepository;

        public DeleteSpecificationValueCommandHandler(IRepository<SpecificationValue> specificationValueRepository,
            IRepository<ProductSpecificationValueMapping> productSpecificationValueMappingRepository)
        {
            _specificationValueRepository = specificationValueRepository;
            _productSpecificationValueMappingRepository = productSpecificationValueMappingRepository;
        }

        public async Task<BaseResponse> Handle(DeleteSpecificationValueCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(nameof(request));

            BaseResponse response = new();

            // Check removing specification value would be existed
            var specificationValueId = request.SpecificationValueId;
            var specificationValue = await _specificationValueRepository.Get(specificationValueId);
            if (specificationValue == null)
            {
                response.Errors.Add($"Specification value with ID of {specificationValueId} is not found!");
                response.HttpStatusCode = HttpStatusCode.BadRequest;
                return response;
            }

            // Check related product specification value mappings
            var relatedProductSpecificationValueMappings = _productSpecificationValueMappingRepository
                    .AsQueryable()
                    .Where(psvm => psvm.SpecificationValueId == specificationValueId)
                    .ToList();

            // Hard delete the related product specification value mappings without saving changes
            relatedProductSpecificationValueMappings.ForEach(async relatedProductSpecificationValueMapping => await _productSpecificationValueMappingRepository.Delete(relatedProductSpecificationValueMapping, false));

            // Hard delete the specification value without saving changes
            await _specificationValueRepository.Delete(specificationValue, false);

            // Now saving changes
            await _specificationValueRepository.SaveChangesAsync();

            // Return resopnse
            response.HttpStatusCode = HttpStatusCode.OK;
            response.Message = "The spoecification value has been removed successfully!";
            return response;
        }
    }
}
