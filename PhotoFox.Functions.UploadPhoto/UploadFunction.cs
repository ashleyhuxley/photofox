using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using PhotoFox.Core.Exif;
using PhotoFox.Core.Hashing;
using PhotoFox.Storage.Blob;
using PhotoFox.Storage.Models;
using PhotoFox.Storage.Table;
using System.IO;
using System;
using System.Threading.Tasks;
using PhotoFox.Core.Imaging;
using System.Drawing;
using PhotoFox.Core.Extensions;
using System.Drawing.Imaging;
using LogLevel = PhotoFox.Storage.Models.LogLevel;
using System.Runtime.Versioning;
using System.Text.Json;
using PhotoFox.Model;
using NLog.LayoutRenderers;

namespace PhotoFox.Functions.UploadPhoto
{
    [SupportedOSPlatform("windows")]
    public class UploadFunction
    {
        private readonly IPhotoFileStorage photoFileStorage;
        private readonly IStreamHash streamHash;
        private readonly IPhotoHashStorage photoHashStorage;
        private readonly IThumbnailProvider thumbnailProvider;
        private readonly IPhotoMetadataStorage photoMetadataStorage;
        private readonly IPhotoInAlbumStorage photoInAlbumStorage;
        private readonly ILogStorage logStorage;
        private readonly IVideoInAlbumStorage videoInAlbumStorage;
        private readonly IVideoStorage videoStorage;
        private readonly IUploadStorage uploadStorage;

        private const string source = "UploadFunction";

        public UploadFunction(
            IPhotoFileStorage photoFileStorage,
            IStreamHash streamHash,
            IPhotoHashStorage photoHashStorage,
            IThumbnailProvider thumbnailProvider,
            IPhotoMetadataStorage photoMetadataStorage,
            IPhotoInAlbumStorage photoInAlbumStorage,
            IVideoInAlbumStorage videoInAlbumStorage,
            ILogStorage logStorage,
            IVideoStorage videoStorage,
            IUploadStorage uploadStorage)
        {
            this.photoFileStorage = photoFileStorage ?? throw new ArgumentNullException(nameof(photoFileStorage));
            this.streamHash = streamHash ?? throw new ArgumentNullException(nameof(streamHash));
            this.photoHashStorage = photoHashStorage ?? throw new ArgumentNullException(nameof(photoHashStorage));
            this.thumbnailProvider = thumbnailProvider ?? throw new ArgumentNullException(nameof(thumbnailProvider));
            this.photoMetadataStorage = photoMetadataStorage ?? throw new ArgumentNullException(nameof(photoMetadataStorage));
            this.photoInAlbumStorage = photoInAlbumStorage ?? throw new ArgumentNullException(nameof(photoInAlbumStorage));
            this.videoInAlbumStorage = videoInAlbumStorage ?? throw new ArgumentNullException(nameof(videoInAlbumStorage));
            this.logStorage = logStorage ?? throw new ArgumentNullException(nameof(logStorage));
            this.videoStorage = videoStorage ?? throw new ArgumentNullException(nameof(videoStorage));
            this.uploadStorage = uploadStorage ?? throw new ArgumentNullException(nameof(uploadStorage));
        }

        [FunctionName("Upload")]
        public async Task Run([QueueTrigger("uploads", Connection = "PhotoFoxStorage")]string message, ILogger log)
        {
            var uploadMessage = JsonSerializer.Deserialize<UploadMessage>(message);
            if (uploadMessage == null)
            {
                throw new ArgumentException("The input message could not be deserialized", nameof(message));
            }

            // Get the photo from storage
            var blob = await uploadStorage.GetFileAsync(uploadMessage.EntityId);
            if (blob == null)
            {
                var errorMessage = $"File not found in blob storage: {uploadMessage.EntityId}";
                log.LogError(errorMessage);
                throw new FileNotFoundException(errorMessage);
            }

            var albumId = uploadMessage.Album ?? Guid.Empty.ToString();
            var title = uploadMessage.Title ?? uploadMessage.DateTaken.ToString("yyyy-MM-dd HH:mm:ss");

            log.LogInformation($"Processing entity ID: {uploadMessage.EntityId} into album {uploadMessage.Album} as {uploadMessage.Type}");

            switch (uploadMessage.Type.ToUpperInvariant())
            {
                case "PHOTO":
                    await ProcessPhoto(blob, uploadMessage.EntityId, albumId, title, uploadMessage.DateTaken, log);
                    break;
                case "VIDEO":
                    if (uploadMessage.FileExt == null)
                    {
                        throw new InvalidOperationException("Cannot upload video with unpecified file extension");
                    }

                    await ProcessVideo(blob, uploadMessage.EntityId, albumId, title, uploadMessage.DateTaken, uploadMessage.FileExt, log);
                    break;
                default:
                    throw new ArgumentException($"Unknown Message Type: {uploadMessage.Type}");
            }
        }

        private async Task ProcessPhoto(BinaryData blob, string photoId, string albumId, string title, DateTime dateTaken, ILogger log)
        {
            var stream = blob.ToStream();

            // Check hash
            var md5 = await Task.Run(() => streamHash.ComputeHash(stream));
            if (await photoHashStorage.HashExistsAsync(md5).ConfigureAwait(false) != null)
            {
                log.LogWarning($"An image with the same hash as {photoId} already exists");

                await logStorage.Log("An image with this hash already exists", source, photoId, albumId, LogLevel.Warn, md5);

                await uploadStorage.DeleteFileAsync(photoId);
                return;
            }

            // Get EXIF data
            var exifReader = await ExifReader.FromStreamAsync(stream).ConfigureAwait(false);
            var metadata = new PhotoMetadata();
            metadata.Orientation = exifReader.GetOrientation();

            var image = Image.FromStream(stream);
            var thumbnail = await Task.Run(() => thumbnailProvider.GenerateThumbnail(image, 250, metadata.Orientation.ToRotationDegrees())).ConfigureAwait(false);

            metadata.FocalLength = exifReader.GetFocalLength();
            metadata.Device = exifReader.GetModel();
            metadata.Aperture = exifReader.GetApeture();
            metadata.Exposure = exifReader.GetExposure();
            metadata.GeolocationLattitude = exifReader.GetGpsLatitude();
            metadata.GeolocationLongitude = exifReader.GetGpsLongitude();
            metadata.ISO = exifReader.GetIso();
            metadata.Title = title;
            metadata.DimensionWidth = exifReader.GetDimensionWidth() ?? image.Width;
            metadata.DimensionHeight = exifReader.GetDimensionHeight() ?? image.Height;
            metadata.Manufacturer = exifReader.GetManufacturer();
            metadata.FileSize = blob.ToArray().Length;

            var date = exifReader.GetDateTakenUtc() ?? dateTaken;
            metadata.PartitionKey = date.ToPartitionKey();
            metadata.UtcDate = date;
            metadata.RowKey = photoId;
            metadata.FileHash = md5;

            log.LogTrace($"PK: {metadata.PartitionKey}, RK: {metadata.RowKey}");

            // Upload thumbnail
            using (var ms = new MemoryStream())
            {
                thumbnail.Save(ms, ImageFormat.Jpeg);

                ms.Seek(0, SeekOrigin.Begin);
                var data = await BinaryData.FromStreamAsync(ms);
                await photoFileStorage.PutThumbnailAsync(metadata.RowKey, data);
            }

            // Store blob
            await photoFileStorage.PutPhotoAsync(photoId, blob);

            // Store table items
            await photoMetadataStorage.AddPhotoAsync(metadata);
            await logStorage.Log("Metadata entry added", source, photoId, "", LogLevel.Info, md5);

            await photoHashStorage.AddHashAsync(md5, metadata.PartitionKey, metadata.RowKey);
            await logStorage.Log("Photo hash added", source, photoId, "", LogLevel.Info, md5);

            await photoInAlbumStorage.AddPhotoInAlbumAsync(albumId, metadata.RowKey, metadata.UtcDate.Value);
            await logStorage.Log("Photo added to album", source, photoId, albumId, LogLevel.Info, md5);

            // Delete temp file
            await uploadStorage.DeleteFileAsync(photoId);
        }

        private async Task ProcessVideo(
            BinaryData blob, 
            string videoId, 
            string albumId, 
            string title, 
            DateTime dateTaken, 
            string fileExt,
            ILogger log)
        {
            var videoInAlbum = new VideoInAlbum
            {
                FileSize = blob.ToArray().Length,
                PartitionKey = albumId,
                RowKey = videoId,
                Title = title,
                VideoDate = DateTime.SpecifyKind(dateTaken, DateTimeKind.Utc),
                FileExt = fileExt
            };

            log.LogInformation($"Moving video {videoId} to video storage and creating record...");
            await videoStorage.PutVideoAsync(videoId, blob);
            await videoInAlbumStorage.AddVideoInAlbumAsync(videoInAlbum);

            log.LogInformation($"Deleting video {videoId} from temp storage...");
            await uploadStorage.DeleteFileAsync(videoId);
        }
    }
}
