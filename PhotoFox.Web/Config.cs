﻿using PhotoFox.Storage;

namespace PhotoFox.Web
{
    public class Config : IStorageConfig
    {
        public string StorageConnectionString => "DefaultEndpointsProtocol=https;AccountName=photofox;AccountKey=LTIetKvBGKAqSvnpnqNe05ObOWoLW6P+uMXgd01DEN26eekmgJtFB8MTi+jjZshXh/RXb6i2weQoiosBjlMq7A==;EndpointSuffix=core.windows.net";
    }
}