using PhotoFox.Storage;
using System.Configuration;

namespace PhotoFox.Ui.Wpf
{
    public class PhotoFoxConfig : IStorageConfig
    {
        public string StorageConnectionString => ConfigurationManager.AppSettings["StorageConnectionString"] ?? throw new ConfigurationErrorsException("StorageConnectionString missing from config");
    }
}
