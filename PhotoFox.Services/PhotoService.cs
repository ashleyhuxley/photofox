using PhotoFox.Core;
using PhotoFox.Core.Exif;
using PhotoFox.Extensions;
using PhotoFox.Model;
using PhotoFox.Storage.Blob;
using PhotoFox.Storage.Table;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhotoFox.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly IPhotoMetadataStorage photoMetadataStorage;

        private readonly IPhotoFileStorage photoFileStorage;

        private readonly IPhotoInAlbumStorage photoInAlbumStorage;

        public PhotoService(
            IPhotoMetadataStorage photoMetadataStorage,
            IPhotoFileStorage photoFileStorage,
            IPhotoInAlbumStorage photoInAlbumStorage)
        {
            this.photoMetadataStorage = photoMetadataStorage ?? throw new ArgumentNullException(nameof(photoMetadataStorage));
            this.photoFileStorage = photoFileStorage ?? throw new ArgumentNullException(nameof(photoFileStorage));
            this.photoInAlbumStorage = photoInAlbumStorage ?? throw new ArgumentNullException(nameof(photoInAlbumStorage));
        }

        public async Task<Photo> GetPhotoAsync(DateTime dateTaken, string photoId)
        {
            var metadata = await this.photoMetadataStorage.GetPhotoMetadataAsync(dateTaken, photoId).ConfigureAwait(false);
            return Converter.ToPhoto(metadata);
        }

        public async Task<Photo> GetPhotoAsync(string dateTaken, string photoId)
        {
            var metadata = await this.photoMetadataStorage.GetPhotoMetadataAsync(dateTaken, photoId).ConfigureAwait(false);
            return Converter.ToPhoto(metadata);
        }

        public async Task SavePhotoAsync(Photo photo)
        {
            await this.photoMetadataStorage.SavePhotoAsync(Converter.ToPhotoMetadata(photo)).ConfigureAwait(false);
        }

        public async Task DeletePhotoAsync(Photo photo)
        {
            await Task.WhenAll(
                this.photoFileStorage.DeleteThumbnailAsync(photo.PhotoId),
                this.photoFileStorage.DeletePhotoAsync(photo.PhotoId),
                this.photoMetadataStorage.DeletePhotoAsync(photo.DateTaken.ToPartitionKey(), photo.PhotoId),
                this.photoInAlbumStorage.RemoveFromAllAlbumsAsync(photo.PhotoId)).ConfigureAwait(false);
        }

        public async IAsyncEnumerable<Photo> GetPhotosInAlbumAsync(string albumId)
        {
            await foreach (var photoInAlbum in this.photoInAlbumStorage.GetPhotosInAlbumAsync(albumId))
            {
                var photo = await this.photoMetadataStorage.GetPhotoMetadataAsync(photoInAlbum.UtcDate, photoInAlbum.RowKey).ConfigureAwait(false);
                yield return Converter.ToPhoto(photo);
            }
        }

        public async Task<Photo> ReloadExifDataAsync(DateTime utcDate, string photoId)
        {
            var photoData = await this.photoFileStorage.GetPhotoAsync(photoId).ConfigureAwait(false);
            var exifReader = await ExifReader.FromStreamAsync(photoData.ToStream()).ConfigureAwait(false);

            var metadata = await this.photoMetadataStorage.GetPhotoMetadataAsync(utcDate, photoId).ConfigureAwait(false);
            metadata.Orientation = exifReader.GetOrientation();
            await this.photoMetadataStorage.SavePhotoAsync(metadata).ConfigureAwait(false);

            return Converter.ToPhoto(metadata);
        }

        public async Task<int> DecrementRatingAsync(DateTime utcDate, string photoId)
        {
            var metadata = await this.photoMetadataStorage.GetPhotoMetadataAsync(utcDate, photoId).ConfigureAwait(false);

            if (!metadata.StarRating.HasValue)
            {
                metadata.StarRating = Constants.DefaultStarRating;
            }

            if (metadata.StarRating >= 1)
            {
                metadata.StarRating -= 1;
            }

            await this.photoMetadataStorage.SavePhotoAsync(metadata).ConfigureAwait(false);
            return metadata.StarRating.Value;
        }

        public async Task<int> IncrementRatingAsync(DateTime utcDate, string photoId)
        {
            var metadata = await this.photoMetadataStorage.GetPhotoMetadataAsync(utcDate, photoId).ConfigureAwait(false);

            if (!metadata.StarRating.HasValue)
            {
                metadata.StarRating = Constants.DefaultStarRating;
            }

            if (metadata.StarRating < 5)
            {
                metadata.StarRating += 1;
            }

            await this.photoMetadataStorage.SavePhotoAsync(metadata).ConfigureAwait(false);
            return metadata.StarRating.Value;
        }
    }
}
