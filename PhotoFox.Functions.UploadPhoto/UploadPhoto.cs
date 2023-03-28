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

namespace PhotoFox.Functions.UploadPhoto
{
    public class UploadPhoto
    {
        private IPhotoFileStorage photoFileStorage;
        private IStreamHash streamHash;
        private IPhotoHashStorage photoHashStorage;
        private IThumbnailProvider thumbnailProvider;
        private IPhotoMetadataStorage photoMetadataStorage;
        private IPhotoInAlbumStorage photoInAlbumStorage;

        public UploadPhoto(
            IPhotoFileStorage photoFileStorage,
            IStreamHash streamHash,
            IPhotoHashStorage photoHashStorage,
            IThumbnailProvider thumbnailProvider,
            IPhotoMetadataStorage photoMetadataStorage,
            IPhotoInAlbumStorage photoInAlbumStorage)
        {
            this.photoFileStorage = photoFileStorage;
            this.streamHash = streamHash;
            this.photoHashStorage = photoHashStorage;
            this.thumbnailProvider = thumbnailProvider;
            this.photoMetadataStorage = photoMetadataStorage;
            this.photoInAlbumStorage= photoInAlbumStorage;
        }

        [FunctionName("Upload")]
        public async Task Run([QueueTrigger("uploads", Connection = "AzureWebJobsStorage")]string photoId, ILogger log)
        {
            log.LogInformation($"Processing image ID: {photoId}");

            // Get the photo from storage
            var blob = await photoFileStorage.GetPhotoAsync(photoId);
            if (blob == null )
            {
                log.LogError($"Image not found in blob storage: {photoId}");
                return;
            }

            var stream = blob.ToStream();
            var image = Image.FromStream(stream);

            // Check hash
            var md5 = await Task.Run(() => streamHash.ComputeHash(stream));
            if (await photoHashStorage.HashExistsAsync(md5).ConfigureAwait(false) != null)
            {
                log.LogWarning($"An image with the same hash as {photoId} already exists");
                await photoFileStorage.DeletePhotoAsync(photoId);
                return;
            }

            // Get EXIF data
            var exifReader = await ExifReader.FromStreamAsync(stream).ConfigureAwait(false);
            var metadata = new PhotoMetadata();
            metadata.Orientation = exifReader.GetOrientation();

            var thumbnail = await Task.Run(() => thumbnailProvider.GenerateThumbnail(image, 250, metadata.Orientation.ToRotationDegrees())).ConfigureAwait(false);

            metadata.FocalLength = exifReader.GetFocalLength();
            metadata.Device = exifReader.GetModel();
            metadata.Aperture = exifReader.GetApeture();
            metadata.Exposure = exifReader.GetExposure();
            metadata.GeolocationLattitude = exifReader.GetGpsLatitude();
            metadata.GeolocationLongitude = exifReader.GetGpsLongitude();
            metadata.ISO = exifReader.GetIso();
            metadata.Title = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
            metadata.DimensionWidth = exifReader.GetDimensionWidth() ?? image.Width;
            metadata.DimensionHeight = exifReader.GetDimensionHeight() ?? image.Height;
            metadata.Manufacturer = exifReader.GetManufacturer();
            metadata.FileSize = blob.ToArray().Length;

            var date = exifReader.GetDateTakenUtc() ?? DateTime.UtcNow;
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
            await photoHashStorage.AddHashAsync(md5, metadata.PartitionKey, metadata.RowKey);
            await photoInAlbumStorage.AddPhotoInAlbumAsync(Guid.Empty.ToString(), metadata.RowKey, metadata.UtcDate.Value);

        }
    }
}
