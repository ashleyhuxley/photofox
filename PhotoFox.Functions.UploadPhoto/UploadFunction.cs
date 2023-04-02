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
            IVideoStorage videoStorage)
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
        }

        [FunctionName("Upload")]
        public async Task Run([QueueTrigger("uploads", Connection = "PhotoFoxStorage")]string message, ILogger log)
        {
            var uploadMessage = JsonSerializer.Deserialize<UploadMessage>(message);
            if (uploadMessage == null)
            {
                throw new ArgumentException("The input message could not be deserialized", nameof(message));
            }

            var albumId = uploadMessage.Album ?? Guid.Empty.ToString();
            var title = uploadMessage.Title ?? uploadMessage.DateTaken.ToString("yyyy-MM-dd HH:mm:ss");

            log.LogInformation($"Processing entity ID: {uploadMessage.EntityId} into album {uploadMessage.Album} as {uploadMessage.Type}");

            switch (uploadMessage.Type.ToUpperInvariant())
            {
                case "PHOTO":
                    await ProcessPhoto(uploadMessage.EntityId, albumId, title, uploadMessage.DateTaken, log);
                    break;
                case "VIDEO":
                    await ProcessVideo(uploadMessage.EntityId, albumId, title, uploadMessage.DateTaken, log);
                    break;
                default:
                    throw new ArgumentException($"Unknown Message Type: {uploadMessage.Type}");
            }
        }

        private async Task ProcessPhoto(string photoId, string albumId, string title, DateTime dateTaken, ILogger log)
        {
            // Get the photo from storage
            var blob = await photoFileStorage.GetPhotoAsync(photoId);
            if (blob == null)
            {
                var errorMessage = $"Image not found in blob storage: {photoId}";
                log.LogError(errorMessage);
                throw new FileNotFoundException(errorMessage);
            }

            var stream = blob.ToStream();

            // Check hash
            var md5 = await Task.Run(() => streamHash.ComputeHash(stream));
            if (await photoHashStorage.HashExistsAsync(md5).ConfigureAwait(false) != null)
            {
                log.LogWarning($"An image with the same hash as {photoId} already exists");

                await logStorage.Log("An image with this hash already exists", source, photoId, albumId, LogLevel.Warn, md5);

                await photoFileStorage.DeletePhotoAsync(photoId);
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

            // Store table items
            await photoMetadataStorage.AddPhotoAsync(metadata);
            await logStorage.Log("Metadata entry added", source, photoId, "", LogLevel.Info, md5);

            await photoHashStorage.AddHashAsync(md5, metadata.PartitionKey, metadata.RowKey);
            await logStorage.Log("Photo hash added", source, photoId, "", LogLevel.Info, md5);

            await photoInAlbumStorage.AddPhotoInAlbumAsync(albumId, metadata.RowKey, metadata.UtcDate.Value);
            await logStorage.Log("Photo added to album", source, photoId, albumId, LogLevel.Info, md5);
        }

        private async Task ProcessVideo(string videoId, string albumId, string title, DateTime dateTaken, ILogger log)
        {
            // Get the photo from storage
            var blob = await videoStorage.GetVideoAsync(videoId);
            if (blob == null)
            {
                var errorMessage = $"Video not found in blob storage: {videoId}";
                log.LogError(errorMessage);
                throw new FileNotFoundException(errorMessage);
            }

            var videoInAlbum = new VideoInAlbum
            {
                FileSize = blob.ToArray().Length,
                PartitionKey = albumId,
                RowKey = videoId,
                Title = title,
                VideoDate = dateTaken
            };

            await videoInAlbumStorage.AddVideoInAlbumAsync(videoInAlbum);
        }
    }
}
