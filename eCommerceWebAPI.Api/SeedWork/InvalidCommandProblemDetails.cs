using eCommerceWebAPI.Application.Validation;
using Microsoft.AspNetCore.Mvc;

namespace eCommerceWebAPI.Api.SeedWork
{
    public class InvalidCommandProblemDetails : ProblemDetails
    {
        public InvalidCommandProblemDetails(InvalidCommandException exception)
        {
            Title = exception.Message;
            Status = StatusCodes.Status400BadRequest;
            Detail = exception.Details;
            Type = "https://somedomain/validation-error";
        }
    }
}
