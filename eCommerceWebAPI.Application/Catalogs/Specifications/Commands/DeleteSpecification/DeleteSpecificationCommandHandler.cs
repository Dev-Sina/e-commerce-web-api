using eCommerceWebAPI.Application.Configuration.Commands;
using eCommerceWebAPI.Application.SeedWork;
using eCommerceWebAPI.Domain;
using eCommerceWebAPI.Domain.Catalogs;
using System.Net;

namespace eCommerceWebAPI.Application.Catalogs.Specifications.Commands.DeleteSpecification
{
    public class DeleteSpecificationCommandHandler : ICommandHandler<DeleteSpecificationCommand, BaseResponse>
    {
        private readonly IRepository<Specification> _specificationRepository;
        private readonly IRepository<SpecificationValue> _specificationValueRepository;
        private readonly IRepository<ProductSpecificationValueMapping> _productSpecificationValueMappingRepository;

        public DeleteSpecificationCommandHandler(IRepository<Specification> specificationRepository,
            IRepository<SpecificationValue> specificationValueRepository,
            IRepository<ProductSpecificationValueMapping> productSpecificationValueMappingRepository)
        {
            _specificationRepository = specificationRepository;
            _specificationValueRepository = specificationValueRepository;
            _productSpecificationValueMappingRepository = productSpecificationValueMappingRepository;
        }

        public async Task<BaseResponse> Handle(DeleteSpecificationCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(nameof(request));

            BaseResponse response = new();

            // Check removing specification would be existed
            var specificationId = request.SpecificationId;
            var specification = await _specificationRepository.Get(specificationId);
            if (specification == null)
            {
                response.Errors.Add($"Specification with ID of {specificationId} is not found!");
                response.HttpStatusCode = HttpStatusCode.BadRequest;
                return response;
            }

            // Check related specification values
            var relatedSpecificationValues = _specificationValueRepository.AsQueryable().Where(sv => sv.SpecificationId == specificationId).ToList();

            // Check related product specification value mappings
            var relatedProductSpecificationValueMappings = _productSpecificationValueMappingRepository
                    .AsQueryable()
                    .Where(psvm => relatedSpecificationValues.Select(sv => sv.Id).Contains(psvm.SpecificationValueId))
                    .ToList();

            // Hard delete the related product specification value mappings without saving changes
            relatedProductSpecificationValueMappings.ForEach(async relatedProductSpecificationValueMapping => await _productSpecificationValueMappingRepository.Delete(relatedProductSpecificationValueMapping, false));

            // Hard delete the related specification values without saving changes
            relatedSpecificationValues.ForEach(async relatedSpecificationValue => await _specificationValueRepository.Delete(relatedSpecificationValue, false));

            // Hard delete the specification without saving changes
            await _specificationRepository.Delete(specification, false);

            // Now saving changes
            await _specificationRepository.SaveChangesAsync();

            // Return resopnse
            response.HttpStatusCode = HttpStatusCode.OK;
            response.Message = "The spoecification has been removed successfully!";
            return response;
        }
    }
}
