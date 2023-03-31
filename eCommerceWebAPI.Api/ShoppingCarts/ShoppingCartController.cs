using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using eCommerceWebAPI.Api.SeedWork;
using eCommerceWebAPI.Application.ShoppingCarts;
using eCommerceWebAPI.Application.ShoppingCarts.Queries.GetShoppingCartItem;
using eCommerceWebAPI.Application.ShoppingCarts.Commands.UpdateShoppingCartItem;
using eCommerceWebAPI.Application.ShoppingCarts.Commands.DeleteShoppingCartItem;

namespace eCommerceWebAPI.Api.ShoppingCarts
{
    [Route("api/cart")]
    [ApiController]
    public class ShoppingCartController : BaseController
    {
        private readonly ISender _sender;

        public ShoppingCartController(ISender sender) : base(sender)
        {
            _sender = sender;
        }

        /// <summary>
        /// Adds/Update/Delete customer's shopping cart item
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="updateShoppingCartItemDto"></param>
        /// <returns></returns>
        [HttpPost("{customerId:long}")]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UpdateShoppingCartItem([FromRoute] long customerId, [FromBody] UpdateShoppingCartItemDto updateShoppingCartItemDto)
        {
            try
            {
                var command = new UpdateShoppingCartItemCommand() { CustomerId = customerId, UpdateShoppingCartItemDto = updateShoppingCartItemDto };
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
        /// Delete item from customer's shopping cart
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="deleteShoppingCartItemDto"></param>
        /// <returns></returns>
        [HttpDelete("{customerId:long}")]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteShoppingCartItem([FromRoute] long customerId, [FromBody] DeleteShoppingCartItemDto deleteShoppingCartItemDto)
        {
            try
            {
                UpdateShoppingCartItemDto updateShoppingCartItemDto = new() { ProductId = deleteShoppingCartItemDto.ProductId, Quantity = int.MinValue };
                var command = new UpdateShoppingCartItemCommand() { CustomerId = customerId, UpdateShoppingCartItemDto = updateShoppingCartItemDto };
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
        /// Get shopping cart of a specific customer
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        [HttpGet("{customerId:long}")]
        [ProducesResponseType(typeof(CustomResponseList<ShoppingCartItemDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAll([FromRoute] long customerId)
        {
            try
            {
                var query = new GetShoppingCartItemsByCustomerIdQuery() { CustomerId = customerId };
                var queryResult = await _sender.Send(query);

                return Ok(new CustomResponseList<ShoppingCartItemDto>
                {
                    Status = "Success",
                    Message = queryResult.Count > 0 ? "All cart items has been received successfully." : "The cart is empty now.",
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
