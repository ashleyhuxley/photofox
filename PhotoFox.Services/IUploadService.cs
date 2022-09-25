using System.IO;
using System.Threading.Tasks;
using System;
using PhotoFox.Model;

namespace PhotoFox.Services
{
    public interface IUploadService
    {
        Task<PhotoMetadata> UploadFromStreamAsync(Stream stream, DateTime fallbackTime, string fallbackTitle);
    }
}
