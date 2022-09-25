using Azure;
using Azure.Data.Tables;
using System;

namespace PhotoFox.Model
{
    public class PhotoInAlbum : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public ETag ETag { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public DateTime UtcDate { get; set; }
    }
}
