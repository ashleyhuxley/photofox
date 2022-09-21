using Azure.Data.Tables;
using Azure;
using PhotoFox.Model;
using System.Threading.Tasks;
using System;
using System.Collections.Concurrent;
using PhotoFox.Core.Extensions;

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
    }
}
