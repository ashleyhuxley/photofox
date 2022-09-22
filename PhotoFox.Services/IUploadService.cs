using System.IO;
using System.Threading.Tasks;
using System;

namespace PhotoFox.Services
{
    public interface IUploadService
    {
        Task UploadFromStreamAsync(Stream stream, DateTime fallbackTime);
    }
}
