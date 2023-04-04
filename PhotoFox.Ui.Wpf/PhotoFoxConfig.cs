using PhotoFox.Storage;
using System.Configuration;

namespace PhotoFox.Ui.Wpf
{
    public class PhotoFoxConfig : IStorageConfig, IViewerConfig
    {
        public string StorageConnectionString => ConfigurationManager.AppSettings["StorageConnectionString"] ?? throw new ConfigurationErrorsException("StorageConnectionString missing from config");

        public string PhotoViewerPath => ConfigurationManager.AppSettings["PhotoViewerPath"] ?? throw new ConfigurationErrorsException("PhotoViewerPath missing from config");

        public string VideoViewerPath => ConfigurationManager.AppSettings["VideoViewerPath"] ?? throw new ConfigurationErrorsException("VideoViewerPath missing from config");
    }
}
