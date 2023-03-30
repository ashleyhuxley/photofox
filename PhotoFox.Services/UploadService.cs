using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using AutoMapper;
using NLog;
using PhotoFox.Core.Exif;
using PhotoFox.Core.Extensions;
using PhotoFox.Core.Hashing;
using PhotoFox.Core.Imaging;
using PhotoFox.Model;
using PhotoFox.Storage.Blob;
using PhotoFox.Storage.Models;
using PhotoFox.Storage.Table;

namespace PhotoFox.Services
{
    public class UploadService : IUploadService
    {
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        private readonly IPhotoMetadataStorage photoMetadataStorage;

        private readonly IThumbnailProvider thumbnailProvider;

        private readonly IPhotoInAlbumStorage photoInAlbumStorage;

        private readonly IStreamHash streamHash;

        private readonly IPhotoFileStorage photoFileStorage;

        private readonly IPhotoHashStorage photoHashStorage;

        private readonly IMapper mapper;

        public UploadService(
            IPhotoMetadataStorage photoMetadataStorage,
            IThumbnailProvider thumbnailProvider,
            IPhotoInAlbumStorage photoInAlbumStorage,
            IStreamHash streamHash,
            IPhotoFileStorage photoFileStorage,
            IPhotoHashStorage photoHashStorage,
            IMapper mapper)
        {
            this.photoMetadataStorage = photoMetadataStorage;
            this.thumbnailProvider = thumbnailProvider;
            this.photoInAlbumStorage = photoInAlbumStorage;
            this.streamHash = streamHash;
            this.photoFileStorage = photoFileStorage;
            this.photoHashStorage = photoHashStorage;
            this.mapper = mapper;
        }

        [SupportedOSPlatform("windows")]
        public async Task<Photo> UploadFromStreamAsync(Stream stream, DateTime fallbackTime, string fallbackTitle)
        {
            var image = Image.FromStream(stream);

            stream.Seek(0, SeekOrigin.Begin);
            var binaryData = await BinaryData.FromStreamAsync(stream).ConfigureAwait(false);

            var md5 = await Task.Run(() => this.streamHash.ComputeHash(stream));
            if (await photoHashStorage.HashExistsAsync(md5).ConfigureAwait(false) != null)
            {
                return null;
            }

            var exifReader = await ExifReader.FromStreamAsync(stream).ConfigureAwait(false);
            var metadata = new PhotoMetadata();
            metadata.Orientation = exifReader.GetOrientation();

            var thumbnail = await Task.Run(() => this.thumbnailProvider.GenerateThumbnail(image, 250, metadata.Orientation.ToRotationDegrees())).ConfigureAwait(false);
          
            metadata.FocalLength = exifReader.GetFocalLength();
            metadata.Device = exifReader.GetModel();
            metadata.Aperture = exifReader.GetApeture();
            metadata.Exposure = exifReader.GetExposure();
            metadata.GeolocationLattitude = exifReader.GetGpsLatitude();
            metadata.GeolocationLongitude = exifReader.GetGpsLongitude();
            metadata.ISO = exifReader.GetIso();
            metadata.Title = fallbackTitle;
            metadata.DimensionWidth = exifReader.GetDimensionWidth() ?? image.Width;
            metadata.DimensionHeight = exifReader.GetDimensionHeight() ?? image.Height;
            metadata.Manufacturer = exifReader.GetManufacturer();
            metadata.FileSize = binaryData.ToArray().Length;

            var date = exifReader.GetDateTakenUtc() ?? fallbackTime;
            metadata.PartitionKey = date.ToPartitionKey();
            metadata.UtcDate = date;

            metadata.RowKey = Guid.NewGuid().ToString();
            metadata.FileHash = md5;

            Log.Trace($"PK: {metadata.PartitionKey}, RK: {metadata.RowKey}");

            using (var ms = new MemoryStream())
            {
                thumbnail.Save(ms, ImageFormat.Jpeg);

                ms.Seek(0, SeekOrigin.Begin);
                var data = await BinaryData.FromStreamAsync(ms).ConfigureAwait(false);
                await this.photoFileStorage.PutThumbnailAsync(metadata.RowKey, data).ConfigureAwait(false);
            }

            await this.photoFileStorage.PutPhotoAsync(metadata.RowKey, binaryData).ConfigureAwait(false);

            await this.photoMetadataStorage.AddPhotoAsync(metadata).ConfigureAwait(false);
            await this.photoHashStorage.AddHashAsync(md5, metadata.PartitionKey, metadata.RowKey).ConfigureAwait(false);
            await this.photoInAlbumStorage.AddPhotoInAlbumAsync(Guid.Empty.ToString(), metadata.RowKey, metadata.UtcDate.Value);

            return mapper.Map<Photo>(metadata);
        }
    }
}
