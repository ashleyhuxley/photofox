using Azure.Data.Tables;
using PhotoFox.Core.Extensions;
using PhotoFox.Storage.Models;
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

            if (await tableClient.EntityExistsAsync<PhotoHash>(hash.ToHashPartitionKey(), hash))
            {
                var hashResult = await tableClient.GetEntityAsync<PhotoHash>(hash.ToHashPartitionKey(), hash);
                return Tuple.Create(hashResult.Value.PhotoPartitionKey, hashResult.Value.PhotoRowKey);
            }
            else
            {
                return null;
            }
        }

        public async Task DeleteHashAsync(string hash)
        {
            var client = new TableServiceClient(config.StorageConnectionString);
            var tableClient = client.GetTableClient(TableName);
            await tableClient.DeleteEntityAsync(hash.ToHashPartitionKey(), hash);
        }
    }
}
