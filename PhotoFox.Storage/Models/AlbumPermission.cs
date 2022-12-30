using Azure;
using Azure.Data.Tables;
using System;

namespace PhotoFox.Storage.Models
{
    public class AlbumPermission : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
