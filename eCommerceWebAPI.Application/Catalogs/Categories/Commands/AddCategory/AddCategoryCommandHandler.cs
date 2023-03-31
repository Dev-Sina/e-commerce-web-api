using eCommerceWebAPI.Application.Configuration.Commands;
using eCommerceWebAPI.Application.SeedWork;
using eCommerceWebAPI.Domain;
using eCommerceWebAPI.Domain.Catalogs;
using System.Net;

namespace eCommerceWebAPI.Application.Catalogs.Categories.Commands.AddCategory
{
    public class AddCategoryCommandHandler : ICommandHandler<AddCategoryCommand, BaseResponse>
    {
        private readonly IRepository<Category> _categoryRepository;

        public AddCategoryCommandHandler(IRepository<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<BaseResponse> Handle(AddCategoryCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(nameof(request));

            var addCategoryDto = request.AddCategoryDto;
            ArgumentNullException.ThrowIfNull(nameof(addCategoryDto));

            BaseResponse response = new();

            // Bad request parameters
            if (string.IsNullOrEmpty(addCategoryDto.Name?.Trim())) response.Errors.Add("Category name could not be empty!");
            if (response.Errors.Any())
            {
                response.HttpStatusCode = HttpStatusCode.BadRequest;
                return response;
            };

            // Check parent category
            if (addCategoryDto.ParentCategoryId != null)
            {
                if (addCategoryDto.ParentCategoryId <= 0)
                {
                    addCategoryDto.ParentCategoryId = null;
                }
                else
                {
                    var parentCategory = await _categoryRepository.Get(addCategoryDto.ParentCategoryId.Value);
                    if (parentCategory == null)
                    {
                        response.Errors.Add("Requested parent category is not existed.");
                        response.HttpStatusCode = HttpStatusCode.BadRequest;
                        return response;
                    }
                }
            }

            // Create category entity
            Category category = new()
            {
                Name = addCategoryDto.Name!.Trim(),
                Description = addCategoryDto.Description?.Trim(),
                DisplayOrder = addCategoryDto.DisplayOrder,
                ParentCategoryId = addCategoryDto.ParentCategoryId
            };

            // Inserting category into the Db
            var insertedCategory = await _categoryRepository.Add(category);

            // Check it's inserted or not
            if (insertedCategory.Id <= 0)
            {
                response.HttpStatusCode = HttpStatusCode.InternalServerError;
                response.Errors.Add("An error has been occured while inserting category!");
                return response;
            }

            response.HttpStatusCode = HttpStatusCode.Created;
            response.Message = "The category has been inserted successfully!";
            return response;
        }
    }
}
