using PhotoFox.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhotoFox.Services
{
    public interface IVideoService
    {
        IAsyncEnumerable<Video> GetVideosInAlbumAsync(string albumId);
        Task DeleteVideoAsync(Video video);
    }
}
