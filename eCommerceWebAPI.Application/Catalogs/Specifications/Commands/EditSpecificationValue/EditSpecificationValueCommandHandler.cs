using eCommerceWebAPI.Application.Configuration.Commands;
using eCommerceWebAPI.Application.SeedWork;
using eCommerceWebAPI.Domain;
using eCommerceWebAPI.Domain.Catalogs;
using System.Net;

namespace eCommerceWebAPI.Application.Catalogs.Specifications.Commands.EditSpecificationValue
{
    public class EditSpecificationValueCommandHandler : ICommandHandler<EditSpecificationValueCommand, BaseResponse>
    {
        private readonly IRepository<Specification> _specificationRepository;
        private readonly IRepository<SpecificationValue> _specificationValueRepository;

        public EditSpecificationValueCommandHandler(IRepository<Specification> specificationRepository,
            IRepository<SpecificationValue> specificationValueRepository)
        {
            _specificationRepository = specificationRepository;
            _specificationValueRepository = specificationValueRepository;
        }

        public async Task<BaseResponse> Handle(EditSpecificationValueCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(nameof(request));

            var editSpecificationValueDto = request.EditSpecificationValueDto;
            ArgumentNullException.ThrowIfNull(nameof(editSpecificationValueDto));

            BaseResponse response = new();

            // Check requested specification value would be existed
            long specificationValueId = request.SpecificationValueId;
            var specificationValue = await _specificationValueRepository.Get(specificationValueId);
            if (specificationValue == null)
            {
                response.Errors.Add($"The editing specification value with the ID of : {specificationValueId} is not existed!");
                response.HttpStatusCode = HttpStatusCode.BadRequest;
                return response;
            }

            // Check parent specification would be existed
            long specificationId = specificationValue.SpecificationId;
            var specification = await _specificationRepository.Get(specificationId);
            if (specification == null)
            {
                response.Errors.Add($"The editing specification value's specification with the ID of : {specificationValueId} is not existed!");
                response.HttpStatusCode = HttpStatusCode.BadRequest;
                return response;
            }

            // Bad request parameters
            if (string.IsNullOrEmpty(editSpecificationValueDto.Name?.Trim())) response.Errors.Add("Specification value name could not be empty!");
            if (response.Errors.Any())
            {
                response.HttpStatusCode = HttpStatusCode.BadRequest;
                return response;
            };

            // Modify specification value entity
            specificationValue.Name = editSpecificationValueDto.Name!.Trim();
            specificationValue.DisplayOrder = editSpecificationValueDto.DisplayOrder;
            specificationValue.SpecificationId = specificationId;

            // Updating specification value in the Db
            await _specificationValueRepository.Update(specificationValue);

            // Return the result
            response.HttpStatusCode = HttpStatusCode.OK;
            response.Message = "The specification value has been inserted successfully!";
            return response;
        }
    }
}
