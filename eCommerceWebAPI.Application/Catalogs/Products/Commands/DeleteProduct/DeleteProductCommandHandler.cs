using eCommerceWebAPI.Application.Configuration.Commands;
using eCommerceWebAPI.Application.SeedWork;
using eCommerceWebAPI.Domain;
using eCommerceWebAPI.Domain.Catalogs;
using System.Net;

namespace eCommerceWebAPI.Application.Catalogs.Products.Commands.DeleteProduct
{
    public class DeleteProductCommandHandler : ICommandHandler<DeleteProductCommand, BaseResponse>
    {
        private readonly IRepository<Product> _productRepository;

        public DeleteProductCommandHandler(IRepository<Product> productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<BaseResponse> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(nameof(request));

            BaseResponse response = new();

            // Check editing product would be existed
            var productId = request.ProductId;
            var product = await _productRepository.Get(productId);
            if (product == null || product.Deleted)
            {
                response.Errors.Add($"Product with ID of {productId} is not found!");
                response.HttpStatusCode = HttpStatusCode.BadRequest;
                return response;
            }

            // Removing product (Soft delete)
            await _productRepository.Delete(product);

            // Return resopnse
            response.HttpStatusCode = HttpStatusCode.OK;
            response.Message = "The product has been removed successfully!";
            return response;
        }
    }
}
