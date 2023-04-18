using System;
using System.Drawing;

namespace PhotoFox.Model
{
    public class ImageProperties
    {
        public ImageProperties(
            long? fileSize,
            Size dimensions,
            string title,
            string description,
            DateTime dateTaken,
            int? orientation,
            string fileHash)
        {
            this.FileSize = fileSize;
            this.Dimensions = dimensions;
            this.Title = title;
            this.Description = description;
            this.DateTaken = dateTaken;
            this.Orientation = orientation;
            this.FileHash = fileHash;
        }

        public long? FileSize { get; }
        public Size Dimensions { get; }
        public string Title { get; }
        public string Description { get; }
        public DateTime DateTaken { get; }
        public int? Orientation { get; }
        public string FileHash { get; }
    }
}
