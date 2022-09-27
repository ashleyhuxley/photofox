using Azure;
using PhotoFox.Model;
using System.Threading.Tasks;

namespace PhotoFox.Storage.Table
{
    public interface IPhotoInAlbumStorage
    {
        AsyncPageable<PhotoInAlbum> GetPhotosInAlbum(string albumId);
        Task AddPhotoInAlbumAsync(string albumId, string photoId);
    }
}
