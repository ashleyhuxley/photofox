using PhotoFox.Model;
using PhotoFox.Storage.Models;
using System.Drawing;
using System;
using PhotoFox.Core.Extensions;

namespace PhotoFox.Services
{
    internal static class Converter
    {
        internal static Photo ToPhoto(PhotoMetadata photo)
        {
            var size = new Size(photo.DimensionWidth.GetValueOrDefault(0), photo.DimensionHeight.GetValueOrDefault(0));
            var imageProperties = new ImageProperties(photo.FileSize, size, photo.Title, photo.Description, photo.UtcDate.GetValueOrDefault(DateTime.MinValue), photo.Orientation, photo.FileHash);
            var geolocation = photo.GeolocationLattitude.HasValue && photo.GeolocationLongitude.HasValue ? new Geolocation(photo.GeolocationLattitude.Value, photo.GeolocationLongitude.Value) : null;
            var cameraSettings = new CameraSettings(photo.ISO, photo.Aperture, photo.FocalLength, photo.Device, photo.Manufacturer, photo.Exposure);
            return new Photo(photo.RowKey, imageProperties, geolocation, cameraSettings);
        }

        internal static PhotoMetadata ToPhotoMetadata(Photo photo)
        {
            return new PhotoMetadata
            {
                Aperture = photo.CameraSettings.Aperture,
                Description = photo.ImageProperties.Description,
                Device = photo.CameraSettings.Device,
                DimensionHeight = photo.ImageProperties.Dimensions.Height,
                DimensionWidth = photo.ImageProperties.Dimensions.Width,
                Exposure = photo.CameraSettings.Exposure,
                FileHash = photo.ImageProperties.FileHash,
                FileSize = photo.ImageProperties.FileSize,
                FocalLength = photo.CameraSettings.FocalLength,
                GeolocationLattitude = photo.GeolocationLatitude,
                GeolocationLongitude = photo.GeolocationLongitude,
                ISO = photo.CameraSettings.ISO,
                Manufacturer = photo.CameraSettings.Manufacturer,
                Orientation = photo.ImageProperties.Orientation,
                PartitionKey = photo.DateTaken.ToPartitionKey(),
                RowKey = photo.PhotoId,
                Title = photo.Title,
                UtcDate = photo.DateTaken
            };
        }

        internal static Video ToVideo(VideoInAlbum videoInAlbum)
        {
            var geolocation = (videoInAlbum.GeolocationLattitude.HasValue && videoInAlbum.GeolocationLongitude.HasValue) 
                ? new Geolocation(videoInAlbum.GeolocationLattitude.Value, videoInAlbum.GeolocationLongitude.Value) 
                : null;

            var date = videoInAlbum.VideoDate.HasValue ? videoInAlbum.VideoDate.Value : DateTime.MinValue;

            return new Video(videoInAlbum.RowKey, videoInAlbum.Title, geolocation, date, videoInAlbum.FileSize, videoInAlbum.FileExt);
        }
    }
}
