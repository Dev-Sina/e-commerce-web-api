using eCommerceWebAPI.Application.Configuration.Commands;
using eCommerceWebAPI.Application.SeedWork;
using eCommerceWebAPI.Domain;
using eCommerceWebAPI.Domain.Catalogs;
using System.Net;

namespace eCommerceWebAPI.Application.Catalogs.Specifications.Commands.AddSpecification
{
    public class AddSpecificationCommandHandler : ICommandHandler<AddSpecificationCommand, BaseResponse>
    {
        private readonly IRepository<Specification> _specificationRepository;

        public AddSpecificationCommandHandler(IRepository<Specification> specificationRepository)
        {
            _specificationRepository = specificationRepository;
        }

        public async Task<BaseResponse> Handle(AddSpecificationCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(nameof(request));

            var addSpecificationDto = request.AddSpecificationDto;
            ArgumentNullException.ThrowIfNull(nameof(addSpecificationDto));

            BaseResponse response = new();

            // Bad request parameters
            if (string.IsNullOrEmpty(addSpecificationDto.Name?.Trim())) response.Errors.Add("Specification name could not be empty!");
            if (response.Errors.Any())
            {
                response.HttpStatusCode = HttpStatusCode.BadRequest;
                return response;
            };

            // Create specification entity
            Specification specification = new()
            {
                Name = addSpecificationDto.Name!.Trim(),
                DisplayOrder = addSpecificationDto.DisplayOrder
            };

            // Inserting specification into the Db
            var insertedSpecification = await _specificationRepository.Add(specification);

            // Check it's inserted or not
            if (insertedSpecification.Id <= 0)
            {
                response.HttpStatusCode = HttpStatusCode.InternalServerError;
                response.Errors.Add("An error has been occured while inserting specification!");
                return response;
            }

            response.HttpStatusCode = HttpStatusCode.Created;
            response.Message = "The specification has been inserted successfully!";
            return response;
        }
    }
}
