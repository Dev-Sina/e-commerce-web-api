using AutoMapper;
using eCommerceWebAPI.Application.Configuration.Queries;
using eCommerceWebAPI.Domain;

namespace eCommerceWebAPI.Application.ShoppingCarts.Queries.GetShoppingCartItem
{
    public class GetShoppingCartItemsByCustomerIdQueryHandler : IQueryHandler<GetShoppingCartItemsByCustomerIdQuery, IList<ShoppingCartItemDto>>
    {
        private readonly IMapper _mapper;
        private readonly IShoppingCartRepository _shoppingCartRepository;

        public GetShoppingCartItemsByCustomerIdQueryHandler(IMapper mapper,
            IShoppingCartRepository shoppingCartRepository)
        {
            _mapper = mapper;
            _shoppingCartRepository = shoppingCartRepository;
        }

        public async Task<IList<ShoppingCartItemDto>> Handle(GetShoppingCartItemsByCustomerIdQuery request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));

            try
            {
                long customerId = request.CustomerId;
                var customerShoppingCartItems = await _shoppingCartRepository.GetShoppingCartAsync(customerId);

                var customerShoppingCartItemDtos = _mapper.Map<List<ShoppingCartItemDto>>(customerShoppingCartItems);
                return customerShoppingCartItemDtos;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }
}
