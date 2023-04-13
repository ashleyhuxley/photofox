using System;
using System.IO;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using NLog;
using PhotoFox.Storage.Blob;
using PhotoFox.Storage.Models;
using PhotoFox.Storage.Queue;

namespace PhotoFox.Services
{
    public class UploadService : IUploadService
    {
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        private readonly IUploadQueue uploadQueue;

        private readonly IUploadStorage uploadStorage;

        public UploadService(
            IUploadQueue uploadQueue,
            IUploadStorage uploadStorage)
        {
            this.uploadQueue = uploadQueue;
            this.uploadStorage = uploadStorage;
        }

        [SupportedOSPlatform("windows")]
        public async Task UploadFromStreamAsync(Stream stream, string albumId, string fallbackTitle, string fileExt, DateTime createdDate)
        {
            var photoId = Guid.NewGuid().ToString();
            Log.Info($"Uploading photo {photoId}");

            var message = new UploadMessage
            {
                Album = albumId,
                DateTaken = createdDate,
                EntityId = photoId,
                FileExt = fileExt,
                Title = fallbackTitle,
                Type = "PHOTO"
            };

            var binaryData = await BinaryData.FromStreamAsync(stream).ConfigureAwait(false);

            await this.uploadStorage.PutFileAsync(photoId, binaryData).ConfigureAwait(false);

            await this.uploadQueue.QueueUploadMessageAsync(message).ConfigureAwait(false);
        }

        public async Task UploadVideoFromStreamAsync(Stream stream, string albumId, string fallbackTitle, string fileExt, DateTime createdDate)
        {
            var videoId = Guid.NewGuid().ToString();
            Log.Info($"Uploading video {videoId}");

            var message = new UploadMessage
            {
                Album = albumId,
                DateTaken = createdDate,
                EntityId = videoId,
                FileExt = fileExt,
                Title = fallbackTitle,
                Type = "VIDEO"
            };

            var binaryData = await BinaryData.FromStreamAsync(stream).ConfigureAwait(false);

            await this.uploadStorage.PutFileAsync(videoId, binaryData).ConfigureAwait(false);

            await this.uploadQueue.QueueUploadMessageAsync(message).ConfigureAwait(false);
        }
    }
}
