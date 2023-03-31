using eCommerceWebAPI.Application.Configuration.Commands;
using eCommerceWebAPI.Application.SeedWork;
using eCommerceWebAPI.Domain;
using eCommerceWebAPI.Domain.Catalogs;
using System.Net;

namespace eCommerceWebAPI.Application.Catalogs.Specifications.Commands.AddSpecificationValue
{
    public class AddSpecificationValueCommandHandler : ICommandHandler<AddSpecificationValueCommand, BaseResponse>
    {
        private readonly IRepository<Specification> _specificationRepository;
        private readonly IRepository<SpecificationValue> _specificationValueRepository;

        public AddSpecificationValueCommandHandler(IRepository<Specification> specificationRepository,
            IRepository<SpecificationValue> specificationValueRepository)
        {
            _specificationRepository = specificationRepository;
            _specificationValueRepository = specificationValueRepository;
        }

        public async Task<BaseResponse> Handle(AddSpecificationValueCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(nameof(request));

            var addSpecificationValueDto = request.AddSpecificationValueDto;
            ArgumentNullException.ThrowIfNull(nameof(addSpecificationValueDto));

            BaseResponse response = new();

            // Bad request parameters
            if (string.IsNullOrEmpty(addSpecificationValueDto.Name?.Trim())) response.Errors.Add("Specification value name could not be empty!");
            if (response.Errors.Any())
            {
                response.HttpStatusCode = HttpStatusCode.BadRequest;
                return response;
            };

            // Check parent specification would be existed
            var specificationId = addSpecificationValueDto.SpecificationId;
            var specification = await _specificationRepository.Get(specificationId);
            if (specification == null)
            {
                response.Errors.Add($"No specification with the ID of : {specificationId} is found!");
                response.HttpStatusCode = HttpStatusCode.BadRequest;
                return response;
            }

            // Create specification value entity
            SpecificationValue specificationValue = new()
            {
                Name = addSpecificationValueDto.Name!.Trim(),
                DisplayOrder = addSpecificationValueDto.DisplayOrder,
                SpecificationId = specificationId
            };

            // Inserting specification value into the Db
            var insertedSpecificationValue = await _specificationValueRepository.Add(specificationValue);

            // Check it's inserted or not
            if (insertedSpecificationValue.Id <= 0)
            {
                response.HttpStatusCode = HttpStatusCode.InternalServerError;
                response.Errors.Add("An error has been occured while inserting specification value!");
                return response;
            }

            response.HttpStatusCode = HttpStatusCode.Created;
            response.Message = "The specification value has been inserted successfully!";
            return response;
        }
    }
}
