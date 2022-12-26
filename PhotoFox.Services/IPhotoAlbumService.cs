using PhotoFox.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhotoFox.Services
{
    public interface IPhotoAlbumService
    {
        IAsyncEnumerable<PhotoAlbum> GetAllAlbumsAsync();

        IAsyncEnumerable<Photo> GetPhotosInAlbumAsync(string albumId);

        Task AddAlbumAsync(PhotoAlbum album);

        Task AddPhotoToAlbumAsync(string albumId, string photoId, DateTime utcDate);

        Task DeleteAlbumAsync(string albumId);

        Task SetCoverImageAsync(string albumId, string photoId);

        Task<PhotoAlbum> GetPhotoAlbumAsync(string albumId);

        Task RemoveFromAlbumAsync(string albumId, string photoId);
    }
}
