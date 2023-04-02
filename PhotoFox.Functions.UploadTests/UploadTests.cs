using Microsoft.Extensions.Logging;
using Moq;
using NLog.LayoutRenderers.Wrappers;
using PhotoFox.Core.Hashing;
using PhotoFox.Core.Imaging;
using PhotoFox.Functions.UploadPhoto;
using PhotoFox.Storage.Blob;
using PhotoFox.Storage.Models;
using PhotoFox.Storage.Table;
using System.Drawing;
using System.Runtime.Versioning;
using LogLevel = PhotoFox.Storage.Models.LogLevel;

namespace PhotoFox.Functions.UploadTests
{
    [SupportedOSPlatform("windows")]
    public class UploadTests
    {
        readonly byte[] jpegData = new byte[]
        {
            0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46, 0x49, 0x46, 0x00, 0x01, 0x01, 0x01, 0x00, 0x48, 0x00, 0x48,
            0x00, 0x00, 0xFF, 0xDB, 0x00, 0x43, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xC2, 0x00, 0x0B, 0x08, 0x00, 0x01, 0x00, 0x01, 0x01, 0x01, 0x11, 0x00, 0xFF, 0xC4, 0x00, 0x14, 0x10, 0x01,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xDA,
            0x00, 0x08, 0x01, 0x01, 0x00, 0x01, 0x3F, 0x10
        };

        [SetUp]
        public void Setup()
        {
            // Method intentionally left empty.
        }

        [Test]
        public async Task Upload_PhotoUploaded_BlobIsRetrievedFromStorage()
        {
            string photoId = "photoId";

            var photoFileStorage = new Mock<IPhotoFileStorage>();
            photoFileStorage.Setup(p => p.GetPhotoAsync(photoId)).Returns(Task.FromResult(BinaryData.FromBytes(jpegData)));

            var thumbnailProvider = new Mock<IThumbnailProvider>();
            thumbnailProvider.Setup(t => t.GenerateThumbnail(It.IsAny<Image>(), It.IsAny<int>(), It.IsAny<int>())).Returns(Image.FromStream(new MemoryStream(jpegData)));

            var function = new UploadFunctionBuilder()
                .WithPhotoFileStorage(photoFileStorage.Object)
                .WithThumbnailProvider(thumbnailProvider.Object)
                .Build();

            string message = "{\"Type\": \"PHOTO\", \"EntityId\": \"" + photoId + "\", \"DateTaken\": \"2000-01-01T00:00:00.0000000Z\"}";

            await function.Run(message, Mock.Of<ILogger>());

            photoFileStorage.Verify(p => p.GetPhotoAsync(photoId), Times.Once);
        }

        [Test]
        public void Upload_InvalidType_ThrowsArgumentException()
        {
            string photoId = "photoId";

            var function = new UploadFunctionBuilder()
                .Build();

            string message = "{\"Type\": \"TEST_INVALID_TYPE\", \"EntityId\": \"" + photoId + "\", \"DateTaken\": \"2000-01-01T00:00:00.0000000Z\"}";

            Assert.ThrowsAsync<ArgumentException>(async () => await function.Run(message, Mock.Of<ILogger>()));
        }

        [Test]
        public void Upload_FileNotInStorage_ThrowsFileNotFoundException()
        {
            string photoId = "photoId";

            var photoFileStorage = new Mock<IPhotoFileStorage>();
            photoFileStorage.Setup(f => f.GetPhotoAsync(photoId)).Returns(Task.FromResult<BinaryData?>(null));

            var function = new UploadFunctionBuilder()
                .WithPhotoFileStorage(photoFileStorage.Object)
                .Build();

            string message = "{\"Type\": \"PHOTO\", \"EntityId\": \"" + photoId + "\", \"DateTaken\": \"2000-01-01T00:00:00.0000000Z\"}";

            Assert.ThrowsAsync<FileNotFoundException>(async () => await function.Run(message, Mock.Of<ILogger>()));
        }

        [Test]
        public void Upload_HashExists_DoesNotThrow()
        {
            string photoId = "photoId";
            string hash = "hash";

            var photoFileStorage = new Mock<IPhotoFileStorage>();
            photoFileStorage.Setup(p => p.GetPhotoAsync(photoId)).Returns(Task.FromResult(BinaryData.FromBytes(jpegData)));

            var streamHash = new Mock<IStreamHash>();
            streamHash.Setup(p => p.ComputeHash(It.IsAny<Stream>())).Returns(hash);

            var photoHashStorage = new Mock<IPhotoHashStorage>();
            photoHashStorage.Setup(p => p.HashExistsAsync(hash)).Returns(Task.FromResult(new Tuple<string, string>("foo", "bar")));

            var function = new UploadFunctionBuilder()
                .WithPhotoFileStorage(photoFileStorage.Object)
                .WithStreamHash(streamHash.Object)
                .WithPhotoHashStorage(photoHashStorage.Object)
                .Build();

            string message = "{\"Type\": \"PHOTO\", \"EntityId\": \"" + photoId + "\", \"DateTaken\": \"2000-01-01T00:00:00.0000000Z\"}";

            Assert.DoesNotThrowAsync(async () => await function.Run(message, Mock.Of<ILogger>()));
        }

        [Test]
        public async Task Upload_HashExists_MessageIsLogged()
        {
            string photoId = "photoId";
            string hash = "hash";

            var photoFileStorage = new Mock<IPhotoFileStorage>();
            photoFileStorage.Setup(p => p.GetPhotoAsync(photoId)).Returns(Task.FromResult(BinaryData.FromBytes(jpegData)));

            var streamHash = new Mock<IStreamHash>();
            streamHash.Setup(p => p.ComputeHash(It.IsAny<Stream>())).Returns(hash);

            var photoHashStorage = new Mock<IPhotoHashStorage>();
            photoHashStorage.Setup(p => p.HashExistsAsync(hash)).Returns(Task.FromResult(new Tuple<string, string>("foo", "bar")));

            var logStorage = new Mock<ILogStorage>();

            var function = new UploadFunctionBuilder()
                .WithPhotoFileStorage(photoFileStorage.Object)
                .WithStreamHash(streamHash.Object)
                .WithPhotoHashStorage(photoHashStorage.Object)
                .WithLogStorage(logStorage.Object)
                .Build();

            string message = "{\"Type\": \"PHOTO\", \"EntityId\": \"" + photoId + "\", \"DateTaken\": \"2000-01-01T00:00:00.0000000Z\"}";

            await function.Run(message, Mock.Of<ILogger>());

            logStorage.Verify(l => l.Log(It.IsAny<string>(), "UploadFunction", photoId, It.IsAny<string>(), LogLevel.Warn, hash), Times.Once);
        }

        [Test]
        public async Task Upload_HashExists_DuplicateIsDeleted()
        {
            string photoId = "photoId";
            string hash = "hash";

            var photoFileStorage = new Mock<IPhotoFileStorage>();
            photoFileStorage.Setup(p => p.GetPhotoAsync(photoId)).Returns(Task.FromResult(BinaryData.FromBytes(jpegData)));

            var streamHash = new Mock<IStreamHash>();
            streamHash.Setup(p => p.ComputeHash(It.IsAny<Stream>())).Returns(hash);

            var photoHashStorage = new Mock<IPhotoHashStorage>();
            photoHashStorage.Setup(p => p.HashExistsAsync(hash)).Returns(Task.FromResult(new Tuple<string, string>("foo", "bar")));

            var function = new UploadFunctionBuilder()
                .WithPhotoFileStorage(photoFileStorage.Object)
                .WithStreamHash(streamHash.Object)
                .WithPhotoHashStorage(photoHashStorage.Object)
                .Build();

            string message = "{\"Type\": \"PHOTO\", \"EntityId\": \"" + photoId + "\", \"DateTaken\": \"2000-01-01T00:00:00.0000000Z\"}";

            await function.Run(message, Mock.Of<ILogger>());

            photoFileStorage.Verify(s => s.DeletePhotoAsync(photoId), Times.Once);
        }

        [Test]
        public async Task Upload_HashExists_PhotoNotSaved()
        {
            string photoId = "photoId";
            string hash = "hash";

            var photoFileStorage = new Mock<IPhotoFileStorage>();
            photoFileStorage.Setup(p => p.GetPhotoAsync(photoId)).Returns(Task.FromResult(BinaryData.FromBytes(jpegData)));

            var streamHash = new Mock<IStreamHash>();
            streamHash.Setup(p => p.ComputeHash(It.IsAny<Stream>())).Returns(hash);

            var photoHashStorage = new Mock<IPhotoHashStorage>();
            photoHashStorage.Setup(p => p.HashExistsAsync(hash)).Returns(Task.FromResult(new Tuple<string, string>("foo", "bar")));

            var photoMetadataStorage = new Mock<IPhotoMetadataStorage>();

            var function = new UploadFunctionBuilder()
                .WithPhotoFileStorage(photoFileStorage.Object)
                .WithStreamHash(streamHash.Object)
                .WithPhotoHashStorage(photoHashStorage.Object)
                .WithPhotoMetadataStorage(photoMetadataStorage.Object)
                .Build();

            string message = "{\"Type\": \"PHOTO\", \"EntityId\": \"" + photoId + "\", \"DateTaken\": \"2000-01-01T00:00:00.0000000Z\"}";

            await function.Run(message, Mock.Of<ILogger>());

            photoMetadataStorage.Verify(s => s.AddPhotoAsync(It.IsAny<PhotoMetadata>()), Times.Never);
        }

        [Test]
        public async Task Upload_HashDoesNotExist_PhotoDataSaved()
        {
            string photoId = "photoId";

            var photoFileStorage = new Mock<IPhotoFileStorage>();
            photoFileStorage.Setup(p => p.GetPhotoAsync(photoId)).Returns(Task.FromResult(BinaryData.FromBytes(jpegData)));

            var thumbnailProvider = new Mock<IThumbnailProvider>();
            thumbnailProvider.Setup(t => t.GenerateThumbnail(It.IsAny<Image>(), It.IsAny<int>(), It.IsAny<int>())).Returns(Image.FromStream(new MemoryStream(jpegData)));

            var photoHashStorage = new Mock<IPhotoHashStorage>();

            var photoMetadataStorage = new Mock<IPhotoMetadataStorage>();

            var photoInAlbumStorage = new Mock<IPhotoInAlbumStorage>();

            var function = new UploadFunctionBuilder()
                .WithPhotoFileStorage(photoFileStorage.Object)
                .WithPhotoMetadataStorage(photoMetadataStorage.Object)
                .WithPhotoHashStorage(photoHashStorage.Object)
                .WithPhotoInAlbumStorage(photoInAlbumStorage.Object)
                .WithThumbnailProvider(thumbnailProvider.Object)
                .Build();

            string message = "{\"Type\": \"PHOTO\", \"EntityId\": \"" + photoId + "\", \"DateTaken\": \"2000-01-01T00:00:00.0000000Z\"}";

            await function.Run(message, Mock.Of<ILogger>());

            photoMetadataStorage.Verify(s => s.AddPhotoAsync(It.IsAny<PhotoMetadata>()), Times.Once);
            photoHashStorage.Verify(s => s.AddHashAsync(It.IsAny<string>(), It.IsAny<string>(), photoId), Times.Once);
            photoInAlbumStorage.Verify(s => s.AddPhotoInAlbumAsync(It.IsAny<string>(), photoId, It.IsAny<DateTime>()), Times.Once);
        }
    }

    [SupportedOSPlatform("windows")]
    public class UploadFunctionBuilder
    {
        private IPhotoFileStorage photoFileStorage;
        private IStreamHash streamHash;
        private IPhotoHashStorage photoHashStorage;
        private IThumbnailProvider thumbnailProvider;
        private IPhotoMetadataStorage photoMetadataStorage;
        private IPhotoInAlbumStorage photoInAlbumStorage;
        private ILogStorage logStorage;
        private IVideoInAlbumStorage videoInAlbumStorage;
        private IVideoStorage videoStorage;

        public UploadFunctionBuilder()
        {
            this.photoFileStorage = Mock.Of<IPhotoFileStorage>();
            this.streamHash = Mock.Of<IStreamHash>();
            this.photoHashStorage = Mock.Of<IPhotoHashStorage>();
            this.photoMetadataStorage = Mock.Of<IPhotoMetadataStorage>();
            this.thumbnailProvider = Mock.Of<IThumbnailProvider>();
            this.photoInAlbumStorage = Mock.Of<IPhotoInAlbumStorage>();
            this.logStorage = Mock.Of<ILogStorage>();
            this.videoInAlbumStorage = Mock.Of<IVideoInAlbumStorage>();
            this.videoStorage = Mock.Of<IVideoStorage>();
        }

        public UploadFunction Build()
        {
            return new UploadFunction(
                this.photoFileStorage,
                this.streamHash,
                this.photoHashStorage,
                this.thumbnailProvider,
                this.photoMetadataStorage,
                this.photoInAlbumStorage,
                this.videoInAlbumStorage,
                this.logStorage,
                this.videoStorage);
        }

        public UploadFunctionBuilder WithPhotoFileStorage(IPhotoFileStorage photoFileStorage)
        {
            this.photoFileStorage = photoFileStorage;
            return this;
        }

        public UploadFunctionBuilder WithThumbnailProvider(IThumbnailProvider thumbnailProvider)
        {
            this.thumbnailProvider = thumbnailProvider;
            return this;
        }

        public UploadFunctionBuilder WithStreamHash(IStreamHash streamHash)
        {
            this.streamHash = streamHash;
            return this;
        }

        public UploadFunctionBuilder WithPhotoHashStorage(IPhotoHashStorage hashStorage)
        {
            this.photoHashStorage = hashStorage;
            return this;
        }

        public UploadFunctionBuilder WithPhotoMetadataStorage(IPhotoMetadataStorage photoMetadataStorage)
        {
            this.photoMetadataStorage = photoMetadataStorage;
            return this;
        }

        public UploadFunctionBuilder WithPhotoInAlbumStorage(IPhotoInAlbumStorage photoInAlbumStorage)
        {
            this.photoInAlbumStorage = photoInAlbumStorage;
            return this;
        }

        public UploadFunctionBuilder WithLogStorage(ILogStorage logStorage)
        {
            this.logStorage = logStorage;
            return this;
        }

        public UploadFunctionBuilder WithVideoInAlbumStorage(IVideoInAlbumStorage videoInAlbumStorage)
        {
            this.videoInAlbumStorage = videoInAlbumStorage;
            return this;
        }

        public UploadFunctionBuilder WithVideoStorage(IVideoStorage videoStorage)
        {
            this.videoStorage = videoStorage;
            return this;
        }
    }
}