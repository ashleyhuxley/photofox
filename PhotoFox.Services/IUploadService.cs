using System.IO;
using System.Threading.Tasks;

namespace PhotoFox.Services
{
    public interface IUploadService
    {
        Task UploadFromStreamAsync(Stream stream, string albumId, string fallbackTitle);
    }
}
