using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using eCommerceWebAPI.Api.SeedWork;
using eCommerceWebAPI.Application.Orders.Commands.AddOrder;
using eCommerceWebAPI.Application.Orders;
using eCommerceWebAPI.Application.Orders.Queries.GetOrder;

namespace eCommerceWebAPI.Api.Orders
{
    [Route("api/order")]
    [ApiController]
    public class OrderController : BaseController
    {
        private readonly ISender _sender;

        public OrderController(ISender sender) : base(sender)
        {
            _sender = sender;
        }

        /// <summary>
        /// Places new order
        /// </summary>
        /// <param name="addOrderDto"></param>
        /// <returns></returns>
        [HttpPost("{customerId:long}")]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> PlaceOrder([FromRoute] long customerId, [FromBody] AddOrderDto addOrderDto)
        {
            try
            {
                var command = new AddOrderCommand() { CustomerId = customerId, AddOrderDto = addOrderDto };
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
        /// Get all orders of a customer
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("{customerId:long}")]
        [ProducesResponseType(typeof(CustomResponseList<OrderDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAll([FromRoute] long customerId, [FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            try
            {
                var query = new GetAllOrdersByCustomerIdQuery() { CustomerId = customerId, PageNumber = pageNumber, PageSize = pageSize };
                var queryResult = await _sender.Send(query);

                return Ok(new CustomResponseList<OrderDto>
                {
                    Status = "Success",
                    Message = queryResult.Count > 0 ? "All orders has been received successfully." : "No orders found.",
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
