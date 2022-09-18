using PhotoFox.Storage;
using System.Configuration;

namespace PhotoFox.CmdLine
{
    public class PhotoFoxConfig : IStorageConfig
    {
        public string StorageConnectionString => ConfigurationManager.AppSettings["StorageConnectionString"];
    }
}
