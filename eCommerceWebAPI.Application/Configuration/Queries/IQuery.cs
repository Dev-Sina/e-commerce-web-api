using MediatR;

namespace eCommerceWebAPI.Application.Configuration.Queries
{
    public interface IQuery<out TResult> : IRequest<TResult>
    {
    }
}
