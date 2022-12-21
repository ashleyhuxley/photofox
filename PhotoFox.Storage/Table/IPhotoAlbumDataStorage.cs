using Azure;
using PhotoFox.Storage.Models;
using System.Threading.Tasks;

namespace PhotoFox.Storage.Table
{
    public interface IPhotoAlbumDataStorage
    {
        AsyncPageable<PhotoAlbum> GetPhotoAlbums();

        Task AddPhotoAlbum(PhotoAlbum album);

        Task DeleteAlbumAsync(string albumId);

        Task<PhotoAlbum> GetPhotoAlbum(string albumId);

        Task ModifyAlbumAsync(PhotoAlbum album);
    }
}
