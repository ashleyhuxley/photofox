using PhotoFox.Storage.Table;
using System;
using System.Threading.Tasks;

namespace PhotoFox.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAlbumPermissionStorage albumPermissionStorage;

        public AuthService(IAlbumPermissionStorage albumPermissionStorage)
        {
            this.albumPermissionStorage = albumPermissionStorage ?? throw new ArgumentNullException(nameof(albumPermissionStorage));
        }

        public async Task<bool> HasPermission(string albumId, string username)
        {
            return await this.albumPermissionStorage.HasPermissionAsync(albumId, username).ConfigureAwait(false);
        }
    }
}
