using System.Threading.Tasks;

namespace PhotoFox.Services
{
    public interface IAuthService
    {
        Task<bool> HasPermission(string albumId, string username);
    }
}
