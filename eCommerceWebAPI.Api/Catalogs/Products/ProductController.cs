using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using eCommerceWebAPI.Api.SeedWork;
using eCommerceWebAPI.Application.Catalogs.Products.Commands.AddProduct;
using eCommerceWebAPI.Application.Catalogs.Products.Commands.EditProduct;
using eCommerceWebAPI.Application.Catalogs.Products.Commands.DeleteProduct;
using eCommerceWebAPI.Application.Catalogs.Products.Queries.GetProduct;
using eCommerceWebAPI.Application.Catalogs.Products;
using eCommerceWebAPI.Application.Catalogs.Products.Queries.GetSimilarProduct;

namespace eCommerceWebAPI.Api.Catalogs.Products
{
    [Route("api/product")]
    [ApiController]
    public class ProductController : BaseController
    {
        private readonly ISender _sender;

        public ProductController(ISender sender) : base(sender)
        {
            _sender = sender;
        }

        /// <summary>
        /// Creates new product with its categories and specifications
        /// </summary>
        /// <param name="addProductDto"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Create([FromBody] AddProductDto addProductDto)
        {
            try
            {
                var command = new AddProductCommand() { AddProductDto = addProductDto };
                var commandResult = await _sender.Send(command);

                return StatusCode((int)commandResult.HttpStatusCode, new Response
                {
                    Errors = commandResult.Errors,
                    Status = commandResult.HttpStatusCode == HttpStatusCode.Created ? "Success" : "Failed",
                    Message = commandResult.Message
                });
            }
            catch (ArgumentNullException ex1)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, new Response
                {
                    Errors = new() { ex1.ToString() },
                    Status = "Failed",
                    Message = ex1.Message
                });
            }
            catch (Exception ex2)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new Response
                {
                    Errors = new() { ex2.ToString() },
                    Status = "Failed",
                    Message = ex2.Message
                });
            }
        }

        /// <summary>
        /// Modifies a product with its categories and specifications
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="editProductDto"></param>
        /// <returns></returns>
        [HttpPut("{productId:long}")]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Edit([FromRoute] long productId, [FromBody] EditProductDto editProductDto)
        {
            try
            {
                var command = new EditProductCommand() { ProductId = productId, EditProductDto = editProductDto };
                var commandResult = await _sender.Send(command);

                return StatusCode((int)commandResult.HttpStatusCode, new Response
                {
                    Errors = commandResult.Errors,
                    Status = commandResult.HttpStatusCode == HttpStatusCode.OK ? "Success" : "Failed",
                    Message = commandResult.Message
                });
            }
            catch (ArgumentNullException ex1)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, new Response
                {
                    Errors = new() { ex1.ToString() },
                    Status = "Failed",
                    Message = ex1.Message
                });
            }
            catch (Exception ex2)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new Response
                {
                    Errors = new() { ex2.ToString() },
                    Status = "Failed",
                    Message = ex2.Message
                });
            }
        }

        /// <summary>
        /// Removes a product (Soft delete)
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        [HttpDelete("{productId:long}")]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Delete([FromRoute] long productId)
        {
            try
            {
                var command = new DeleteProductCommand() { ProductId = productId };
                var commandResult = await _sender.Send(command);

                return StatusCode((int)commandResult.HttpStatusCode, new Response
                {
                    Errors = commandResult.Errors,
                    Status = commandResult.HttpStatusCode == HttpStatusCode.OK ? "Success" : "Failed",
                    Message = commandResult.Message
                });
            }
            catch (ArgumentNullException ex1)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, new Response
                {
                    Errors = new() { ex1.ToString() },
                    Status = "Failed",
                    Message = ex1.Message
                });
            }
            catch (Exception ex2)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new Response
                {
                    Errors = new() { ex2.ToString() },
                    Status = "Failed",
                    Message = ex2.Message
                });
            }
        }

        /// <summary>
        /// Get all products
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("all")]
        [ProducesResponseType(typeof(CustomResponseList<ProductDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            try
            {
                var query = new GetAllProductsQuery() { PageNumber = pageNumber, PageSize = pageSize };
                var queryResult = await _sender.Send(query);

                return Ok(new CustomResponseList<ProductDto>
                {
                    Status = queryResult.Count > 0 ? "Success" : "Failed",
                    Message = queryResult.Count > 0 ? "All products has been received successfully." : "No products found.",
                    Count = queryResult.Count,
                    Data = queryResult.ToList()
                });
            }
            catch (ArgumentNullException ex1)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, new Response
                {
                    Errors = new() { ex1.ToString() },
                    Status = "Failed",
                    Message = ex1.Message
                });
            }
            catch (Exception ex2)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new Response
                {
                    Errors = new() { ex2.ToString() },
                    Status = "Failed",
                    Message = ex2.Message
                });
            }
        }

        /// <summary>
        /// Get similar products to a specific product which are in-stock, too
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("similar/{productId:long}")]
        [ProducesResponseType(typeof(CustomResponseList<ProductDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetSimilarProductsByProductId([FromRoute] long productId, [FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            try
            {
                var query = new GetSimilarProductsByProductIdQuery() { ProductId = productId, PageNumber = pageNumber, PageSize = pageSize };
                var queryResult = await _sender.Send(query);

                return Ok(new CustomResponseList<ProductDto>
                {
                    Status = queryResult.Count > 0 ? "Success" : "Failed",
                    Message = queryResult.Count > 0 ? "All similar products has been received successfully." : "No in-stock similar products found.",
                    Count = queryResult.Count,
                    Data = queryResult.ToList()
                });
            }
            catch (ArgumentNullException ex1)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, new Response
                {
                    Errors = new() { ex1.ToString() },
                    Status = "Failed",
                    Message = ex1.Message
                });
            }
            catch (Exception ex2)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new Response
                {
                    Errors = new() { ex2.ToString() },
                    Status = "Failed",
                    Message = ex2.Message
                });
            }
        }
    }
}
