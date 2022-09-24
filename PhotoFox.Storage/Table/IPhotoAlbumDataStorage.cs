using Azure;
using PhotoFox.Model;
using System.Threading.Tasks;

namespace PhotoFox.Storage.Table
{
    public interface IPhotoAlbumDataStorage
    {
        AsyncPageable<PhotoAlbum> GetPhotoAlbums();

        Task<PhotoAlbum> GetPhotoAlbum(int id);

        Task AddPhotoAlbum(PhotoAlbum album);

        Task DeleteAlbumAsync(string albumId);
    }
}
