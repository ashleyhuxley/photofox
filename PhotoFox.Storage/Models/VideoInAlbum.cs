using Azure;
using Azure.Data.Tables;
using System;

namespace PhotoFox.Storage.Models
{
    public class VideoInAlbum : ITableEntity
    {
        public VideoInAlbum()
        {
            this.PartitionKey = string.Empty;
            this.RowKey= string.Empty;
            this.Title= string.Empty;
            this.FileExt= string.Empty;
        }

        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
        public string Title { get; set; }
        public DateTime? VideoDate { get; set; }
        public int FileSize { get; set; }
        public double? GeolocationLattitude { get; set; }
        public double? GeolocationLongitude { get; set; }
        public string FileExt { get; set; }

    }
}
