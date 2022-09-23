using Azure.Data.Tables;
using PhotoFox.Model;
using System;
using System.Threading.Tasks;

namespace PhotoFox.Storage.Table
{
    public class PhotoHashStorage
    {
        private const string TableName = "PhotoHashes";

        private readonly IStorageConfig config;

        public PhotoHashStorage(IStorageConfig storageConfig)
        {
            config = storageConfig;
        }

        public async Task AddHash(string hash, string partitionKey, string rowKey)
        {
            var client = new TableServiceClient(config.StorageConnectionString);
            var tableClient = client.GetTableClient(TableName);
            await tableClient.AddEntityAsync(new PhotoHash {
                PartitionKey = hash.Substring(0, 1),
                RowKey = hash,
                PhotoPartitionKey = partitionKey,
                PhotoRowKey = rowKey
            });
        }

        public async Task<Tuple<string, string>?> HashExists(string hash)
        {
            var client = new TableServiceClient(config.StorageConnectionString);
            var tableClient = client.GetTableClient(TableName);
            var result = await tableClient.GetEntityAsync<PhotoHash>(hash.Substring(0, 1), hash);
            return result == null 
                ? null 
                : Tuple.Create(result.Value.PhotoPartitionKey, result.Value.PhotoRowKey);
        }

        public async Task DeleteHash(string hash)
        {
            var client = new TableServiceClient(config.StorageConnectionString);
            var tableClient = client.GetTableClient(TableName);
            await tableClient.DeleteEntityAsync(hash.Substring(0, 1), hash);
        }
    }
}
