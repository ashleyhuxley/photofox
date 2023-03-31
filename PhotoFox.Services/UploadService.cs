using System;
using System.IO;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using NLog;
using PhotoFox.Storage.Blob;
using PhotoFox.Storage.Queue;

namespace PhotoFox.Services
{
    public class UploadService : IUploadService
    {
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        private readonly IPhotoFileStorage photoFileStorage;

        private readonly IUploadQueue uploadQueue;

        public UploadService(
            IPhotoFileStorage photoFileStorage,
            IUploadQueue uploadQueue)
        {
            this.uploadQueue= uploadQueue;
            this.photoFileStorage = photoFileStorage;
        }

        [SupportedOSPlatform("windows")]
        public async Task UploadFromStreamAsync(Stream stream, string albumId, string fallbackTitle)
        {
            var photoId = Guid.NewGuid().ToString();
            Log.Info($"Uploading photo {photoId}");

            var binaryData = await BinaryData.FromStreamAsync(stream).ConfigureAwait(false);

            await this.photoFileStorage.PutPhotoAsync(photoId, binaryData).ConfigureAwait(false);

            await this.uploadQueue.QueueUploadMessage(photoId, albumId, fallbackTitle);
        }
    }
}
