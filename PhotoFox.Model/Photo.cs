using System;

namespace PhotoFox.Model
{
    public class Photo
    {
        public string PhotoId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DateTaken { get; set; }
        public string Iso { get; set; }
        public string Aperture { get; set; }
        public string FocalLength { get; set; }
        public string Device { get; set; }
        public int? Orientation { get; set; }
        public string Exposure { get; set; }
        public double? GeolocationLattitude { get; set; }
        public double? GeolocationLongitude { get; set; }
        public string FileHash { get; set; }
    }
}
