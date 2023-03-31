using eCommerceWebAPI.Application.Catalogs.Categories.Commands.EditCategory;
using eCommerceWebAPI.Application.Configuration.Commands;
using eCommerceWebAPI.Application.SeedWork;
using eCommerceWebAPI.Domain;
using eCommerceWebAPI.Domain.Catalogs;
using System.Net;

namespace eCommerceWebAPI.Application.Catalogs.Products.Commands.EditProduct
{
    public class EditCategoryCommandHandler : ICommandHandler<EditCategoryCommand, BaseResponse>
    {
        private readonly IRepository<Category> _categoryRepository;

        public EditCategoryCommandHandler(IRepository<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<BaseResponse> Handle(EditCategoryCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(nameof(request));

            var editCategoryDto = request.EditCategoryDto;
            ArgumentNullException.ThrowIfNull(nameof(editCategoryDto));

            BaseResponse response = new();

            // Check category would be existed
            var category = await _categoryRepository.Get(request.CategoryId);
            if (category == null)
            {
                response.Errors.Add($"Requested category with ID of {request.CategoryId} is not existed!");
                response.HttpStatusCode = HttpStatusCode.BadRequest;
                return response;
            }

            // Bad request parameters
            if (string.IsNullOrEmpty(editCategoryDto.Name?.Trim())) response.Errors.Add("Category name could not be empty!");
            if (response.Errors.Any())
            {
                response.HttpStatusCode = HttpStatusCode.BadRequest;
                return response;
            };

            // Check parent category
            if (editCategoryDto.ParentCategoryId != null)
            {
                if (editCategoryDto.ParentCategoryId <= 0)
                {
                    editCategoryDto.ParentCategoryId = null;
                }
                else
                {
                    var parentCategory = await _categoryRepository.Get(editCategoryDto.ParentCategoryId.Value);
                    if (parentCategory == null)
                    {
                        response.Errors.Add("Requested parent category is not existed.");
                        response.HttpStatusCode = HttpStatusCode.BadRequest;
                        return response;
                    }
                }
            }

            // Mofigy category entity
            category.Name = editCategoryDto.Name!.Trim();
            category.Description = editCategoryDto.Description?.Trim();
            category.DisplayOrder = editCategoryDto.DisplayOrder;
            category.ParentCategoryId = editCategoryDto.ParentCategoryId;

            // Updating the category in Db
            await _categoryRepository.Update(category);

            response.HttpStatusCode = HttpStatusCode.OK;
            response.Message = "The category has been modified successfully!";
            return response;
        }
    }
}
