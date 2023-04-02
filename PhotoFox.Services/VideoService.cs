using AutoMapper;
using PhotoFox.Model;
using PhotoFox.Storage.Table;
using System.Collections.Generic;

namespace PhotoFox.Services
{
    public class VideoService : IVideoService
    {
        private readonly IVideoInAlbumStorage videoInAlbumStorage;

        private readonly IMapper mapper;

        public VideoService(
            IVideoInAlbumStorage videoInAlbumStorage,
            IMapper mapper)
        {
            this.videoInAlbumStorage= videoInAlbumStorage;
            this.mapper= mapper;
        }

        public async IAsyncEnumerable<Video> GetVideosInAlbumAsync(string albumId)
        {
            await foreach (var videoInAlbum in this.videoInAlbumStorage.GetVideosInAlbumAsync(albumId))
            {
                yield return mapper.Map<Video>(videoInAlbum);
            }
        }
    }
}
