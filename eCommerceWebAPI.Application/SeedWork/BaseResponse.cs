using System.Net;

namespace eCommerceWebAPI.Application.SeedWork
{
    public class BaseResponse : IBaseResponse
    {
        public HttpStatusCode HttpStatusCode { get; set; }
        public List<string> Errors { get; set; } = new();
        public string Message { get; set; } = string.Empty;
    }
}
