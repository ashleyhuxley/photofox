using Azure;
using PhotoFox.Storage.Models;
using System.Threading.Tasks;

namespace PhotoFox.Storage.Table
{
    public interface IPhotoInAlbumStorage
    {
        AsyncPageable<PhotoInAlbum> GetPhotosInAlbum(string albumId);
        Task AddPhotoInAlbumAsync(string albumId, string photoId);
    }
}
