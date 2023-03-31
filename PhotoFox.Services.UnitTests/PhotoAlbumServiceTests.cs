using AutoMapper;
using Azure;
using Moq;
using PhotoFox.Storage.Models;
using PhotoFox.Storage.Table;

namespace PhotoFox.Services.UnitTests
{
    public class PhotoAlbumServiceTests
    {
        private Mock<IPhotoAlbumDataStorage> photoAlbumDataStorage;

        private Mock<IPhotoInAlbumStorage> photoInAlbumStorage;

        private Mock<IPhotoMetadataStorage> photoMetadataStorage;

        private Mock<IAlbumPermissionStorage> albumPermissionStorage;

        private Mock<IMapper> mapper;

        [SetUp]
        public void Setup()
        {
            photoAlbumDataStorage= new Mock<IPhotoAlbumDataStorage>();
            photoInAlbumStorage = new Mock<IPhotoInAlbumStorage>();
            photoMetadataStorage = new Mock<IPhotoMetadataStorage>();
            albumPermissionStorage = new Mock<IAlbumPermissionStorage>();
            mapper = new Mock<IMapper>();
        }

        [Test]
        public void Constructor_NullArgumentsProvided_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new PhotoAlbumService(null, photoInAlbumStorage.Object, photoMetadataStorage.Object, albumPermissionStorage.Object, mapper.Object));
            Assert.Throws<ArgumentNullException>(() => new PhotoAlbumService(photoAlbumDataStorage.Object, null, photoMetadataStorage.Object, albumPermissionStorage.Object, mapper.Object));
            Assert.Throws<ArgumentNullException>(() => new PhotoAlbumService(photoAlbumDataStorage.Object, photoInAlbumStorage.Object, null, albumPermissionStorage.Object, mapper.Object));
            Assert.Throws<ArgumentNullException>(() => new PhotoAlbumService(photoAlbumDataStorage.Object, photoInAlbumStorage.Object, photoMetadataStorage.Object, null, mapper.Object));
            Assert.Throws<ArgumentNullException>(() => new PhotoAlbumService(photoAlbumDataStorage.Object, photoInAlbumStorage.Object, photoMetadataStorage.Object, albumPermissionStorage.Object, null));
        }

        [Test]
        public async Task GetAllAlbumsAsync_Invoked_ReturnsAllAlbums()
        {
            photoAlbumDataStorage.Setup(s => s.GetPhotoAlbumsAsync()).Returns(GetTestData());

            var service = new PhotoAlbumService(
                photoAlbumDataStorage.Object,
                photoInAlbumStorage.Object,
                photoMetadataStorage.Object,
                albumPermissionStorage.Object,
                mapper.Object);

            var results = await AsyncEnumerableToArray<Model.PhotoAlbum>(service.GetAllAlbumsAsync());

            Assert.That(results.Count, Is.EqualTo(15));
            Assert.That(results.First().Title, Is.EqualTo("Album 1"));
            Assert.That(results.Last().Title, Is.EqualTo("Album 15"));
        }

        [Test]
        public void GetAllAlbums_UsernameNull_ThrowsArgumentNullException()
        {
            var service = new PhotoAlbumService(
                photoAlbumDataStorage.Object,
                photoInAlbumStorage.Object,
                photoMetadataStorage.Object,
                albumPermissionStorage.Object,
                mapper.Object);

            Assert.Throws<ArgumentNullException>(() => _ = service.GetAllAlbumsAsync(null));
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
            albumPermissionStorage.Setup(s => s.GetPermissionsByUsernameAsync(username)).Returns(GetSingleItemAsAsyncPageable(permission));

            var service = new PhotoAlbumService(
                photoAlbumDataStorage.Object,
                photoInAlbumStorage.Object,
                photoMetadataStorage.Object,
                albumPermissionStorage.Object,
                mapper.Object);

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
            albumPermissionStorage.Setup(s => s.GetPermissionsByUsernameAsync(username)).Returns(GetSingleItemAsAsyncPageable(permission));

            var service = new PhotoAlbumService(
                photoAlbumDataStorage.Object,
                photoInAlbumStorage.Object,
                photoMetadataStorage.Object,
                albumPermissionStorage.Object,
                mapper.Object);

            var results = await AsyncEnumerableToArray<Model.PhotoAlbum>(service.GetAllAlbumsAsync(username));

            Assert.That(results.Count, Is.GreaterThan(0));
            Assert.That(results.Any(r => r.AlbumId == "ALBUM-0002"), Is.False);
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

        private AsyncPageable<T> GetSingleItemAsAsyncPageable<T>(T item)
            where T: notnull
        {
            var page = Page<T>.FromValues(new[] { item }, null, Mock.Of<Response>());
            return AsyncPageable<T>.FromPages(new[] { page });
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
