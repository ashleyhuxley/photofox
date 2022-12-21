using Azure;
using Azure.Data.Tables;
using System.Threading.Tasks;
using PhotoFox.Storage.Models;

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

        public async Task<PhotoAlbum> GetPhotoAlbum(string albumId)
        {
            var client = new TableServiceClient(config.StorageConnectionString);
            var tableClient = client.GetTableClient(TableName);
            var result = await tableClient.GetEntityAsync<PhotoAlbum>(albumId, string.Empty);
            return result.Value;
        }

        public async Task ModifyAlbumAsync(PhotoAlbum album)
        {
            var client = new TableServiceClient(config.StorageConnectionString);
            var tableClient = client.GetTableClient(TableName);
            await tableClient.UpdateEntityAsync(album, album.ETag, TableUpdateMode.Replace);
        }
    }
}
