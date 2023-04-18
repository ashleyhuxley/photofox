using Azure.Data.Tables;
using Azure;
using System.Threading.Tasks;
using System;
using PhotoFox.Core.Extensions;
using PhotoFox.Storage.Models;
using PhotoFox.Core.Exceptions;
using System.Linq;

namespace PhotoFox.Storage.Table
{
    public class PhotoMetadataStorage : IPhotoMetadataStorage
    {
        private readonly IStorageConfig config;

        private const string TableName = "PhotoMetadata";

        public PhotoMetadataStorage(IStorageConfig storageConfig)
        {
            this.config = storageConfig;
        }

        public Task<bool> PhotoExistsAsync(DateTime utcDate, string photoId)
        {
            return PhotoExistsAsync(utcDate.ToPartitionKey(), photoId);
        }

        public async Task<bool> PhotoExistsAsync(string utcDate, string photoId)
        {
            var client = new TableServiceClient(config.StorageConnectionString);
            var tableClient = client.GetTableClient(TableName);
            return await tableClient.EntityExistsAsync<PhotoMetadata>(utcDate, photoId).ConfigureAwait(false);
        }

        public async Task<PhotoMetadata> GetPhotoMetadataAsync(string utcDate, string photoId)
        {
            var client = new TableServiceClient(config.StorageConnectionString);
            var tableClient = client.GetTableClient(TableName);
            var result = await tableClient.GetEntityAsync<PhotoMetadata>(utcDate, photoId).ConfigureAwait(false);
            if (result.Value == null)
            {
                throw new EntityNotFoundException(TableName, photoId);
            }

            return result.Value;
        }

        public Task<PhotoMetadata> GetPhotoMetadataAsync(DateTime utcDate, string photoId)
        {
            return this.GetPhotoMetadataAsync(utcDate.ToPartitionKey(), photoId);
        }

        public async Task<PhotoMetadata> GetPhotoMetadataAsync(string photoId)
        {
            var client = new TableServiceClient(config.StorageConnectionString);
            var tableClient = client.GetTableClient(TableName);
            var results = tableClient.QueryAsync<PhotoMetadata>(p => p.RowKey == photoId);

            var result = await results.FirstOrDefaultAsync();

            if (result == null)
            {
                throw new EntityNotFoundException(nameof(PhotoMetadata), photoId);
            }

            return result;
        }

        public AsyncPageable<PhotoMetadata> GetAllPhotosAsync()
        {
            var client = new TableServiceClient(config.StorageConnectionString);
            var tableClient = client.GetTableClient(TableName);
            return tableClient.QueryAsync<PhotoMetadata>();
        }

        public AsyncPageable<PhotoMetadata> GetPhotosByDateAsync(DateTime date)
        {
            var client = new TableServiceClient(config.StorageConnectionString);
            var tableClient = client.GetTableClient(TableName);
            return tableClient.QueryAsync<PhotoMetadata>(p => p.PartitionKey == date.ToPartitionKey());
        }

        public async Task AddPhotoAsync(PhotoMetadata photo)
        {
            var client = new TableServiceClient(config.StorageConnectionString);
            var tableClient = client.GetTableClient(TableName);
            await tableClient.AddEntityAsync(photo).ConfigureAwait(false);
        }

        public async Task DeletePhotoAsync(string partitionKey, string rowKey)
        {
            var client = new TableServiceClient(config.StorageConnectionString);
            var tableClient = client.GetTableClient(TableName);
            await tableClient.DeleteEntityAsync(partitionKey, rowKey).ConfigureAwait(false);
        }

        public async Task SavePhotoAsync(PhotoMetadata metadata)
        {
            var client = new TableServiceClient(config.StorageConnectionString);
            var tableClient = client.GetTableClient(TableName);
            await tableClient.UpdateEntityAsync(metadata, metadata.ETag, TableUpdateMode.Replace).ConfigureAwait(false);
        }
    }
}
