using System;

namespace PhotoFox.Functions.UploadPhoto
{
    internal class UploadMessage
    {
        public string Type { get; set; }
        public string EntityId { get; set; }
        public string? Album { get; set; }
        public string? Title { get; set; }
        public DateTime DateTaken { get; set; }

        public UploadMessage()
        {
            this.Type = "UNKNOWN";
            this.EntityId = Guid.Empty.ToString();
        }
    }
}
