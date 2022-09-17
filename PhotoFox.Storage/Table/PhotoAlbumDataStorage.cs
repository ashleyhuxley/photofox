using Azure;
using Azure.Data.Tables;
using PhotoFox.Model;
using System.Threading.Tasks;

namespace PhotoFox.Storage.Table
{
    public class PhotoAlbumDataStorage : IPhotoAlbumDataStorage
    {
        private readonly IStorageConfig config;

        private const string TableName = "PhotoAlbums";

        private const string PartitionKey = "photoalbum";

        public PhotoAlbumDataStorage(IStorageConfig config)
        {
            this.config = config;
        }

        public Task AddPhotoAlbum(PhotoAlbum album)
        {
            throw new System.NotImplementedException();
        }

        public Task<PhotoAlbum> GetPhotoAlbum(int id)
        {
            throw new System.NotImplementedException();
        }

        public AsyncPageable<PhotoAlbum> GetPhotoAlbums()
        {
            var client = new TableServiceClient(config.StorageConnectionString);
            var tableClient = client.GetTableClient(TableName);
            return tableClient.QueryAsync<PhotoAlbum>(p => p.PartitionKey == PartitionKey);
        }
    }
}
