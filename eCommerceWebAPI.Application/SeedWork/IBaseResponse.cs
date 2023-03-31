using System.Net;

namespace eCommerceWebAPI.Application.SeedWork
{
    public interface IBaseResponse
    {
        HttpStatusCode HttpStatusCode { get; set; }
        List<string> Errors { get; set; }
        string Message { get; set; }
    }
}
