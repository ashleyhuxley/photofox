using System;

namespace PhotoFox.Model
{
    public class Video : IDisplayableItem
    {
        public string VideoId { get; set; }
        public string Title { get; set; }
        public double? GeolocationLatitude { get; set; }
        public double? GeolocationLongitude { get; set; }
        public DateTime DateTaken { get; set; }
        public long? FileSize { get; set; }
    }
}
