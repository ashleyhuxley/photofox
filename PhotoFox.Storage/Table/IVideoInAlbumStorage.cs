using Azure;
using PhotoFox.Storage.Models;
using System.Threading.Tasks;

namespace PhotoFox.Storage.Table
{
    public interface IVideoInAlbumStorage
    {
        AsyncPageable<VideoInAlbum> GetVideosInAlbumAsync(string albumId);
        Task AddVideoInAlbumAsync(VideoInAlbum videoInAlbum);
        Task ModifyVideoInAlbumAsync(VideoInAlbum videoInAlbum);
        Task RemoveFromAllAlbumsAsync(string videoId);
    }
}
