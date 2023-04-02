using PhotoFox.Storage;
using System;
using System.Configuration;

namespace PhotoFox.Functions.UploadPhoto
{
    internal class Config : IStorageConfig
    {
        public string StorageConnectionString => Environment.GetEnvironmentVariable("PhotoFoxStorage") ?? throw new ConfigurationErrorsException("PhotoFoxStorage connection string missing from config");
    }
}
