using PhotoFox.Model;
using PhotoFox.Storage.Table;
using System;
using System.Collections.Generic;

namespace PhotoFox.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly IPhotoMetadataStorage photoMetadataStorage;

        public PhotoService(IPhotoMetadataStorage photoMetadataStorage)
        {
            this.photoMetadataStorage = photoMetadataStorage;
        }

        public async IAsyncEnumerable<Photo> GetPhotosByDateTaken(DateTime dateTaken)
        {
            await foreach (var photo in this.photoMetadataStorage.GetPhotosByDate(dateTaken))
            {
                yield return new Photo
                {
                    Aperture = photo.Aperture,
                    DateTaken = photo.UtcDate,
                    Description = photo.Description,
                    Device = photo.Device,
                    Exposure = photo.Exposure,
                    FileHash = photo.FileHash,
                    FocalLength = photo.FocalLength,
                    GeolocationLattitude = photo.GeolocationLattitude,
                    GeolocationLongitude = photo.GeolocationLongitude,
                    Iso = photo.Iso,
                    Orientation = photo.Orientation,
                    Title = photo.Title,
                    PhotoId = photo.RowKey
                };
            }
        }
    }
}
