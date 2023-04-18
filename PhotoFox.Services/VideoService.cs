using PhotoFox.Model;
using PhotoFox.Storage.Blob;
using PhotoFox.Storage.Table;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhotoFox.Services
{
    public class VideoService : IVideoService
    {
        private readonly IVideoInAlbumStorage videoInAlbumStorage;

        private readonly IVideoStorage videoStorage;

        public VideoService(
            IVideoInAlbumStorage videoInAlbumStorage,
            IVideoStorage videoStorage)
        {
            this.videoInAlbumStorage = videoInAlbumStorage;
            this.videoStorage = videoStorage;
        }

        public async IAsyncEnumerable<Video> GetVideosInAlbumAsync(string albumId)
        {
            await foreach (var videoInAlbum in this.videoInAlbumStorage.GetVideosInAlbumAsync(albumId))
            {
                yield return Converter.ToVideo(videoInAlbum);
            }
        }

        public async Task DeleteVideoAsync(Video video)
        {
            await Task.WhenAll(
                this.videoStorage.DeleteVideoAsync(video.VideoId),
                this.videoInAlbumStorage.RemoveFromAllAlbumsAsync(video.VideoId)).ConfigureAwait(false);
        }
    }
}
