using Azure.Storage.Blobs;
using System;
using System.Threading.Tasks;

namespace PhotoFox.Storage.Blob
{
    public class PhotoFileStorage : IPhotoFileStorage
    {
        private readonly IStorageConfig config;

        private const string ThumbnailsContainer = "thumbnails";

        private const string PhotosContainer = "photos";

        public PhotoFileStorage(IStorageConfig config)
        {
            this.config = config;
        }

        public async Task<BinaryData> GetThumbnailAsync(string id)
        {
            return await GetFileAsync(id, ThumbnailsContainer);
        }

        public async Task<BinaryData> GetPhotoAsync(string id)
        {
            return await GetFileAsync(id, PhotosContainer);
        }

        public async Task PutThumbnailAsync(string id, BinaryData data)
        {
            await PutFileAsync(id, data, ThumbnailsContainer);
        }

        public async Task PutPhotoAsync(string id, BinaryData data)
        {
            await PutFileAsync(id, data, PhotosContainer);
        }

        private async Task<BinaryData> GetFileAsync(string id, string containerName)
        {
            var client = new BlobServiceClient(this.config.StorageConnectionString);
            var container = client.GetBlobContainerClient(containerName);

            var blob = container.GetBlobClient(id);

            if (!blob.Exists())
            {
                return null;
            }

            var res = await blob.DownloadContentAsync();
            return res.Value.Content;
        }

        private async Task PutFileAsync(string id, BinaryData data, string containerName)
        {
            var client = new BlobServiceClient(this.config.StorageConnectionString);
            var container = client.GetBlobContainerClient(containerName);
        }
    }
}
