using Azure;
using Azure.Data.Tables;
using System;

namespace PhotoFox.Model
{
    public class PhotoHash : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public ETag ETag { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public string PhotoPartitionKey { get; set; }
        public string PhotoRowKey { get; set; }
    }
}
