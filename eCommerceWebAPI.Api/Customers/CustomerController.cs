using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using eCommerceWebAPI.Api.SeedWork;
using eCommerceWebAPI.Application.Customers.Commands.AddCustomer;
using eCommerceWebAPI.Application.Customers.Commands.EditCustomer;
using eCommerceWebAPI.Application.Customers.Queries.GetCustomer;
using eCommerceWebAPI.Application.Customers;

namespace eCommerceWebAPI.Api.Customers
{
    [Route("api/customer")]
    [ApiController]
    public class CustomerController : BaseController
    {
        private readonly ISender _sender;

        public CustomerController(ISender sender) : base(sender)
        {
            _sender = sender;
        }

        /// <summary>
        /// Creates new customer
        /// </summary>
        /// <param name="addCustomerDto"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> CreateCustomer([FromBody] AddCustomerDto addCustomerDto)
        {
            try
            {
                var command = new AddCustomerCommand() { AddCustomerDto = addCustomerDto };
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
        /// Modifies a customer info
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="editCustomerDto"></param>
        /// <returns></returns>
        [HttpPut("{customerId:long}")]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> EditCustomer([FromRoute] long customerId, [FromBody] EditCustomerDto editCustomerDto)
        {
            try
            {
                var command = new EditCustomerCommand() { CustomerId = customerId, EditCustomerDto = editCustomerDto };
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
        /// Gets all customers info
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("all")]
        [ProducesResponseType(typeof(CustomResponseList<CustomerDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAllCustomers([FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            try
            {
                var query = new GetAllCustomersQuery() { PageNumber = pageNumber, PageSize = pageSize };
                var queryResult = await _sender.Send(query);

                return Ok(new CustomResponseList<CustomerDto>
                {
                    Status = queryResult.Count > 0 ? "Success" : "Failed",
                    Message = queryResult.Count > 0 ? "All customers info has been found successfully." : "No customers found!",
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
