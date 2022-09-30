using PhotoFox.Model;
using System.Collections.Generic;

namespace PhotoFox.Services
{
    public interface IPhotoAlbumService
    {
        IAsyncEnumerable<PhotoAlbum> GetAllAlbums();

        IAsyncEnumerable<Photo> GetPhotosInAlbum(string albumId);
    }
}
