using System;

namespace PhotoFox.Model
{
    public class Photo : IDisplayableItem
    {
        public Photo(
            string photoId, 
            ImageProperties imageProperties, 
            Geolocation? geolocation, 
            CameraSettings cameraSettings)
        {
            PhotoId = photoId;
            ImageProperties = imageProperties;
            Geolocation = geolocation;
            CameraSettings = cameraSettings;
        }

        public string PhotoId { get; set; }
        public ImageProperties ImageProperties { get; }
        public Geolocation? Geolocation { get; }
        public CameraSettings CameraSettings { get; }

        public double? GeolocationLatitude => this.Geolocation?.Latitude;
        public double? GeolocationLongitude => this.Geolocation?.Longitude;
        public string Title => this.ImageProperties.Title;
        public long? FileSize => this.ImageProperties.FileSize;
        public DateTime DateTaken => this.ImageProperties.DateTaken;
    }
}
