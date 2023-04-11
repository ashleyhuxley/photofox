using AutoMapper;
using Azure;
using Moq;
using PhotoFox.Core.Extensions;
using PhotoFox.Mappings;
using PhotoFox.Storage.Blob;
using PhotoFox.Storage.Models;
using PhotoFox.Storage.Table;

namespace PhotoFox.Services.UnitTests
{
    [TestFixture]
    public class VideoServiceTests
    {
        private Mock<IVideoInAlbumStorage> videoInAlbumStorage;

        private Mock<IVideoStorage> videoStorage;

        private IMapper mapper;

        [SetUp]
        public void Setup()
        {
            videoInAlbumStorage= new Mock<IVideoInAlbumStorage>();
            videoStorage= new Mock<IVideoStorage>();
            mapper = MapFactory.GetMap();
        }

        [Test]
        public void Constructor_ParametersAreNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new VideoService(null, videoStorage.Object, mapper));
            Assert.Throws<ArgumentNullException>(() => _ = new VideoService(videoInAlbumStorage.Object, null, mapper));
            Assert.Throws<ArgumentNullException>(() => _ = new VideoService(videoInAlbumStorage.Object, videoStorage.Object, null));
        }

        [Test]
        public async Task Thing()
        {
            string albumId = "albumId";

            var videoInAlbum = new VideoInAlbum
            {
                PartitionKey = albumId,
                RowKey = "videoId"
            };

            videoInAlbumStorage.Setup(p => p.GetVideosInAlbumAsync(albumId)).Returns(videoInAlbum.AsAsyncPageable());

            var service = new VideoService(
                videoInAlbumStorage.Object,
                videoStorage.Object,
                mapper);

            await foreach (var video in service.GetVideosInAlbumAsync(albumId))
            {
                _ = video;
            }

            videoInAlbumStorage.Verify(s => s.GetVideosInAlbumAsync(albumId), Times.Once);
        }
    }
}
