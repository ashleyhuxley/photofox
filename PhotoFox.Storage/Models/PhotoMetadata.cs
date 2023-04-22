using Azure;
using Azure.Data.Tables;
using System;

namespace PhotoFox.Storage.Models
{
    public class PhotoMetadata : ITableEntity
    {
        public PhotoMetadata()
        {
            Title = string.Empty;
            Description = string.Empty;
            ISO = string.Empty;
            Aperture = string.Empty;
            PartitionKey = string.Empty;
            FocalLength = string.Empty;
            RowKey = string.Empty;
            Manufacturer = string.Empty;
            Device = string.Empty;
            Exposure = string.Empty;
            FileHash = string.Empty;
        }

        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? UtcDate { get; set; }
        public string ISO { get; set; }
        public string Aperture { get; set; }
        public string PartitionKey { get; set; }
        public string FocalLength { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
        public string Manufacturer { get; set; }
        public string Device { get; set; }
        public int? Orientation { get; set; }
        public string Exposure { get; set; }
        public double? GeolocationLattitude { get; set; }
        public double? GeolocationLongitude { get; set; }
        public string FileHash { get; set; }
        public int? DimensionWidth { get; set; }
        public int? DimensionHeight { get; set; }
        public long? FileSize { get; set; }
        public int? StarRating { get; set; }
    }
}
