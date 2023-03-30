using PhotoFox.Storage;
using System;

namespace PhotoFox.Functions.UploadPhoto
{
    internal class Config : IStorageConfig
    {
        public string StorageConnectionString => Environment.GetEnvironmentVariable("PhotoFoxStorage");
    }
}
