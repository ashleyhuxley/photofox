using Azure;
using Azure.Data.Tables;
using System;

namespace PhotoFox.Model
{
    public class PhotoMetadata : ITableEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DateTaken { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
