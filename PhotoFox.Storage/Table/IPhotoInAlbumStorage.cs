using Azure;
using PhotoFox.Storage.Models;
using System;
using System.Threading.Tasks;

namespace PhotoFox.Storage.Table
{
    public interface IPhotoInAlbumStorage
    {
        AsyncPageable<PhotoInAlbum> GetPhotosInAlbumAsync(string albumId);
        Task AddPhotoInAlbumAsync(string albumId, string photoId, DateTime utcDate);
        Task<bool> IsPhotoInAnAlbumAsync(string photoId);
        Task RemoveFromAllAlbumsAsync(string photoId);
        Task RemovePhotoFromAlbumAsync(string albumId, string photoId);
    }
}
