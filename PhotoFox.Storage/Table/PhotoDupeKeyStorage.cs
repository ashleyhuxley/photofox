using Azure.Data.Tables;
using PhotoFox.Core.Extensions;
using PhotoFox.Storage.Models;
using System;
using System.Threading.Tasks;

namespace PhotoFox.Storage.Table
{
    public class PhotoDupeKeyStorage : IPhotoDupeKeyStorage
    {
        private const string TableName = "PhotoDupeKeys";

        private readonly IStorageConfig config;

        public PhotoDupeKeyStorage(IStorageConfig storageConfig)
        {
            config = storageConfig;
        }

        public async Task AddKeyAsync(DateTime utcDate, long fileSize, string partitionKey, string rowKey)
        {
            var client = new TableServiceClient(config.StorageConnectionString);
            var tableClient = client.GetTableClient(TableName);
            await tableClient.AddEntityAsync(new PhotoHash
            {
                PartitionKey = utcDate.ToDupeKeyPartitionKey(),
                RowKey = fileSize.ToString(),
                PhotoPartitionKey = partitionKey,
                PhotoRowKey = rowKey
            });
        }

        public async Task<Tuple<string, string>?> KeyExistsAsync(DateTime utcDate, long fileSize, string partitionKey)
        {
            var client = new TableServiceClient(config.StorageConnectionString);
            var tableClient = client.GetTableClient(TableName);

            if (await tableClient.EntityExistsAsync<PhotoHash>(utcDate.ToDupeKeyPartitionKey(), fileSize.ToString()))
            {
                var hashResult = await tableClient.GetEntityAsync<PhotoHash>(utcDate.ToDupeKeyPartitionKey(), fileSize.ToString());
                return Tuple.Create(hashResult.Value.PhotoPartitionKey, hashResult.Value.PhotoRowKey);
            }
            else
            {
                return null;
            }
        }

        public async Task DeleteKeyAsync(DateTime utcDate, long fileSize)
        {
            var client = new TableServiceClient(config.StorageConnectionString);
            var tableClient = client.GetTableClient(TableName);
            await tableClient.DeleteEntityAsync(utcDate.ToDupeKeyPartitionKey(), fileSize.ToString());
        }
    }
}
