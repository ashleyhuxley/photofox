using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using PhotoFox.Core.Hashing;
using PhotoFox.Core.Imaging;
using PhotoFox.Functions.UploadPhoto;
using PhotoFox.Storage;
using PhotoFox.Storage.Blob;
using PhotoFox.Storage.Table;
using System.Runtime.Versioning;

[assembly: FunctionsStartup(typeof(MyNamespace.Startup))]

namespace MyNamespace
{
    public class Startup : FunctionsStartup
    {
        [SupportedOSPlatform("windows")]
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<IPhotoFileStorage, PhotoFileStorage>();
            builder.Services.AddSingleton<IStreamHash, StreamHashMD5>();
            builder.Services.AddSingleton<IPhotoHashStorage, PhotoHashStorage>();
            builder.Services.AddSingleton<IThumbnailProvider, ThumbnailProvider>();
            builder.Services.AddSingleton<IPhotoMetadataStorage, PhotoMetadataStorage>();
            builder.Services.AddSingleton<IPhotoInAlbumStorage, PhotoInAlbumStorage>();
            builder.Services.AddSingleton<ILogStorage, LogStorage>();
            builder.Services.AddSingleton<IStorageConfig, Config>();
            builder.Services.AddSingleton<IVideoInAlbumStorage, VideoInAlbumStorage>();
            builder.Services.AddSingleton<IVideoStorage, VideoStorage>();
        }
    }
}
