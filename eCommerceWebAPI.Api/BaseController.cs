using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace eCommerceWebAPI.Api
{
    public class BaseController : ControllerBase
    {
        private readonly ISender _sender;

        public BaseController(ISender sender)
        {
            _sender = sender;
        }
    }

}
