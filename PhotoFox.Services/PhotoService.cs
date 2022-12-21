using AutoMapper;
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

        public async IAsyncEnumerable<Photo> GetPhotosByDateTaken(DateTime dateTaken)
        {
            await foreach (var photo in this.photoMetadataStorage.GetPhotosByDate(dateTaken))
            {
                yield return mapper.Map<Photo>(photo);
            }
        }

        public async Task<Photo> GetPhotoAsync(DateTime dateTaken, string photoId)
        {
            var metadata = await this.photoMetadataStorage.GetPhotoMetadata(dateTaken, photoId);
            return mapper.Map<Photo>(metadata);
        }

        public async Task SavePhotoAsync(Photo photo)
        {
            await this.photoMetadataStorage.SavePhotoAsync(mapper.Map<PhotoMetadata>(photo));
        }

        public async Task DeletePhotoAsync(Photo photo)
        {
            await Task.WhenAll(
                this.photoFileStorage.DeleteThumbnailAsync(photo.PhotoId),
                this.photoFileStorage.DeletePhotoAsync(photo.PhotoId),
                this.photoMetadataStorage.DeletePhotoAsync(photo.DateTaken.ToPartitionKey(), photo.PhotoId),
                this.photoHashStorage.DeleteHashAsync(photo.FileHash),
                this.photoInAlbumStorage.RemoveFromAllAlbums(photo.PhotoId));
        }

        public async IAsyncEnumerable<Photo> GetPhotosByDateNotInAlbum(DateTime dateTaken)
        {
            await foreach (var photo in this.photoMetadataStorage.GetPhotosByDate(dateTaken))
            {
                if (! await this.photoInAlbumStorage.IsPhotoInAnAlbumAsync(photo.RowKey))
                {
                    yield return mapper.Map<Photo>(photo);
                }
            }
        }

        public async IAsyncEnumerable<Photo> GetPhotosInAlbum(string albumId)
        {
            await foreach (var photoInAlbum in this.photoInAlbumStorage.GetPhotosInAlbum(albumId))
            {
                var photo = await this.photoMetadataStorage.GetPhotoMetadata(photoInAlbum.UtcDate, photoInAlbum.RowKey);
                yield return mapper.Map<Photo>(photo);
            }
        }
    }
}
