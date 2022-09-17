using Azure.Storage.Blobs;
using System;
using System.Threading.Tasks;

namespace PhotoFox.Storage.Blob
{
    public class PhotoFileStorage : IPhotoFileStorage
    {
        private readonly IStorageConfig config;

        public PhotoFileStorage(IStorageConfig config)
        {
            this.config = config;
        }

        public async Task<BinaryData> GetFileAsync(string id)
        {
            var client = new BlobServiceClient(this.config.StorageConnectionString);
            var container = client.GetBlobContainerClient("thumbnails");

            var blob = container.GetBlobClient(id);

            var res = await blob.DownloadContentAsync();
            return res.Value.Content;
        }
    }
}
