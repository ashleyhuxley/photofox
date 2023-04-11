using System;
using System.IO;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using NLog;
using PhotoFox.Model;
using PhotoFox.Storage.Blob;
using PhotoFox.Storage.Models;
using PhotoFox.Storage.Queue;

namespace PhotoFox.Services
{
    public class UploadService : IUploadService
    {
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        private readonly IPhotoFileStorage photoFileStorage;

        private readonly IVideoStorage videoStorage;

        private readonly IUploadQueue uploadQueue;

        public UploadService(
            IPhotoFileStorage photoFileStorage,
            IVideoStorage videoStorage,
            IUploadQueue uploadQueue)
        {
            this.uploadQueue= uploadQueue ?? throw new ArgumentNullException(nameof(uploadQueue));
            this.photoFileStorage = photoFileStorage ?? throw new ArgumentNullException(nameof(photoFileStorage));
            this.videoStorage = videoStorage ?? throw new ArgumentNullException(nameof(videoStorage));
        }

        [SupportedOSPlatform("windows")]
        public async Task UploadFromStreamAsync(Stream stream, string albumId, string fallbackTitle, string fileExt, DateTime createdDate)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

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

            await this.photoFileStorage.PutPhotoAsync(photoId, binaryData).ConfigureAwait(false);

            await this.uploadQueue.QueueUploadMessage(message);
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

            await this.videoStorage.PutVideoAsync(videoId, binaryData).ConfigureAwait(false);

            await this.uploadQueue.QueueUploadMessage(message);
        }
    }
}
