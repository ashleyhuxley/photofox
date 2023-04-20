using Azure;
using Azure.Data.Tables;
using System;

namespace PhotoFox.Storage.Models
{
    public class PhotoAlbum : ITableEntity
    {
        public PhotoAlbum()
        {
            this.AlbumName = string.Empty;
            this.AlbumDescription = string.Empty;
            this.CoverPhotoId = string.Empty;
            this.RowKey = string.Empty;
            this.PartitionKey = string.Empty;
            this.Folder = string.Empty;
            this.SortOrder = string.Empty;
        }

        public string AlbumName { get; set; }
        public string AlbumDescription { get; set; }
        public string CoverPhotoId { get; set; }
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string Folder { get; set; }
        public string SortOrder { get; set; }
        public ETag ETag { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
    }
}
