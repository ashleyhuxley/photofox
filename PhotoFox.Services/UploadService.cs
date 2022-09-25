using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using NLog;
using PhotoFox.Core.Exif;
using PhotoFox.Core.Extensions;
using PhotoFox.Core.Hashing;
using PhotoFox.Core.Imaging;
using PhotoFox.Model;
using PhotoFox.Storage.Blob;
using PhotoFox.Storage.Table;

namespace PhotoFox.Services
{
    public class UploadService : IUploadService
    {
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        private readonly IPhotoMetadataStorage photoMetadataStorage;

        private readonly IThumbnailProvider thumbnailProvider;

        private readonly IStreamHash streamHash;

        private readonly IPhotoFileStorage photoFileStorage;

        private readonly IPhotoHashStorage photoHashStorage;

        public UploadService(
            IPhotoMetadataStorage photoMetadataStorage,
            IThumbnailProvider thumbnailProvider,
            IStreamHash streamHash,
            IPhotoFileStorage photoFileStorage,
            IPhotoHashStorage photoHashStorage)
        {
            this.photoMetadataStorage = photoMetadataStorage;
            this.thumbnailProvider = thumbnailProvider;
            this.streamHash = streamHash;
            this.photoFileStorage = photoFileStorage;
            this.photoHashStorage = photoHashStorage;
        }

        public async Task<PhotoMetadata> UploadFromStreamAsync(Stream stream, DateTime fallbackTime, string fallbackTitle)
        {
            var image = Image.FromStream(stream);
            var binaryData = await BinaryData.FromStreamAsync(stream);

            var md5 = await Task.Run(() => this.streamHash.ComputeHash(stream));
            if (await photoHashStorage.HashExistsAsync(md5) != null)
            {
                return null;
            }

            var thumbnail = await Task.Run(() => this.thumbnailProvider.GenerateThumbnail(image, 250));

            var metatdata = new PhotoMetadata();

            var exifReader = await ExifReader.FromStreamAsync(stream);
            metatdata.FocalLength = exifReader.GetFocalLength();
            metatdata.Device = exifReader.GetModel();
            metatdata.Aperture = exifReader.GetApeture();
            metatdata.Exposure = exifReader.GetExposure();
            metatdata.Orientation = exifReader.GetOrientation();
            metatdata.GeolocationLattitude = exifReader.GetGpsLatitude();
            metatdata.GeolocationLongitude = exifReader.GetGpsLongitude();
            metatdata.Iso = exifReader.GetIso();
            metatdata.Title = fallbackTitle;

            var date = exifReader.GetDateTakenUtc() ?? fallbackTime;
            metatdata.PartitionKey = date.ToPartitionKey();
            metatdata.UtcDate = date;

            metatdata.RowKey = Guid.NewGuid().ToString();
            metatdata.FileHash = md5;

            Log.Trace($"PK: {metatdata.PartitionKey}, RK: {metatdata.RowKey}");

            using (var ms = new MemoryStream())
            {
                thumbnail.Save(ms, ImageFormat.Jpeg);

                ms.Seek(0, SeekOrigin.Begin);
                var data = await BinaryData.FromStreamAsync(ms);
                await this.photoFileStorage.PutThumbnailAsync(metatdata.RowKey, data);
            }

            await this.photoFileStorage.PutPhotoAsync(metatdata.RowKey, binaryData);

            await this.photoMetadataStorage.AddPhotoAsync(metatdata);
            await this.photoHashStorage.AddHashAsync(md5, metatdata.PartitionKey, metatdata.RowKey);

            return metatdata;
        }
    }
}
