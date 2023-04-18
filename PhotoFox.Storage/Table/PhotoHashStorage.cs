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
                PartitionKey = hash,
                RowKey = string.Empty,
                PhotoPartitionKey = partitionKey,
                PhotoRowKey = rowKey
            }).ConfigureAwait(false);
        }

        public async Task<HashSearchResult> HashExistsAsync(string hash)
        {
            var client = new TableServiceClient(config.StorageConnectionString);
            var tableClient = client.GetTableClient(TableName);

            if (await tableClient.EntityExistsAsync<PhotoHash>(hash, string.Empty).ConfigureAwait(false))
            {
                var hashResult = await tableClient.GetEntityAsync<PhotoHash>(hash, string.Empty);
                return new HashSearchResult(hashResult.Value.PhotoPartitionKey, hashResult.Value.PhotoRowKey);
            }
            else
            {
                return HashSearchResult.NotFoundResult;
            }
        }

        public async Task DeleteHashAsync(string hash)
        {
            var client = new TableServiceClient(config.StorageConnectionString);
            var tableClient = client.GetTableClient(TableName);
            await tableClient.DeleteEntityAsync(hash, string.Empty).ConfigureAwait(false);
        }
    }
}
