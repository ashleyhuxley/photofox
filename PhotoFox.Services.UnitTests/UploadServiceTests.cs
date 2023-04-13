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
        private Mock<IUploadQueue> uploadQueue;

        private Mock<IUploadStorage> uploadStorage;

        [SetUp]
        public void Setup()
        {
            uploadQueue = new Mock<IUploadQueue>();
            uploadStorage = new Mock<IUploadStorage>();
        }

        [Test]
        [SupportedOSPlatform("windows")]
        public async Task UploadFromStreamAsync_StreamHasData_StoresPhoto()
        {
            var service = new UploadService(uploadQueue.Object, uploadStorage.Object);
            var data = new byte[] { 0, 1, 2, 3 };

            using (var stream = new MemoryStream(data))
            {
                await service.UploadFromStreamAsync(stream, "album", "title", "ext", new DateTime(2000, 1, 1));
            }

            uploadStorage.Verify(s => s.PutFileAsync(It.IsAny<string>(), It.Is<BinaryData>(d => d.ToArray().Length == data.Length)));
        }

        [Test]
        [SupportedOSPlatform("windows")]
        public async Task UploadFromStreamAsync_StreamHasData_MessageIsEnqueud()
        {
            string albumId = "albumId";
            string ext = "ext";
            string title = "title";

            var service = new UploadService(uploadQueue.Object, uploadStorage.Object);
            var data = new byte[] { 0, 1, 2, 3 };

            using (var stream = new MemoryStream(data))
            {
                await service.UploadFromStreamAsync(stream, albumId, title, ext, new DateTime(2000, 1, 1));
            }

            uploadQueue.Verify(s => s.QueueUploadMessageAsync(It.Is<UploadMessage>(m => m.Album == albumId && m.FileExt == ext && m.Title == title)));
        }

        [Test]
        [SupportedOSPlatform("windows")]
        public async Task UploadVideoFromStreamAsync_StreamHasData_StoresVideo()
        {
            var service = new UploadService(uploadQueue.Object, uploadStorage.Object);
            var data = new byte[] { 0, 1, 2, 3 };

            using (var stream = new MemoryStream(data))
            {
                await service.UploadVideoFromStreamAsync(stream, "album", "title", "ext", new DateTime(2000, 1, 1));
            }

            uploadStorage.Verify(s => s.PutFileAsync(It.IsAny<string>(), It.Is<BinaryData>(d => d.ToArray().Length == data.Length)));
        }

        [Test]
        [SupportedOSPlatform("windows")]
        public async Task UploadVideoFromStreamAsync_StreamHasData_MessageIsEnqueud()
        {
            string albumId = "albumId";
            string ext = "ext";
            string title = "title";

            var service = new UploadService(uploadQueue.Object, uploadStorage.Object);
            var data = new byte[] { 0, 1, 2, 3 };

            using (var stream = new MemoryStream(data))
            {
                await service.UploadVideoFromStreamAsync(stream, albumId, title, ext, new DateTime(2000, 1, 1));
            }

            uploadQueue.Verify(s => s.QueueUploadMessageAsync(It.Is<UploadMessage>(m => m.Album == albumId && m.FileExt == ext && m.Title == title)));
        }
    }
}
