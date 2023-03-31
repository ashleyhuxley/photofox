using PhotoFox.Storage;
using System.Configuration;

namespace PhotoFox.Web
{
    public class Config : IStorageConfig
    {
        public string StorageConnectionString => Environment.GetEnvironmentVariable("PhotoFoxStorage") ?? throw new ConfigurationErrorsException("Storage account connection string is missing from config");
    }
}
