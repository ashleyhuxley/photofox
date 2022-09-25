using Azure;
using Azure.Data.Tables;
using PhotoFox.Model;

namespace PhotoFox.Storage.Table
{
    public class PhotoInAlbumStorage : IPhotoInAlbumStorage
    {
        private readonly IStorageConfig config;

        private const string TableName = "PhotoAlbums";

        public PhotoInAlbumStorage(IStorageConfig config)
        {
            this.config = config;
        }

        public AsyncPageable<PhotoInAlbum> GetPhotosInAlbum(string albumId)
        {
            var client = new TableServiceClient(config.StorageConnectionString);
            var tableClient = client.GetTableClient(TableName);
            return tableClient.QueryAsync<PhotoInAlbum>(p => p.PartitionKey == albumId);
        }
    }
}

