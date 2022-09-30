using PhotoFox.Model;
using PhotoFox.Storage.Table;
using System.Collections.Generic;

namespace PhotoFox.Services
{
    public class PhotoAlbumService : IPhotoAlbumService
    {
        private readonly IPhotoAlbumDataStorage photoAlbumDataStorage;

        private readonly IPhotoInAlbumStorage photoInAlbumStorage;

        private readonly IPhotoMetadataStorage photoMetadataStorage;

        public PhotoAlbumService(
            IPhotoAlbumDataStorage photoAlbumDataStorage,
            IPhotoInAlbumStorage photoInAlbumStorage,
            IPhotoMetadataStorage photoMetadataStorage)
        {
            this.photoAlbumDataStorage = photoAlbumDataStorage;
            this.photoInAlbumStorage = photoInAlbumStorage;
            this.photoMetadataStorage = photoMetadataStorage;
        }

        public async IAsyncEnumerable<PhotoAlbum> GetAllAlbums()
        {
            await foreach (var album in this.photoAlbumDataStorage.GetPhotoAlbums())
            {
                yield return new PhotoAlbum
                {
                    AlbumId = album.RowKey,
                    CoverPhotoId = album.CoverPhotoId,
                    Description = album.AlbumDescription,
                    Title = album.AlbumName
                };
            }
        }

        public async IAsyncEnumerable<Photo> GetPhotosInAlbum(string albumId)
        {
            await foreach (var photoInAlbum in this.photoInAlbumStorage.GetPhotosInAlbum(albumId))
            {
                var photo = await this.photoMetadataStorage.GetPhotoMetadata(photoInAlbum.UtcDate, photoInAlbum.RowKey);

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
