using eCommerceWebAPI.Application.Addresses;
using eCommerceWebAPI.Application.Catalogs.Categories;
using eCommerceWebAPI.Application.Customers;
using eCommerceWebAPI.Application.Invoices;
using eCommerceWebAPI.Application.Orders;
using eCommerceWebAPI.Application.Catalogs.Products;
using eCommerceWebAPI.Application.ShoppingCarts;
using eCommerceWebAPI.Application.Catalogs.Specifications;
using eCommerceWebAPI.Domain.Addresses;
using eCommerceWebAPI.Domain.Catalogs;
using eCommerceWebAPI.Domain.Customers;
using eCommerceWebAPI.Domain.Invoices;
using eCommerceWebAPI.Domain.Orders;
using eCommerceWebAPI.Domain.ShoppingCarts;

namespace eCommerceWebAPI.Application.Configuration.Mapping
{

    public class AutoMapping : AutoMapper.Profile
    {
        public AutoMapping()
        {
            CreateMap<Country, CountryDto>();
            CreateMap<CountryDto, Country>();
            CreateMap<Province, ProvinceDto>();
            CreateMap<ProvinceDto, Province>();
            CreateMap<City, CityDto>();
            CreateMap<CityDto, City>();
            CreateMap<Address, AddressDto>();
            CreateMap<AddressDto, Address>();

            CreateMap<Category, CategoryDto>();
            CreateMap<CategoryDto, Category>();
            CreateMap<Product, ProductDto>();
            CreateMap<ProductDto, Product>();
            CreateMap<ProductCategoryMapping, ProductCategoryMappingDto>();
            CreateMap<ProductCategoryMappingDto, ProductCategoryMapping>();
            CreateMap<Specification, SpecificationDto>();
            CreateMap<SpecificationDto, Specification>();
            CreateMap<SpecificationValue, SpecificationValueDto>();
            CreateMap<SpecificationValueDto, SpecificationValue>();
            CreateMap<ProductSpecificationValueMapping, ProductSpecificationValueMappingDto>();
            CreateMap<ProductSpecificationValueMappingDto, ProductSpecificationValueMapping>();

            CreateMap<Customer, CustomerDto>();
            CreateMap<CustomerDto, Customer>()
                .ForMember(c => c.CreatedOnUtc, opt => opt.MapFrom(cDto => DateTime.SpecifyKind(cDto.CreatedOnUtc, DateTimeKind.Utc)));
            CreateMap<CustomerAddressMapping, CustomerAddressMappingDto>();
            CreateMap<CustomerAddressMappingDto, CustomerAddressMapping>();

            CreateMap<Order, OrderDto>();
            CreateMap<OrderDto, Order>()
                .ForMember(o => o.CreatedOnUtc, opt => opt.MapFrom(oDto => DateTime.SpecifyKind(oDto.CreatedOnUtc, DateTimeKind.Utc)));
            CreateMap<OrderItem, OrderItemDto>();
            CreateMap<OrderItemDto, OrderItem>();

            CreateMap<Invoice, InvoiceDto>();
            CreateMap<InvoiceDto, Invoice>()
                .ForMember(i => i.OrderCreatedOnUtc, opt => opt.MapFrom(iDto => DateTime.SpecifyKind(iDto.OrderCreatedOnUtc, DateTimeKind.Utc)))
                .ForMember(i => i.InvoiceCreatedOnUtc, opt => opt.MapFrom(iDto => DateTime.SpecifyKind(iDto.InvoiceCreatedOnUtc, DateTimeKind.Utc)));
            CreateMap<InvoiceAddress, InvoiceAddressDto>();
            CreateMap<InvoiceAddressDto, InvoiceAddress>();
            CreateMap<InvoiceItem, InvoiceItemDto>();
            CreateMap<InvoiceItemDto, InvoiceItem>();

            CreateMap<ShoppingCartItem, ShoppingCartItemDto>();
            CreateMap<ShoppingCartItemDto, ShoppingCartItem>();
            CreateMap<ShoppingCartItemRedis, ShoppingCartItemRedisDto>();
            CreateMap<ShoppingCartItemRedisDto, ShoppingCartItemRedis>();
            CreateMap<ShoppingCartItemRedis, ShoppingCartItem>();
            CreateMap<ShoppingCartItemRedisDto, ShoppingCartItemDto>();
        }
    }
}
