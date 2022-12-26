using PhotoFox.Storage;

namespace PhotoFox.Web
{
    public class Config : IStorageConfig
    {
        public string StorageConnectionString => "storage_connection_string";
    }
}
