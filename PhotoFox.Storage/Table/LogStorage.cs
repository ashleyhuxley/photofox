using Azure.Data.Tables;
using PhotoFox.Storage.Models;
using System;
using System.Threading.Tasks;

namespace PhotoFox.Storage.Table
{
    public class LogStorage : ILogStorage
    {
        private const string TableName = "logs";

        private readonly IStorageConfig config;

        public LogStorage(IStorageConfig config)
        {
            this.config = config;
        }

        public async Task Log(
            string message, 
            string source, 
            string photoId = "", 
            string albumId = "", 
            string level = "INFO", 
            string hash = "")
        {
            await Log(new LogEntry
            {
                AlbumId = albumId,
                LogLevel = level,
                Message = message,
                PartitionKey = DateTime.UtcNow.ToString("yyyyMMdd"),
                PhotoHash = hash,
                PhotoId = photoId,
                RowKey = Guid.NewGuid().ToString(),
                Source = source
            });
        }

        public async Task Log(LogEntry entry)
        {
            var client = new TableServiceClient(config.StorageConnectionString);
            var tableClient = client.GetTableClient(TableName);
            await tableClient.AddEntityAsync(entry).ConfigureAwait(false);
        }
    }
}
