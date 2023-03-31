using eCommerceWebAPI.Application.Configuration.Commands;
using eCommerceWebAPI.Application.SeedWork;
using eCommerceWebAPI.Domain;
using eCommerceWebAPI.Domain.Catalogs;
using System.Net;

namespace eCommerceWebAPI.Application.Catalogs.Specifications.Commands.EditSpecification
{
    public class EditSpecificationCommandHandler : ICommandHandler<EditSpecificationCommand, BaseResponse>
    {
        private readonly IRepository<Specification> _specificationRepository;

        public EditSpecificationCommandHandler(IRepository<Specification> specificationRepository)
        {
            _specificationRepository = specificationRepository;
        }

        public async Task<BaseResponse> Handle(EditSpecificationCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(nameof(request));

            var editSpecificationDto = request.EditSpecificationDto;
            ArgumentNullException.ThrowIfNull(nameof(editSpecificationDto));

            BaseResponse response = new();

            // Check requested specification would be existed
            long specificationId = request.SpecificationId;
            var specification = await _specificationRepository.Get(specificationId);
            if (specification == null)
            {
                response.Errors.Add($"The editing specification with the ID of : {specificationId} is not existed!");
                response.HttpStatusCode = HttpStatusCode.BadRequest;
                return response;
            }

            // Bad request parameters
            if (string.IsNullOrEmpty(editSpecificationDto.Name?.Trim())) response.Errors.Add("Specification name could not be empty!");
            if (response.Errors.Any())
            {
                response.HttpStatusCode = HttpStatusCode.BadRequest;
                return response;
            };

            // Modify specification entity
            specification.Name = editSpecificationDto.Name!.Trim();
            specification.DisplayOrder = editSpecificationDto.DisplayOrder;

            // Updating specification in the Db
            await _specificationRepository.Update(specification);

            // Return the result
            response.HttpStatusCode = HttpStatusCode.OK;
            response.Message = "The specification has been inserted successfully!";
            return response;
        }
    }
}
