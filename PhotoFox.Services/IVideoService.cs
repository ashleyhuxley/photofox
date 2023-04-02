using PhotoFox.Model;
using System.Collections.Generic;

namespace PhotoFox.Services
{
    public interface IVideoService
    {
        IAsyncEnumerable<Video> GetVideosInAlbumAsync(string albumId);
    }
}
