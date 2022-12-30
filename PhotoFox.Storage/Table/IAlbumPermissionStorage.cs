using PhotoFox.Storage.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhotoFox.Storage.Table
{
    public interface IAlbumPermissionStorage
    {
        Task<bool> HasPermissionAsync(string albumId, string username);
        IAsyncEnumerable<AlbumPermission> GetPermissionsByUsernameAsync(string username);
    }
}
