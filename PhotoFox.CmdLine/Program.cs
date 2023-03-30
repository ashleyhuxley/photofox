using PhotoFox.Core.Extensions;
using PhotoFox.Storage.Table;
using System;
using System.Threading.Tasks;

namespace PhotoFox.CmdLine
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        private static PhotoFoxConfig config = new PhotoFoxConfig();
        private static PhotoMetadataStorage metaDataStorage = new PhotoMetadataStorage(config);
        private static PhotoInAlbumStorage photoInAlbumStorage = new PhotoInAlbumStorage(config);

        private static async Task MainAsync()
        {
            await foreach (var photoInAlbum in photoInAlbumStorage.GetPhotosInAlbumAsync(Guid.Empty.ToString()))
            {
                try
                {
                    await metaDataStorage.GetPhotoMetadataAsync(photoInAlbum.UtcDate, photoInAlbum.RowKey);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{photoInAlbum.UtcDate.ToPartitionKey()} - {photoInAlbum.RowKey}");
                    Console.WriteLine(ex.Message);
                }
            }

            Console.WriteLine($"Done.");
        }
    }
}
