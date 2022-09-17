using Azure;
using PhotoFox.Model;
using System.Threading.Tasks;

namespace PhotoFox.Storage.Table
{
    public interface IPhotoDataStorage
    {
        AsyncPageable<PhotoAlbum> GetPhotoAlbums();

        Task<PhotoAlbum> GetPhotoAlbum(int id);

        Task AddPhotoAlbum(PhotoAlbum album);

    }
}
