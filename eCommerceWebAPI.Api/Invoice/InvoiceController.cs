using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using eCommerceWebAPI.Api.SeedWork;
using eCommerceWebAPI.Application.Invoices.Commands.AddSystemInvoice;
using eCommerceWebAPI.Application.Invoices.Commands.AddManualInvoice;
using eCommerceWebAPI.Application.Invoices;
using eCommerceWebAPI.Application.Invoices.Queries.GetInvoice;

namespace eCommerceWebAPI.Api.Invoices
{
    [Route("api/invoice")]
    [ApiController]
    public class InvoiceController : BaseController
    {
        private readonly ISender _sender;

        public InvoiceController(ISender sender) : base(sender)
        {
            _sender = sender;
        }

        /// <summary>
        /// Creates new invoice automatically by order
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [HttpPost("automatically/{orderId:long}")]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> CreateInvoiceAutomatically([FromRoute] long orderId)
        {
            try
            {
                var command = new AddSystemInvoiceCommand() { OrderId = orderId };
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
        /// Creates new invoice manually by order
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="addManualInvoiceDto"></param>
        /// <returns></returns>
        [HttpPost("manually/{orderId:long}")]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> CreateInvoiceManually([FromRoute] long orderId, [FromBody] AddManualInvoiceDto addManualInvoiceDto)
        {
            try
            {
                var command = new AddManualInvoiceCommand() { OrderId = orderId, AddManualInvoiceDto = addManualInvoiceDto };
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
            catch (ArgumentException ex2)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, new Response
                {
                    Errors = new() { ex2.ToString() },
                    Status = "Failed",
                    Message = ex2.Message
                });
            }
            catch (Exception ex3)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new Response
                {
                    Errors = new() { ex3.ToString() },
                    Status = "Failed",
                    Message = ex3.Message
                });
            }
        }

        /// <summary>
        /// Gets all invoices
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("all")]
        [ProducesResponseType(typeof(CustomResponseList<InvoiceDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAllInvoices([FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            try
            {
                var query = new GetAllInvoicesQuery() { PageNumber = pageNumber, PageSize = pageSize };
                var queryResult = await _sender.Send(query);

                return Ok(new CustomResponseList<InvoiceDto>
                {
                    Status = queryResult.Count > 0 ? "Success" : "Failed",
                    Message = queryResult.Count > 0 ? "All invoices has been found successfully." : "No invoice found!",
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
