using Azure;
using Azure.Data.Tables;
using PhotoFox.Storage.Models;
using System.Threading.Tasks;

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

        public async Task AddPhotoInAlbumAsync(string albumId, string photoId)
        {
            var client = new TableServiceClient(config.StorageConnectionString);
            var tableClient = client.GetTableClient(TableName);
            await tableClient.AddEntityAsync(new PhotoInAlbum { PartitionKey = albumId, RowKey = photoId });
        }
    }
}

