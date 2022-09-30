using PhotoFox.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhotoFox.Services
{
    public interface IPhotoAlbumService
    {
        IAsyncEnumerable<PhotoAlbum> GetAllAlbums();

        IAsyncEnumerable<Photo> GetPhotosInAlbum(string albumId);

        Task AddAlbumAsync(PhotoAlbum album);

        Task AddPhotoToAlbumAsync(string albumId, string photoId);

        Task DeleteAlbumAsync(string albumId);
    }
}
