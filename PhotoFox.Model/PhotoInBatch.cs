using Azure;
using Azure.Data.Tables;
using System;

namespace PhotoFox.Model
{
    public class PhotoInBatch : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
