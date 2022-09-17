using Azure;
using Azure.Data.Tables;
using PhotoFox.Core;
using PhotoFox.Model;

namespace PhotoFox.Storage.Table
{
    public class PhotoInBatchDataStorage : IPhotoInBatchStorage
    {
        private readonly IStorageConfig config;

        private const string TableName = "PhotoInBatch";

        public PhotoInBatchDataStorage(IStorageConfig storageConfig)
        {
            this.config = storageConfig;
        }

        public AsyncPageable<PhotoInBatch> GetPhotosInBatch(int batchId)
        {
            var client = new TableServiceClient(config.StorageConnectionString);
            var tableClient = client.GetTableClient(TableName);
            return tableClient.QueryAsync<PhotoInBatch>(p => p.PartitionKey == batchId.ToBatchId());
        }
    }
}
