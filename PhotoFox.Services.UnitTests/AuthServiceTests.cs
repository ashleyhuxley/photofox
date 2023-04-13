using Moq;
using PhotoFox.Storage.Table;

namespace PhotoFox.Services.UnitTests
{
    public class AuthServiceTests
    {
        private Mock<IAlbumPermissionStorage> albumPermissionStorage;

        [SetUp]
        public void Setup()
        {
            albumPermissionStorage = new Mock<IAlbumPermissionStorage>();
        }

        [Test]
        public async Task AuthService_HasPermission_CallsHasPermissionWithCorrectArgs()
        {
            string albumId = "albumId";
            string username = "username";

            var service = new AuthService(albumPermissionStorage.Object);
            await service.HasPermission(albumId, username);

            albumPermissionStorage.Verify(a => a.HasPermissionAsync(albumId, username), Times.Once());
        }
    }
}