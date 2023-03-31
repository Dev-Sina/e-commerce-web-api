using eCommerceWebAPI.Application.Catalogs.Products.Commands.AddProduct;
using System.Reflection;

namespace eCommerceWebAPI.Infrastructure.Processing
{
    internal static class Assemblies
    {
        public static readonly Assembly Application = typeof(AddProductCommand).Assembly;
    }
}
