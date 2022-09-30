using System.IO;
using System.Threading.Tasks;
using System;
using PhotoFox.Model;

namespace PhotoFox.Services
{
    public interface IUploadService
    {
        Task<Photo> UploadFromStreamAsync(Stream stream, DateTime fallbackTime, string fallbackTitle);
    }
}
