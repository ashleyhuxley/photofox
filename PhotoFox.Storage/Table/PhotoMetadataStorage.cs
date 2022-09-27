using Azure.Data.Tables;
using Azure;
using System.Threading.Tasks;
using System;
using PhotoFox.Core.Extensions;
using PhotoFox.Storage.Models;

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

        public async Task<PhotoMetadata> GetPhotoMetadata(DateTime utcDate, string photoId)
        {
            var client = new TableServiceClient(config.StorageConnectionString);
            var tableClient = client.GetTableClient(TableName);
            var result = await tableClient.GetEntityAsync<PhotoMetadata>(utcDate.ToPartitionKey(), photoId);
            return result.Value;
        }

        public AsyncPageable<PhotoMetadata> GetAllPhotos()
        {
            var client = new TableServiceClient(config.StorageConnectionString);
            var tableClient = client.GetTableClient(TableName);
            return tableClient.QueryAsync<PhotoMetadata>();
        }

        public AsyncPageable<PhotoMetadata> GetPhotosByDate(DateTime date)
        {
            var client = new TableServiceClient(config.StorageConnectionString);
            var tableClient = client.GetTableClient(TableName);
            return tableClient.QueryAsync<PhotoMetadata>(p => p.PartitionKey == date.ToPartitionKey());
        }

        public async Task AddPhotoAsync(PhotoMetadata photo)
        {
            var client = new TableServiceClient(config.StorageConnectionString);
            var tableClient = client.GetTableClient(TableName);
            await tableClient.AddEntityAsync(photo);
        }

        public async Task DeletePhotoAsync(string partitionKey, string rowKey)
        {
            var client = new TableServiceClient(config.StorageConnectionString);
            var tableClient = client.GetTableClient(TableName);
            await tableClient.DeleteEntityAsync(partitionKey, rowKey);
        }

        public async Task SavePhotoAsync(PhotoMetadata metadata)
        {
            var client = new TableServiceClient(config.StorageConnectionString);
            var tableClient = client.GetTableClient(TableName);
            await tableClient.UpdateEntityAsync(metadata, metadata.ETag, TableUpdateMode.Replace);
        }
    }
}
