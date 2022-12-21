using PhotoFox.Core.Extensions;
using PhotoFox.Core.Hashing;
using PhotoFox.Storage.Blob;
using PhotoFox.Storage.Models;
using PhotoFox.Storage.Table;
using System;
using System.IO;
using System.Runtime.CompilerServices;
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

        private static async Task MainAsync()
        {
            await foreach (var album in albumDataStorage.GetPhotoAlbums())
            {
                var newALbum = new PhotoAlbum
                {
                    PartitionKey = album.RowKey,
                    RowKey = string.Empty,
                    AlbumDescription = album.AlbumDescription,
                    AlbumName = album.AlbumName,
                    CoverPhotoId = album.CoverPhotoId
                };

                await albumDataStorage.AddPhotoAlbum(newALbum);
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
