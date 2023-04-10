using System;

namespace PhotoFox.Storage.Models
{
    public class UploadMessage
    {
        public string Type { get; set; }
        public string EntityId { get; set; }
        public string Album { get; set; }
        public string Title { get; set; }
        public DateTime DateTaken { get; set; }
        public string FileExt { get; set; }

        public UploadMessage()
        {
            this.Type = "UNKNOWN";
            this.EntityId = Guid.Empty.ToString();
        }
    }
}
