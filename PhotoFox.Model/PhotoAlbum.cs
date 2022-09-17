using Azure;
using Azure.Data.Tables;
using System;

namespace PhotoFox.Model
{
    public class PhotoAlbum : ITableEntity
    {
        public string AlbumName { get; set; }
        public string AlbumDescription { get; set; }
        public string CoverPhotoId { get; set; }
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public ETag ETag { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
    }
}
