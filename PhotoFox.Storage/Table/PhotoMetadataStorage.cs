using Azure.Data.Tables;
using Azure;
using PhotoFox.Model;
using System.Threading.Tasks;
using System;
using PhotoFox.Core;

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
    }
}
