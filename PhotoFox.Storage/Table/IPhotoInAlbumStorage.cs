using Azure;
using PhotoFox.Model;

namespace PhotoFox.Storage.Table
{
    public interface IPhotoInAlbumStorage
    {
        AsyncPageable<PhotoInAlbum> GetPhotosInAlbum(string albumId);
    }
}
