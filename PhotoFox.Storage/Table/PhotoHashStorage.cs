using Azure.Data.Tables;
using PhotoFox.Core.Extensions;
using PhotoFox.Model;
using System;
using System.Threading.Tasks;

namespace PhotoFox.Storage.Table
{
    public class PhotoHashStorage : IPhotoHashStorage
    {
        private const string TableName = "PhotoHashes";

        private readonly IStorageConfig config;

        public PhotoHashStorage(IStorageConfig storageConfig)
        {
            config = storageConfig;
        }

        public async Task AddHashAsync(string hash, string partitionKey, string rowKey)
        {
            var client = new TableServiceClient(config.StorageConnectionString);
            var tableClient = client.GetTableClient(TableName);
            await tableClient.AddEntityAsync(new PhotoHash {
                PartitionKey = hash.ToHashPartitionKey(),
                RowKey = hash,
                PhotoPartitionKey = partitionKey,
                PhotoRowKey = rowKey
            });
        }

        public async Task<Tuple<string, string>?> HashExistsAsync(string hash)
        {
            var client = new TableServiceClient(config.StorageConnectionString);
            var tableClient = client.GetTableClient(TableName);
            var result = await tableClient.GetEntityAsync<PhotoHash>(hash.ToHashPartitionKey(), hash);
            return result == null 
                ? null 
                : Tuple.Create(result.Value.PhotoPartitionKey, result.Value.PhotoRowKey);
        }

        public async Task DeleteHashAsync(string hash)
        {
            var client = new TableServiceClient(config.StorageConnectionString);
            var tableClient = client.GetTableClient(TableName);
            await tableClient.DeleteEntityAsync(hash.ToHashPartitionKey(), hash);
        }
    }
}
