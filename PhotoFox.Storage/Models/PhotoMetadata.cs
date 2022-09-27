using Azure;
using Azure.Data.Tables;
using System;

namespace PhotoFox.Storage.Models
{
    public class PhotoMetadata : ITableEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime UtcDate { get; set; }
        public string Iso { get; set; }
        public string Aperture { get; set; }
        public string PartitionKey { get; set; }
        public string FocalLength { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
        public string Device { get; set; }
        public int? Orientation { get; set; }
        public string Exposure { get; set; }
        public double? GeolocationLattitude { get; set; }
        public double? GeolocationLongitude { get; set; }
        public string FileHash { get; set; }
    }
}
