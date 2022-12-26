using PhotoFox.Core.Exif;
using PhotoFox.Core.Extensions;
using PhotoFox.Core.Hashing;
using PhotoFox.Core.Imaging;
using PhotoFox.Model;
using PhotoFox.Storage.Blob;
using PhotoFox.Storage.Models;
using PhotoFox.Storage.Table;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

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
        private static PhotoInAlbumStorage photoInAlbumStorage = new PhotoInAlbumStorage(config);

        private static async Task MainAsync()
        {
            await foreach (var photo in metaDataStorage.GetAllPhotosAsync())
            {
                if (!await photoInAlbumStorage.IsPhotoInAnAlbumAsync(photo.RowKey))
                {
                    Console.WriteLine($"Processing {photo.RowKey}");
                    await photoInAlbumStorage.AddPhotoInAlbumAsync(Guid.Empty.ToString(), photo.RowKey, photo.UtcDate.Value);
                }
            }

            Console.WriteLine($"Done.");
        }

        private static async Task RegenerateThumbnails()
        {
            await foreach (var photo in metaDataStorage.GetAllPhotosAsync())
            {
                try
                {
                    if (photo.Orientation.HasValue && photo.Orientation != 1)
                    {
                        Console.WriteLine($"Processing {photo.RowKey}");
                        var file = await fileStore.GetPhotoAsync(photo.RowKey);
                        var image = Image.FromStream(file.ToStream());

                        var thumb = thumbnailProvider.GenerateThumbnail(image, 250, photo.Orientation.ToRotationDegrees());

                        await fileStore.DeleteThumbnailAsync(photo.RowKey);

                        using (var ms = new MemoryStream())
                        {
                            thumb.Save(ms, ImageFormat.Jpeg);

                            ms.Seek(0, SeekOrigin.Begin);
                            var data = await BinaryData.FromStreamAsync(ms);
                            await fileStore.PutThumbnailAsync(photo.RowKey, data);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Photo {photo.RowKey} could not process. {ex.Message}");
                }
            }

            Console.WriteLine("Done.");
            Console.ReadLine();
        }

        private static async Task RegenerateHashes()
        {
            await foreach (var photo in metaDataStorage.GetAllPhotosAsync())
            {
                await RegenerateHash(photo, null);
            }

            Console.WriteLine("Done.");
            Console.ReadLine();
        }

        private static async Task RegenerateHash(PhotoMetadata photo, ParallelLoopState state)
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
