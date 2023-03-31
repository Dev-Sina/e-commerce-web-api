using eCommerceWebAPI.Application.Configuration.Commands;
using eCommerceWebAPI.Application.SeedWork;
using eCommerceWebAPI.Domain;
using eCommerceWebAPI.Domain.Catalogs;
using System.Net;

namespace eCommerceWebAPI.Application.Catalogs.Categories.Commands.DeleteCategory
{
    public class DeleteCategoryCommandHandler : ICommandHandler<DeleteCategoryCommand, BaseResponse>
    {
        private readonly IRepository<Category> _categoryRepository;

        public DeleteCategoryCommandHandler(IRepository<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<BaseResponse> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(nameof(request));

            BaseResponse response = new();

            // Check removing category would be existed
            var categoryId = request.CategoryId;
            var category = await _categoryRepository.Get(categoryId);
            if (category == null)
            {
                response.Errors.Add($"Category with ID of {categoryId} is not found!");
                response.HttpStatusCode = HttpStatusCode.BadRequest;
                return response;
            }

            // Child categories
            var childCategories = _categoryRepository.AsQueryable().Where(c => c.ParentCategoryId != null && c.ParentCategoryId == categoryId).ToList();
            childCategories.ForEach(c => c.ParentCategoryId = null);

            // Change the parent category of child categories
            await _categoryRepository.UpdateRange(childCategories, false);

            // Hard delete the category without saving changes
            await _categoryRepository.Delete(category, false);

            // Now saving changes
            await _categoryRepository.SaveChangesAsync();

            // Return resopnse
            response.HttpStatusCode = HttpStatusCode.OK;
            response.Message = "The category has been removed successfully!";
            return response;
        }
    }
}
