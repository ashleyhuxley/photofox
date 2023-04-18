using System;

namespace PhotoFox.Model
{
    public class Video : IDisplayableItem
    {
        public Video(
            string videoId, 
            string title, 
            Geolocation? geolocation,
            DateTime dateTaken,
            long? fileSize,
            string fileExt)
        {
            this.FileExt = fileExt;
            this.VideoId = videoId;
            this.Title = title;
            this.Geolocation = geolocation;
            this.DateTaken = dateTaken;
            this.FileSize= fileSize;
        }

        public Geolocation? Geolocation { get; }
        public string VideoId { get; }
        public string Title { get; set; }
        public DateTime DateTaken { get; set; }
        public long? FileSize { get; set; }
        public string FileExt { get; set; }

        public double? GeolocationLatitude => this.Geolocation?.Latitude;
        public double? GeolocationLongitude => this.Geolocation?.Longitude;
    }
}
