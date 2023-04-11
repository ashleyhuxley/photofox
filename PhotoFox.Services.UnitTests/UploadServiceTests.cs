using Moq;
using PhotoFox.Storage.Blob;
using PhotoFox.Storage.Models;
using PhotoFox.Storage.Queue;
using System.Runtime.Versioning;

namespace PhotoFox.Services.UnitTests
{
    [TestFixture]
    public class UploadServiceTests
    {
        private Mock<IPhotoFileStorage> photoFileStorage;

        private Mock<IVideoStorage> videoStorage;

        private Mock<IUploadQueue> uploadQueue;

        [SetUp]
        public void Setup()
        {
            photoFileStorage = new Mock<IPhotoFileStorage>();
            videoStorage = new Mock<IVideoStorage>();
            uploadQueue = new Mock<IUploadQueue>();
        }

        [Test]
        public void Constructor_NullParameters_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new UploadService(null, videoStorage.Object, uploadQueue.Object));
            Assert.Throws<ArgumentNullException>(() => new UploadService(photoFileStorage.Object, null, uploadQueue.Object));
            Assert.Throws<ArgumentNullException>(() => new UploadService(photoFileStorage.Object, videoStorage.Object, null));
        }

        [Test]
        public void UploadFromStreamAsync_StreamIsNull_ThrowsArgumentNull()
        {
            var service = new UploadService(photoFileStorage.Object, videoStorage.Object, uploadQueue.Object);
            Assert.ThrowsAsync<ArgumentNullException>(() => service.UploadVideoFromStreamAsync(null, "album", "title", "ext", new DateTime(2000, 1, 1)));
        }

        [Test]
        [SupportedOSPlatform("windows")]
        public async Task UploadFromStreamAsync_StreamHasData_StoresPhoto()
        {
            var service = new UploadService(photoFileStorage.Object, videoStorage.Object, uploadQueue.Object);
            var data = new byte[] { 0, 1, 2, 3 };

            using (var stream = new MemoryStream(data))
            {
                await service.UploadFromStreamAsync(stream, "album", "title", "ext", new DateTime(2000, 1, 1));
            }

            photoFileStorage.Verify(s => s.PutPhotoAsync(It.IsAny<string>(), It.Is<BinaryData>(d => d.ToArray().Length == data.Length)));
        }

        [Test]
        [SupportedOSPlatform("windows")]
        public async Task UploadFromStreamAsync_StreamHasData_MessageIsEnqueud()
        {
            string albumId = "albumId";
            string ext = "ext";
            string title = "title";

            var service = new UploadService(photoFileStorage.Object, videoStorage.Object, uploadQueue.Object);
            var data = new byte[] { 0, 1, 2, 3 };

            using (var stream = new MemoryStream(data))
            {
                await service.UploadFromStreamAsync(stream, albumId, title, ext, new DateTime(2000, 1, 1));
            }

            uploadQueue.Verify(s => s.QueueUploadMessage(It.Is<UploadMessage>(m => m.Album == albumId && m.FileExt == ext && m.Title == title)));
        }

        [Test]
        [SupportedOSPlatform("windows")]
        public async Task UploadVideoFromStreamAsync_StreamHasData_StoresVideo()
        {
            var service = new UploadService(photoFileStorage.Object, videoStorage.Object, uploadQueue.Object);
            var data = new byte[] { 0, 1, 2, 3 };

            using (var stream = new MemoryStream(data))
            {
                await service.UploadVideoFromStreamAsync(stream, "album", "title", "ext", new DateTime(2000, 1, 1));
            }

            videoStorage.Verify(s => s.PutVideoAsync(It.IsAny<string>(), It.Is<BinaryData>(d => d.ToArray().Length == data.Length)));
        }

        [Test]
        [SupportedOSPlatform("windows")]
        public async Task UploadVideoFromStreamAsync_StreamHasData_MessageIsEnqueud()
        {
            string albumId = "albumId";
            string ext = "ext";
            string title = "title";

            var service = new UploadService(photoFileStorage.Object, videoStorage.Object, uploadQueue.Object);
            var data = new byte[] { 0, 1, 2, 3 };

            using (var stream = new MemoryStream(data))
            {
                await service.UploadVideoFromStreamAsync(stream, albumId, title, ext, new DateTime(2000, 1, 1));
            }

            uploadQueue.Verify(s => s.QueueUploadMessage(It.Is<UploadMessage>(m => m.Album == albumId && m.FileExt == ext && m.Title == title)));
        }
    }
}
