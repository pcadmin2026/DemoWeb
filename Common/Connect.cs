using System.Configuration;
using System.Data.SqlClient;
namespace EduWebAPI.Common
{
    public sealed class Connect
    {
        private readonly  IConfiguration? _config;
        public Connect(IConfiguration  config)
        {
            _config=config;
        }
        public string ConnectionString()
        {
           return _config?.GetConnectionString("connectionstring")?? string.Empty;  
        }
    }
}
