using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using eCommerceWebAPI.Api.SeedWork;
using eCommerceWebAPI.Application.Catalogs.Specifications;
using eCommerceWebAPI.Application.Catalogs.Specifications.Queries.GetSpecification;
using eCommerceWebAPI.Application.Catalogs.Specifications.Commands.AddSpecification;
using eCommerceWebAPI.Application.Catalogs.Specifications.Commands.EditSpecification;
using eCommerceWebAPI.Application.Catalogs.Specifications.Commands.DeleteSpecification;
using eCommerceWebAPI.Application.Catalogs.Specifications.Commands.AddSpecificationValue;
using eCommerceWebAPI.Application.Catalogs.Specifications.Commands.EditSpecificationValue;
using eCommerceWebAPI.Application.Catalogs.Specifications.Commands.DeleteSpecificationValue;

namespace eCommerceWebAPI.Api.Catalogs.Specifications
{
    [Route("api/specification")]
    [ApiController]
    public class SpecificationController : BaseController
    {
        private readonly ISender _sender;

        public SpecificationController(ISender sender) : base(sender)
        {
            _sender = sender;
        }

        /// <summary>
        /// Creates new specification
        /// </summary>
        /// <param name="addSpecificationDto"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Create([FromBody] AddSpecificationDto addSpecificationDto)
        {
            try
            {
                var command = new AddSpecificationCommand() { AddSpecificationDto = addSpecificationDto };
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
        /// Modifies a specification
        /// </summary>
        /// <param name="specificationId"></param>
        /// <param name="editSpecificationDto"></param>
        /// <returns></returns>
        [HttpPut("{specificationId:long}")]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Edit([FromRoute] long specificationId, [FromBody] EditSpecificationDto editSpecificationDto)
        {
            try
            {
                var command = new EditSpecificationCommand() { SpecificationId = specificationId, EditSpecificationDto = editSpecificationDto };
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
        /// Removes a specification
        /// </summary>
        /// <param name="specificationId"></param>
        /// <returns></returns>
        [HttpDelete("{specificationId:long}")]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Delete([FromRoute] long specificationId)
        {
            try
            {
                var command = new DeleteSpecificationCommand() { SpecificationId = specificationId };
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
        /// Creates new specification value
        /// </summary>
        /// <param name="addSpecificationValueDto"></param>
        /// <returns></returns>
        [HttpPost("value")]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> CreateSpecificationValue([FromBody] AddSpecificationValueDto addSpecificationValueDto)
        {
            try
            {
                var command = new AddSpecificationValueCommand() { AddSpecificationValueDto = addSpecificationValueDto };
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
        /// Modifies a specification value
        /// </summary>
        /// <param name="specificationValueId"></param>
        /// <param name="editSpecificationValueDto"></param>
        /// <returns></returns>
        [HttpPut("value/{specificationValueId:long}")]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> EditSpecificationValue([FromRoute] long specificationValueId, [FromBody] EditSpecificationValueDto editSpecificationValueDto)
        {
            try
            {
                var command = new EditSpecificationValueCommand() { SpecificationValueId = specificationValueId, EditSpecificationValueDto = editSpecificationValueDto };
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
        /// Removes a specification value
        /// </summary>
        /// <param name="specificationValueId"></param>
        /// <returns></returns>
        [HttpDelete("value/{specificationValueId:long}")]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteSpecificationValue([FromRoute] long specificationValueId)
        {
            try
            {
                var command = new DeleteSpecificationValueCommand() { SpecificationValueId = specificationValueId };
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
        /// Get all specifications
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("all")]
        [ProducesResponseType(typeof(CustomResponseList<SpecificationDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            try
            {
                var query = new GetAllSpecificationsQuery() { PageNumber = pageNumber, PageSize = pageSize };
                var queryResult = await _sender.Send(query);

                return Ok(new CustomResponseList<SpecificationDto>
                {
                    Status = queryResult.Count > 0 ? "Success" : "Failed",
                    Message = queryResult.Count > 0 ? "All specifications has been received successfully." : "No specifications found.",
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
