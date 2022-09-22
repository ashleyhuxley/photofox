using PhotoFox.Core.Extensions;
using PhotoFox.Model;
using PhotoFox.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
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

            var photos = new List<PhotoMetadata>();

            await foreach (var photo in metaDataStorage.GetAllPhotos())
            {
                photos.Add(photo);
            }

            var batch = 0;
            var i = 0;

            foreach (var photo in photos.OrderBy(p => p.UtcDate))
            {
                var batchPhoto = new PhotoInBatch
                {
                    PartitionKey = batch.ToBatchId(),
                    RowKey = photo.RowKey,
                    UtcDate = photo.UtcDate.Value
                };

                Console.WriteLine($"{batchPhoto.PartitionKey} - {batchPhoto.RowKey}");

                i++;
                if (i == 10)
                {
                    i = 0;
                    batch++;
                }
            }
        }
    }
}
