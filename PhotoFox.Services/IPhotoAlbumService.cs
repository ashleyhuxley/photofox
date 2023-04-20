using PhotoFox.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhotoFox.Services
{
    public interface IPhotoAlbumService
    {
        IAsyncEnumerable<PhotoAlbum> GetAllAlbumsAsync();

        IAsyncEnumerable<PhotoAlbum> GetAllAlbumsAsync(string username);

        IAsyncEnumerable<Photo> GetPhotosInAlbumAsync(string albumId);

        Task AddAlbumAsync(PhotoAlbum album);

        Task AddPhotoToAlbumAsync(string albumId, string photoId, DateTime utcDate);

        Task DeleteAlbumAsync(string albumId);

        Task SetCoverImageAsync(string albumId, string photoId);

        Task<PhotoAlbum> GetPhotoAlbumAsync(string albumId);

        Task RemoveFromAlbumAsync(string albumId, string photoId);

        Task<bool> UserHasPermissionAsync(string albumId, string username);

        Task AddPermissionAsync(string albumId, string username);

        Task RemovePermissionAsync(string albumId, string username);

        Task EditAlbumAsync(string albumId, string title, string description, string folder, string sortOrder);
    }
}
