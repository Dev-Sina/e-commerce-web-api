using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using eCommerceWebAPI.Api.SeedWork;
using eCommerceWebAPI.Application.Catalogs.Categories.Queries.GetCategory;
using eCommerceWebAPI.Application.Catalogs.Categories;
using eCommerceWebAPI.Application.Catalogs.Categories.Commands.AddCategory;
using eCommerceWebAPI.Application.Catalogs.Categories.Commands.EditCategory;
using eCommerceWebAPI.Application.Catalogs.Categories.Commands.DeleteCategory;

namespace eCommerceWebAPI.Api.Catalogs.Categories
{
    [Route("api/category")]
    [ApiController]
    public class CategoryController : BaseController
    {
        private readonly ISender _sender;

        public CategoryController(ISender sender) : base(sender)
        {
            _sender = sender;
        }

        /// <summary>
        /// Creates new category
        /// </summary>
        /// <param name="addCategoryDto"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Create([FromBody] AddCategoryDto addCategoryDto)
        {
            try
            {
                var command = new AddCategoryCommand() { AddCategoryDto = addCategoryDto };
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
        /// Modifies a category
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="editCategoryDto"></param>
        /// <returns></returns>
        [HttpPut("{categoryId:long}")]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Edit([FromRoute] long categoryId, [FromBody] EditCategoryDto editCategoryDto)
        {
            try
            {
                var command = new EditCategoryCommand() { CategoryId = categoryId, EditCategoryDto = editCategoryDto };
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
        /// Removes a category
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        [HttpDelete("{categoryId:long}")]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Delete([FromRoute] long categoryId)
        {
            try
            {
                var command = new DeleteCategoryCommand() { CategoryId = categoryId };
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
        /// Get all categories
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("all")]
        [ProducesResponseType(typeof(CustomResponseList<CategoryDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            try
            {
                var query = new GetAllCategoriesQuery() { PageNumber = pageNumber, PageSize = pageSize };
                var queryResult = await _sender.Send(query);

                return Ok(new CustomResponseList<CategoryDto>
                {
                    Status = queryResult.Count > 0 ? "Success" : "Failed",
                    Message = queryResult.Count > 0 ? "All categories has been received successfully." : "No categories found.",
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
