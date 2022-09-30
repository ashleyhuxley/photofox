using AutoMapper;
using PhotoFox.Model;
using PhotoFox.Storage.Table;
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

                yield return mapper.Map<Photo>(photo);
            }
        }

        public async Task AddAlbumAsync(PhotoAlbum album)
        {
            await this.photoAlbumDataStorage.AddPhotoAlbum(mapper.Map<Storage.Models.PhotoAlbum>(album));
        }

        public async Task AddPhotoToAlbumAsync(string albumId, string photoId)
        {
            await this.photoInAlbumStorage.AddPhotoInAlbumAsync(albumId, photoId);
        }
    }
}
