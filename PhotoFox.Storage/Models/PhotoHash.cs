using Azure;
using Azure.Data.Tables;
using System;

namespace PhotoFox.Storage.Models
{
    public class PhotoHash : ITableEntity
    {
        public PhotoHash() 
        {
            PartitionKey = string.Empty;
            RowKey = string.Empty;
            PhotoPartitionKey = string.Empty;
            PhotoRowKey = string.Empty;
        }

        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public ETag ETag { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public string PhotoPartitionKey { get; set; }
        public string PhotoRowKey { get; set; }
    }
}
