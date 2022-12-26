using AutoMapper;
using PhotoFox.Core.Exif;
using PhotoFox.Core.Extensions;
using PhotoFox.Model;
using PhotoFox.Storage.Blob;
using PhotoFox.Storage.Models;
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

        private readonly IPhotoHashStorage photoHashStorage;

        private readonly IPhotoInAlbumStorage photoInAlbumStorage;

        private readonly IMapper mapper;

        public PhotoService(
            IPhotoMetadataStorage photoMetadataStorage,
            IPhotoFileStorage photoFileStorage,
            IPhotoHashStorage photoHashStorage,
            IPhotoInAlbumStorage photoInAlbumStorage,
            IMapper mapper)
        {
            this.photoMetadataStorage = photoMetadataStorage;
            this.photoFileStorage = photoFileStorage;
            this.photoHashStorage = photoHashStorage;
            this.photoInAlbumStorage = photoInAlbumStorage;
            this.mapper = mapper;
        }

        public async IAsyncEnumerable<Photo> GetPhotosByDateTakenAsync(DateTime dateTaken)
        {
            await foreach (var photo in this.photoMetadataStorage.GetPhotosByDateAsync(dateTaken))
            {
                yield return mapper.Map<Photo>(photo);
            }
        }

        public async Task<Photo> GetPhotoAsync(DateTime dateTaken, string photoId)
        {
            var metadata = await this.photoMetadataStorage.GetPhotoMetadataAsync(dateTaken, photoId).ConfigureAwait(false);
            return mapper.Map<Photo>(metadata);
        }

        public async Task SavePhotoAsync(Photo photo)
        {
            await this.photoMetadataStorage.SavePhotoAsync(mapper.Map<PhotoMetadata>(photo)).ConfigureAwait(false);
        }

        public async Task DeletePhotoAsync(Photo photo)
        {
            await Task.WhenAll(
                this.photoFileStorage.DeleteThumbnailAsync(photo.PhotoId),
                this.photoFileStorage.DeletePhotoAsync(photo.PhotoId),
                this.photoMetadataStorage.DeletePhotoAsync(photo.DateTaken.ToPartitionKey(), photo.PhotoId),
                this.photoInAlbumStorage.RemoveFromAllAlbumsAsync(photo.PhotoId)).ConfigureAwait(false);
        }

        public async IAsyncEnumerable<Photo> GetPhotosByDateNotInAlbumAsync(DateTime dateTaken)
        {
            await foreach (var photo in this.photoMetadataStorage.GetPhotosByDateAsync(dateTaken))
            {
                if (! await this.photoInAlbumStorage.IsPhotoInAnAlbumAsync(photo.RowKey).ConfigureAwait(false))
                {
                    yield return mapper.Map<Photo>(photo);
                }
            }
        }

        public async IAsyncEnumerable<Photo> GetPhotosInAlbumAsync(string albumId)
        {
            await foreach (var photoInAlbum in this.photoInAlbumStorage.GetPhotosInAlbumAsync(albumId))
            {
                var photo = await this.photoMetadataStorage.GetPhotoMetadataAsync(photoInAlbum.UtcDate, photoInAlbum.RowKey).ConfigureAwait(false);
                yield return mapper.Map<Photo>(photo);
            }
        }

        public async Task<Photo> ReloadExifDataAsync(DateTime utcDate, string photoId)
        {
            var photoData = await this.photoFileStorage.GetPhotoAsync(photoId).ConfigureAwait(false);
            var exifReader = await ExifReader.FromStreamAsync(photoData.ToStream()).ConfigureAwait(false);

            var metadata = await this.photoMetadataStorage.GetPhotoMetadataAsync(utcDate, photoId).ConfigureAwait(false);
            metadata.Orientation = exifReader.GetOrientation();
            await this.photoMetadataStorage.SavePhotoAsync(metadata).ConfigureAwait(false);

            return mapper.Map<Photo>(metadata);
        }
    }
}
