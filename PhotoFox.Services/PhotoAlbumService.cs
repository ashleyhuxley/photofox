using AutoMapper;
using PhotoFox.Model;
using PhotoFox.Storage.Table;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhotoFox.Services
{
    public class PhotoAlbumService : IPhotoAlbumService
    {
        private readonly IPhotoAlbumDataStorage photoAlbumDataStorage;

        private readonly IPhotoInAlbumStorage photoInAlbumStorage;

        private readonly IPhotoMetadataStorage photoMetadataStorage;

        private readonly IMapper mapper;

        public PhotoAlbumService(
            IPhotoAlbumDataStorage photoAlbumDataStorage,
            IPhotoInAlbumStorage photoInAlbumStorage,
            IPhotoMetadataStorage photoMetadataStorage,
            IMapper mapper)
        {
            this.photoAlbumDataStorage = photoAlbumDataStorage;
            this.photoInAlbumStorage = photoInAlbumStorage;
            this.photoMetadataStorage = photoMetadataStorage;
            this.mapper = mapper;
        }

        public async IAsyncEnumerable<PhotoAlbum> GetAllAlbumsAsync()
        {
            await foreach (var album in this.photoAlbumDataStorage.GetPhotoAlbumsAsync())
            {
                yield return new PhotoAlbum
                {
                    AlbumId = album.PartitionKey,
                    CoverPhotoId = album.CoverPhotoId,
                    Description = album.AlbumDescription,
                    Title = album.AlbumName
                };
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

        public async Task AddAlbumAsync(PhotoAlbum album)
        {
            var storageAlbum = mapper.Map<Storage.Models.PhotoAlbum>(album);
            await this.photoAlbumDataStorage.AddPhotoAlbumAsync(storageAlbum).ConfigureAwait(false);
        }

        public async Task AddPhotoToAlbumAsync(string albumId, string photoId, DateTime utcDate)
        {
            await this.photoInAlbumStorage.AddPhotoInAlbumAsync(albumId, photoId, utcDate).ConfigureAwait(false);
        }

        public async Task DeleteAlbumAsync(string albumId)
        {
            await this.photoAlbumDataStorage.DeleteAlbumAsyncAsync(albumId).ConfigureAwait(false);
        }

        public async Task SetCoverImageAsync(string albumId, string photoId)
        {
            var album = await this.photoAlbumDataStorage.GetPhotoAlbumAsync(albumId).ConfigureAwait(false);
            album.CoverPhotoId = photoId;
            await this.photoAlbumDataStorage.ModifyAlbumAsync(album).ConfigureAwait(false);
        }

        public async Task<PhotoAlbum> GetPhotoAlbumAsync(string albumId)
        {
            var album = await this.photoAlbumDataStorage.GetPhotoAlbumAsync(albumId).ConfigureAwait(false);
            return mapper.Map<PhotoAlbum>(album);
        }

        public async Task RemoveFromAlbumAsync(string albumId, string photoId)
        {
            await this.photoInAlbumStorage.RemovePhotoFromAlbumAsync(albumId, photoId).ConfigureAwait(false);
        }
    }
}
