using PhotoFox.Model;
using System.Collections.Generic;

namespace PhotoFox.Services
{
    public interface IPhotoAlbumService
    {
        IAsyncEnumerable<PhotoAlbum> GetAllAlbums();
    }
}
