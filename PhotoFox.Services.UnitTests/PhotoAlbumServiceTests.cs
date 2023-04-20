using Azure;
using Moq;
using PhotoFox.Extensions;
using PhotoFox.Storage.Models;
using PhotoFox.Storage.Table;
using PhotoAlbum = PhotoFox.Storage.Models.PhotoAlbum;

namespace PhotoFox.Services.UnitTests
{
    public class PhotoAlbumServiceTests
    {
        private Mock<IPhotoAlbumDataStorage> photoAlbumDataStorage;

        private Mock<IPhotoInAlbumStorage> photoInAlbumStorage;

        private Mock<IPhotoMetadataStorage> photoMetadataStorage;

        private Mock<IAlbumPermissionStorage> albumPermissionStorage;

        [SetUp]
        public void Setup()
        {
            photoAlbumDataStorage= new Mock<IPhotoAlbumDataStorage>();
            photoInAlbumStorage = new Mock<IPhotoInAlbumStorage>();
            photoMetadataStorage = new Mock<IPhotoMetadataStorage>();
            albumPermissionStorage = new Mock<IAlbumPermissionStorage>();
        }

        [Test]
        public async Task GetAllAlbumsAsync_Invoked_ReturnsAllAlbums()
        {
            photoAlbumDataStorage.Setup(s => s.GetPhotoAlbumsAsync()).Returns(GetTestData());

            var service = GetDefaultService();

            var results = await AsyncEnumerableToArray(service.GetAllAlbumsAsync());

            Assert.That(results.Count, Is.EqualTo(15));
            Assert.That(results.First().Title, Is.EqualTo("Album 1"));
            Assert.That(results.Last().Title, Is.EqualTo("Album 15"));
        }

        [Test]
        public async Task GetAllAlbumsAsync_UsernamePassed_ReturnsAlbumsWithPermission()
        {
            var username = "USERNAME";
            var albumId = "ALBUM-0001";

            var permission = new AlbumPermission
            {
                PartitionKey = username,
                RowKey = albumId
            };

            photoAlbumDataStorage.Setup(s => s.GetPhotoAlbumsAsync()).Returns(GetTestData());
            albumPermissionStorage.Setup(s => s.GetPermissionsByUsernameAsync(username)).Returns(permission.AsAsyncPageable());

            var service = GetDefaultService();

            var results = await AsyncEnumerableToArray<Model.PhotoAlbum>(service.GetAllAlbumsAsync(username));

            Assert.That(results.Count, Is.EqualTo(1));
            Assert.That(results.First().AlbumId, Is.EqualTo(albumId));
        }

        [Test]
        public async Task GetAllAlbums_UsernameProvided_DoesNotReturnAlbumsWithoutPermission()
        {
            var username = "USERNAME";
            var albumId = "ALBUM-0001";

            var permission = new AlbumPermission
            {
                PartitionKey = username,
                RowKey = albumId
            };

            photoAlbumDataStorage.Setup(s => s.GetPhotoAlbumsAsync()).Returns(GetTestData());
            albumPermissionStorage.Setup(s => s.GetPermissionsByUsernameAsync(username)).Returns(permission.AsAsyncPageable());

            var service = GetDefaultService();

            var results = await AsyncEnumerableToArray<Model.PhotoAlbum>(service.GetAllAlbumsAsync(username));

            Assert.That(results.Count, Is.GreaterThan(0));
            Assert.That(results.Any(r => r.AlbumId == "ALBUM-0002"), Is.False);
        }

        [Test]
        public async Task GetPhotosInAlbumAsync_AlbumIdProvided_ReturnsPhotosInAlbum()
        {
            var albumId = "ALBUM-0001";
            var photoId = "PHOTO-0001";
            var date = DateTime.SpecifyKind(new DateTime(2000, 1, 1), DateTimeKind.Utc);

            var photoInAlbum = new PhotoInAlbum
            {
                PartitionKey = albumId,
                RowKey = photoId,
                UtcDate = date
            };

            var photoMetadata = new PhotoMetadata
            {
                PartitionKey = date.ToPartitionKey(),
                RowKey = photoId,
                Title = "Title"
            };

            this.photoInAlbumStorage.Setup(s => s.GetPhotosInAlbumAsync(albumId)).Returns(photoInAlbum.AsAsyncPageable());
            this.photoMetadataStorage.Setup(s => s.GetPhotoMetadataAsync(date, photoId)).Returns(Task.FromResult(photoMetadata));
            var service = GetDefaultService();

            var results = await AsyncEnumerableToArray(service.GetPhotosInAlbumAsync(albumId));

            Assert.That(results.Count, Is.EqualTo(1));
            Assert.That(results.First().PhotoId, Is.EqualTo(photoId));
        }

        [Test]
        public async Task AddAlbumAsync_Invoked_AddsAlbumToStorage()
        {
            var service = this.GetDefaultService();

            var album = new Model.PhotoAlbum("ALBUMID", "Title", string.Empty, string.Empty, string.Empty, string.Empty);

            await service.AddAlbumAsync(album);

            photoAlbumDataStorage.Verify(d => d.AddPhotoAlbumAsync(It.IsAny<PhotoAlbum>()));
        }

        [Test]
        public async Task AddPhotoToAlbumAsync_Invoked_AddsPhotoToAlbum()
        {
            string albumId = "albumid";
            string photoId = "photoid";

            var date = DateTime.SpecifyKind(new DateTime(2000, 1, 1), DateTimeKind.Utc);

            var service = this.GetDefaultService();

            await service.AddPhotoToAlbumAsync(albumId, photoId, date);

            photoInAlbumStorage.Verify(a => a.AddPhotoInAlbumAsync(albumId, photoId, date), Times.Once);
        }

        private PhotoAlbumService GetDefaultService()
        {
            return new PhotoAlbumService(
                photoAlbumDataStorage.Object,
                photoInAlbumStorage.Object,
                photoMetadataStorage.Object,
                albumPermissionStorage.Object);
        }

        private async Task<T[]> AsyncEnumerableToArray<T>(IAsyncEnumerable<T> values)
        {
            var result = new List<T>();
            await foreach (var val in values)
            {
                result.Add(val);
            }

            return result.ToArray();
        }

        private AsyncPageable<PhotoAlbum> GetTestData()
        {
            var page1 = Page<PhotoAlbum>.FromValues(Enumerable.Range(1, 5).Select(r => GetTestAlbum(r)).ToArray(), "1", Mock.Of<Response>());
            var page2 = Page<PhotoAlbum>.FromValues(Enumerable.Range(6, 5).Select(r => GetTestAlbum(r)).ToArray(), "2", Mock.Of<Response>());
            var lastPage = Page<PhotoAlbum>.FromValues(Enumerable.Range(11, 5).Select(r => GetTestAlbum(r)).ToArray(), null, Mock.Of<Response>());

            return AsyncPageable<PhotoAlbum>.FromPages(new[] { page1, page2, lastPage });
        }

        private PhotoAlbum GetTestAlbum(int index)
        {
            var key = index.ToString("D4");
            return new PhotoAlbum
            {
                PartitionKey = $"ALBUM-{key}",  // Produces sequential keys in the format ALBUM-0001
                RowKey = string.Empty,
                AlbumDescription = string.Empty,
                AlbumName = $"Album {index}",
                CoverPhotoId= Guid.NewGuid().ToString()
            };
        }
    }
}
