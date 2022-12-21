using Azure;
using Azure.Data.Tables;
using PhotoFox.Storage.Models;
using System.Threading.Tasks;

namespace PhotoFox.Storage.Table
{
    public class PhotoAlbumDataStorage : IPhotoAlbumDataStorage
    {
        private readonly IStorageConfig config;

        private const string TableName = "PhotoAlbums";

        public PhotoAlbumDataStorage(IStorageConfig config)
        {
            this.config = config;
        }

        public async Task AddPhotoAlbum(PhotoAlbum album)
        {
            var client = new TableServiceClient(config.StorageConnectionString);
            var tableClient = client.GetTableClient(TableName);
            await tableClient.AddEntityAsync(album);
        }

        public AsyncPageable<PhotoAlbum> GetPhotoAlbums()
        {
            var client = new TableServiceClient(config.StorageConnectionString);
            var tableClient = client.GetTableClient(TableName);
            return tableClient.QueryAsync<PhotoAlbum>();
        }

        public async Task DeleteAlbumAsync(string albumId)
        {
            var client = new TableServiceClient(config.StorageConnectionString);
            var tableClient = client.GetTableClient(TableName);
            await tableClient.DeleteEntityAsync(albumId, string.Empty);
        }
    }
}
