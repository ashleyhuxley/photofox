using AutoMapper;
using Azure;
using Moq;
using PhotoFox.Core.Extensions;
using PhotoFox.Mappings;
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
        private IMapper mapper;

        [SetUp]
        public void Setup()
        {
            photoMetadataStorage = new Mock<IPhotoMetadataStorage>();
            photoFileStorage = new Mock<IPhotoFileStorage>();
            photoInAlbumStorage = new Mock<IPhotoInAlbumStorage>();

            // Use the real mapper otherwise we'll just be recreating the mapper functionality in a mock
            mapper = MapFactory.GetMap();
        }

        [Test]
        public void Constructor_NullParameters_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new PhotoService(null, photoFileStorage.Object, photoInAlbumStorage.Object, mapper));
            Assert.Throws<ArgumentNullException>(() => _ = new PhotoService(photoMetadataStorage.Object, null, photoInAlbumStorage.Object, mapper));
            Assert.Throws<ArgumentNullException>(() => _ = new PhotoService(photoMetadataStorage.Object, photoFileStorage.Object, null, mapper));
            Assert.Throws<ArgumentNullException>(() => _ = new PhotoService(photoMetadataStorage.Object, photoFileStorage.Object, photoInAlbumStorage.Object, null));
        }

        [Test]
        public async Task GetPhotoAsync_ValidArgsDateTimeOverload_GetsPhotoFromStorage()
        {
            var date = new DateTime(2000, 1, 1);
            var photoId = "photoId";

            var service = new PhotoService(
                photoMetadataStorage.Object, 
                photoFileStorage.Object, 
                photoInAlbumStorage.Object, 
                mapper);

            await service.GetPhotoAsync(date, photoId);

            photoMetadataStorage.Verify(s => s.GetPhotoMetadataAsync(date, photoId), Times.Once);
        }

        [Test]
        public async Task GetPhotoAsync_ValidArgsStringOverload_GetsPhotoFromStorage()
        {
            var date = DateTime.SpecifyKind(new DateTime(2000, 1, 1), DateTimeKind.Utc).ToPartitionKey();
            var photoId = "photoId";

            var service = new PhotoService(
                photoMetadataStorage.Object,
                photoFileStorage.Object,
                photoInAlbumStorage.Object,
                mapper);

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
                    && m.Aperture == photo.Aperture
                    && m.Description == photo.Description
                    && m.Device == photo.Device
                    && m.Exposure == photo.Exposure
                    && m.DimensionWidth == photo.DimensionWidth
                    && m.DimensionHeight == photo.DimensionHeight
                    && m.FileHash == photo.FileHash
                    && m.FileSize == photo.FileSize
                    && m.FocalLength == photo.FocalLength
                    && m.GeolocationLattitude == photo.GeolocationLatitude
                    && m.GeolocationLongitude == photo.GeolocationLongitude
                    && m.ISO == photo.ISO
                    && m.Manufacturer == photo.Manufacturer
                    && m.Orientation == photo.Orientation;
            };

            var service = new PhotoService(
                photoMetadataStorage.Object,
                photoFileStorage.Object,
                photoInAlbumStorage.Object,
                mapper);

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
                photoInAlbumStorage.Object,
                mapper);

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
                photoInAlbumStorage.Object,
                mapper);

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
                photoInAlbumStorage.Object,
                mapper);

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
                photoInAlbumStorage.Object,
                mapper);

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

            var service = new PhotoService(
                photoMetadataStorage.Object,
                photoFileStorage.Object,
                photoInAlbumStorage.Object,
                mapper);

            await foreach (var photo in service.GetPhotosInAlbumAsync(albumId))
            {
                _ = photo;
            }

            photoInAlbumStorage.Verify(s => s.GetPhotosInAlbumAsync(albumId), Times.Once);
        }

        private Photo GetSamplePhoto()
        {
            return new Photo
            {
                PhotoId = "photoId",
                DateTaken = DateTime.SpecifyKind(new DateTime(2000, 1, 1), DateTimeKind.Utc),
                Title = "Title",
                Aperture = "Aperture",
                Description = "Description",
                Device = "Device",
                Exposure = "Exposure",
                DimensionHeight = 10,
                DimensionWidth = 20,
                FileHash = "FileHash",
                FileSize = 123,
                FocalLength = "FocalLength",
                GeolocationLatitude = 1.23,
                GeolocationLongitude = 2.34,
                ISO = "iso",
                Manufacturer = "Manufacturer",
                Orientation = 4
            };
        }
    }
}
