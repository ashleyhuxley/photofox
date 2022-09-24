using PhotoFox.Storage.Table;
using System;
using System.Threading.Tasks;

namespace PhotoFox.CmdLine
{
    internal class Program
    {
        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        private static async Task MainAsync()
        {
            var config = new PhotoFoxConfig();
            var metaDataStorage = new PhotoMetadataStorage(config);
            var photoHashStorage = new PhotoHashStorage(config);

            await foreach (var photo in metaDataStorage.GetAllPhotos())
            {
                await photoHashStorage.AddHashAsync(photo.FileHash, photo.PartitionKey, photo.RowKey);
                Console.WriteLine("Added " + photo.RowKey);
            }
        }
    }
}
