using System.Data;

namespace eCommerceWebAPI.Application.Configuration
{
    public interface ISqlConnectionFactory
    {
        IDbConnection GetOpenConnection();
    }
}
