using Azure;
using PhotoFox.Storage.Models;
using System.Threading.Tasks;

namespace PhotoFox.Storage.Table
{
    public interface IPhotoAlbumDataStorage
    {
        AsyncPageable<PhotoAlbum> GetPhotoAlbumsAsync();

        Task AddPhotoAlbumAsync(PhotoAlbum album);

        Task DeleteAlbumAsyncAsync(string albumId);

        Task<PhotoAlbum> GetPhotoAlbumAsync(string albumId);

        Task ModifyAlbumAsync(PhotoAlbum album);

        AsyncPageable<PhotoAlbum> GetPublicPhotoAlbumsAsync();
    }
}
