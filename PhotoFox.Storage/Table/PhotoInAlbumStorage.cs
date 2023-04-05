using Azure;
using Azure.Data.Tables;
using PhotoFox.Storage.Models;
using System;
using System.Threading.Tasks;

namespace PhotoFox.Storage.Table
{
    public class PhotoInAlbumStorage : IPhotoInAlbumStorage
    {
        private readonly IStorageConfig config;

        private const string TableName = "PhotoInAlbum";

        public PhotoInAlbumStorage(IStorageConfig config)
        {
            this.config = config;
        }

        public AsyncPageable<PhotoInAlbum> GetAlbumsForPhotoId(string photoId)
        {
            var client = new TableServiceClient(config.StorageConnectionString);
            var tableClient = client.GetTableClient(TableName);
            return tableClient.QueryAsync<PhotoInAlbum>(p => p.RowKey == photoId);
        }

        public AsyncPageable<PhotoInAlbum> GetPhotosInAlbumAsync(string albumId)
        {
            var client = new TableServiceClient(config.StorageConnectionString);
            var tableClient = client.GetTableClient(TableName);
            return tableClient.QueryAsync<PhotoInAlbum>(p => p.PartitionKey == albumId);
        }

        public async Task AddPhotoInAlbumAsync(string albumId, string photoId, DateTime utcDate)
        {
            var client = new TableServiceClient(config.StorageConnectionString);
            var tableClient = client.GetTableClient(TableName);
            await tableClient.AddEntityAsync(new PhotoInAlbum { PartitionKey = albumId, RowKey = photoId, UtcDate = utcDate }).ConfigureAwait(false);
        }

        public async Task<bool> IsPhotoInAnAlbumAsync(string photoId)
        {
            var client = new TableServiceClient(config.StorageConnectionString);
            var tableClient = client.GetTableClient(TableName);
            var items = tableClient.QueryAsync<PhotoInAlbum>(p => p.RowKey == photoId);
            bool result = false;
            await foreach (var item in items)
            {
                result = true;
            }

            return result;
        }

        public async Task ModifyPhotoInAlbumAsync(PhotoInAlbum photoInAlbum)
        {
            var client = new TableServiceClient(config.StorageConnectionString);
            var tableClient = client.GetTableClient(TableName);
            await tableClient.UpdateEntityAsync(photoInAlbum, ETag.All).ConfigureAwait(false);
        }

        public async Task RemoveFromAllAlbumsAsync(string photoId)
        {
            var client = new TableServiceClient(config.StorageConnectionString);
            var tableClient = client.GetTableClient(TableName);
            var items = tableClient.QueryAsync<PhotoInAlbum>(p => p.RowKey == photoId);

            await foreach (var item in items)
            {
                await tableClient.DeleteEntityAsync(item.PartitionKey, item.RowKey).ConfigureAwait(false);
            }
        }

        public async Task RemovePhotoFromAlbumAsync(string albumId, string photoId)
        {
            var client = new TableServiceClient(config.StorageConnectionString);
            var tableClient = client.GetTableClient(TableName);
            await tableClient.DeleteEntityAsync(albumId, photoId).ConfigureAwait(false);
        }
    }
}

