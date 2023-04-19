using System;
using System.IO;
using Newtonsoft.Json;
using PhotoFox.Extensions;

namespace PhotoFox.Model
{
    public class AlbumImport
    {
        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("photos")]
        public PhotoImport[]? Photos { get; set; }

        [JsonProperty("cover_photo")]
        public Photo? CoverPhoto { get; set; }

        [JsonProperty("description")]
        public string? Description { get; set; }

        [JsonProperty("last_modified_timestamp")]
        public long? LastModifiedTimestamp { get; set; }
    }

    public class PhotoImport
    {
        [JsonProperty("uri")]
        public string? Uri { get; set; }

        [JsonProperty("title")]
        public string? Title { get; set; }

        [JsonProperty("creation_timestamp")]
        public long? CreationTimestamp { get; set; }

        public DateTime? Date => CreationTimestamp.HasValue ? CreationTimestamp.Value.ToDateTime() : null;

        public string FileName => this.Uri == null ? string.Empty : Path.GetFileName(this.Uri);
    }
}
