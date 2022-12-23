using PhotoFox.Core.Exif;
using PhotoFox.Core.Extensions;
using PhotoFox.Core.Hashing;
using PhotoFox.Core.Imaging;
using PhotoFox.Storage.Blob;
using PhotoFox.Storage.Models;
using PhotoFox.Storage.Table;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

namespace PhotoFox.CmdLine
{
    internal class Program
    {
        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        private static PhotoFoxConfig config = new PhotoFoxConfig();
        private static PhotoMetadataStorage metaDataStorage = new PhotoMetadataStorage(config);
        private static PhotoHashStorage hashStorage = new PhotoHashStorage(config);
        private static PhotoFileStorage fileStore = new PhotoFileStorage(config);
        private static StreamHashMD5 hasher = new StreamHashMD5();
        private static PhotoAlbumDataStorage albumDataStorage = new PhotoAlbumDataStorage(config);
        private static ThumbnailProvider thumbnailProvider = new ThumbnailProvider();

        private static async Task MainAsync()
        {
            await foreach (var photo in metaDataStorage.GetAllPhotos())
            {
                try
                {
                    Console.WriteLine($"Processing {photo.RowKey}");

                    var file = await fileStore.GetPhotoAsync(photo.RowKey);

                    var image = Image.FromStream(file.ToStream());

                    var exifReader = await ExifReader.FromStreamAsync(file.ToStream());
                    var newThumb = thumbnailProvider.GenerateThumbnail(image, 250);

                    await fileStore.DeleteThumbnailAsync(photo.RowKey);

                    using (var ms = new MemoryStream())
                    {
                        newThumb.Save(ms, ImageFormat.Jpeg);

                        ms.Seek(0, SeekOrigin.Begin);
                        var data = await BinaryData.FromStreamAsync(ms);
                        await fileStore.PutThumbnailAsync(photo.RowKey, data);
                    }

                    photo.FocalLength = exifReader.GetFocalLength();
                    photo.Device = exifReader.GetModel();
                    photo.Aperture = exifReader.GetApeture();
                    photo.Exposure = exifReader.GetExposure();
                    photo.Orientation = exifReader.GetOrientation();
                    photo.GeolocationLattitude = exifReader.GetGpsLatitude();
                    photo.GeolocationLongitude = exifReader.GetGpsLongitude();
                    photo.ISO = exifReader.GetIso();
                    photo.DimensionWidth = exifReader.GetDimensionWidth() ?? image.Width;
                    photo.DimensionHeight = exifReader.GetDimensionHeight() ?? image.Height;
                    photo.Manufacturer = exifReader.GetManufacturer();
                    await metaDataStorage.SavePhotoAsync(photo);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Photo {photo.RowKey} could not process. {ex.Message}");
                }
            }

            Console.WriteLine("Done.");
            Console.ReadLine();
        }

        private static async Task MainAsync2()
        {
            await foreach (var photo in metaDataStorage.GetAllPhotos())
            {
                await Process(photo, null);
            }

            Console.WriteLine("Done.");
            Console.ReadLine();
        }

        private static async Task Process(PhotoMetadata photo, ParallelLoopState state)
        {
            var file = await fileStore.GetPhotoAsync(photo.RowKey);
            var hash = hasher.ComputeHash(file.ToStream());
            var len = file.ToArray().Length;

            if (hash != photo.FileHash || !photo.FileSize.HasValue || photo.FileSize.Value != len)
            {
                photo.FileHash = hash;
                photo.FileSize = len;
                await metaDataStorage.SavePhotoAsync(photo);
            }

            try
            {
                await hashStorage.AddHashAsync(hash, photo.PartitionKey, photo.RowKey);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Duplicate hash found for photo ID " + photo.RowKey);
            }
        }
    }
}
