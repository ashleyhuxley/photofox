using Moq;
using PhotoFox.Extensions;
using PhotoFox.Model;
using PhotoFox.Storage.Blob;
using PhotoFox.Storage.Models;
using PhotoFox.Storage.Table;

namespace PhotoFox.Services.UnitTests
{
    [TestFixture]
    public class PhotoServiceTests
    {
        private Mock<IPhotoMetadataStorage> photoMetadataStorage;
        private Mock<IPhotoFileStorage> photoFileStorage;
        private Mock<IPhotoInAlbumStorage> photoInAlbumStorage;

        [SetUp]
        public void Setup()
        {
            photoMetadataStorage = new Mock<IPhotoMetadataStorage>();
            photoFileStorage = new Mock<IPhotoFileStorage>();
            photoInAlbumStorage = new Mock<IPhotoInAlbumStorage>();
        }

        [Test]
        public async Task GetPhotoAsync_ValidArgsDateTimeOverload_GetsPhotoFromStorage()
        {
            var date = DateTime.SpecifyKind(new DateTime(2000, 1, 1), DateTimeKind.Utc);
            var photoId = "photoId";

            var metadata = new PhotoMetadata { PartitionKey = date.ToPartitionKey(), RowKey = photoId };

            photoMetadataStorage.Setup(s => s.GetPhotoMetadataAsync(date, photoId)).Returns(Task.FromResult(metadata));

            var service = new PhotoService(
                photoMetadataStorage.Object, 
                photoFileStorage.Object, 
                photoInAlbumStorage.Object);

            await service.GetPhotoAsync(date, photoId);

            photoMetadataStorage.Verify(s => s.GetPhotoMetadataAsync(date, photoId), Times.Once);
        }

        [Test]
        public async Task GetPhotoAsync_ValidArgsStringOverload_GetsPhotoFromStorage()
        {
            var date = DateTime.SpecifyKind(new DateTime(2000, 1, 1), DateTimeKind.Utc).ToPartitionKey();
            var photoId = "photoId";

            var metadata = new PhotoMetadata { PartitionKey = date, RowKey = photoId };

            photoMetadataStorage.Setup(s => s.GetPhotoMetadataAsync(date, photoId)).Returns(Task.FromResult(metadata));

            var service = new PhotoService(
                photoMetadataStorage.Object,
                photoFileStorage.Object,
                photoInAlbumStorage.Object);

            await service.GetPhotoAsync(date, photoId);

            photoMetadataStorage.Verify(s => s.GetPhotoMetadataAsync(date, photoId), Times.Once);
        }

        [Test]
        public async Task SavePhotoAsync_ValidArgs_SavesPhotoWithCorrectData()
        {
            var photo = GetSamplePhoto();

            var verify = (PhotoMetadata m) =>
            {
                return m.RowKey == photo.PhotoId
                    && m.Title == photo.Title
                    && m.Aperture == photo.CameraSettings.Aperture
                    && m.Description == photo.ImageProperties.Description
                    && m.Device == photo.CameraSettings.Device
                    && m.Exposure == photo.CameraSettings.Exposure
                    && m.DimensionWidth == photo.ImageProperties.Dimensions.Width
                    && m.DimensionHeight == photo.ImageProperties.Dimensions.Height
                    && m.FileHash == photo.ImageProperties.FileHash
                    && m.FileSize == photo.FileSize
                    && m.FocalLength == photo.CameraSettings.FocalLength
                    && m.GeolocationLattitude == photo.GeolocationLatitude
                    && m.GeolocationLongitude == photo.GeolocationLongitude
                    && m.ISO == photo.CameraSettings.ISO
                    && m.Manufacturer == photo.CameraSettings.Manufacturer
                    && m.Orientation == photo.ImageProperties.Orientation;
            };

            var service = new PhotoService(
                photoMetadataStorage.Object,
                photoFileStorage.Object,
                photoInAlbumStorage.Object);

            await service.SavePhotoAsync(photo);

            photoMetadataStorage.Verify(s => s.SavePhotoAsync(It.Is<PhotoMetadata>(m => verify(m))), Times.Once);
        }

        [Test]
        public async Task DeletePhotoAsync_PhotoIdProvided_DeletesThumbnail()
        {
            var photo = GetSamplePhoto();

            var service = new PhotoService(
                photoMetadataStorage.Object,
                photoFileStorage.Object,
                photoInAlbumStorage.Object);

            await service.DeletePhotoAsync(photo);

            photoFileStorage.Verify(f => f.DeleteThumbnailAsync(photo.PhotoId), Times.Once);
        }

        [Test]
        public async Task DeletePhotoAsync_PhotoIdProvided_DeletesPhotoFile()
        {
            var photo = GetSamplePhoto();

            var service = new PhotoService(
                photoMetadataStorage.Object,
                photoFileStorage.Object,
                photoInAlbumStorage.Object);

            await service.DeletePhotoAsync(photo);

            photoFileStorage.Verify(f => f.DeletePhotoAsync(photo.PhotoId), Times.Once);
        }

        [Test]
        public async Task DeletePhotoAsync_PhotoIdProvided_DeletesMetadata()
        {
            var photo = GetSamplePhoto();

            string partitionKey = photo.DateTaken.ToPartitionKey();

            var service = new PhotoService(
                photoMetadataStorage.Object,
                photoFileStorage.Object,
                photoInAlbumStorage.Object);

            await service.DeletePhotoAsync(photo);

            photoMetadataStorage.Verify(f => f.DeletePhotoAsync(partitionKey, photo.PhotoId), Times.Once);
        }

        [Test]
        public async Task DeletePhotoAsync_PhotoIdProvided_RemovesPhotoFromAlbums()
        {
            var photo = GetSamplePhoto();

            var service = new PhotoService(
                photoMetadataStorage.Object,
                photoFileStorage.Object,
                photoInAlbumStorage.Object);

            await service.DeletePhotoAsync(photo);

            photoInAlbumStorage.Verify(f => f.RemoveFromAllAlbumsAsync(photo.PhotoId), Times.Once);
        }

        [Test]
        public async Task GetPhotosInAlbumAsync_AlbumIdProvided_PhotosLookedUpFromAlbum()
        {
            string albumId = "albumId";

            var photoInAlbum = new PhotoInAlbum
            {
                PartitionKey = albumId,
                RowKey = "photoId"
            };

            photoInAlbumStorage.Setup(p => p.GetPhotosInAlbumAsync(albumId)).Returns(photoInAlbum.AsAsyncPageable());

            var metadata = new PhotoMetadata { PartitionKey = "pk", RowKey = "rk" };

            photoMetadataStorage.Setup(s => s.GetPhotoMetadataAsync(It.IsAny<DateTime>(), It.IsAny<string>())).Returns(Task.FromResult(metadata));

            var service = new PhotoService(
                photoMetadataStorage.Object,
                photoFileStorage.Object,
                photoInAlbumStorage.Object);

            await foreach (var photo in service.GetPhotosInAlbumAsync(albumId))
            {
                _ = photo;
            }

            photoInAlbumStorage.Verify(s => s.GetPhotosInAlbumAsync(albumId), Times.Once);
        }

        private Photo GetSamplePhoto()
        {
            var date = DateTime.SpecifyKind(new DateTime(2000, 1, 1), DateTimeKind.Utc);
            var imageProperties = new ImageProperties(123, new System.Drawing.Size(800, 600), "Title", "Description", date, 4, "hash");
            var cameraSettings = new CameraSettings("iso", "aperture", "fl", "device", "manufacture", "exposure");
            var geoloaction = new Geolocation(1.23, -0.50);
            return new Photo("photoId", imageProperties, geoloaction, cameraSettings);
        }
    }
}
