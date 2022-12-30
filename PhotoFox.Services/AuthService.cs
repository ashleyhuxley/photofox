using PhotoFox.Storage.Table;
using System.Threading.Tasks;

namespace PhotoFox.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAlbumPermissionStorage albumPermissionStorage;

        public AuthService(IAlbumPermissionStorage albumPermissionStorage)
        {
            this.albumPermissionStorage = albumPermissionStorage;
        }

        public async Task<bool> HasPermission(string albumId, string username)
        {
            return await this.albumPermissionStorage.HasPermissionAsync(albumId, username).ConfigureAwait(false);
        }
    }
}
