using Azure.Storage.Blobs;
using System;
using System.Threading.Tasks;

namespace PhotoFox.Storage.Blob
{
    public class PhotoFileStorage : IPhotoFileStorage
    {
        private readonly IStorageConfig config;

        private const string ThumbnailsContainer = "thumbnails";

        private const string PhotosContainer = "images";

        public PhotoFileStorage(IStorageConfig config)
        {
            this.config = config;
        }

        public async Task<BinaryData> GetThumbnailAsync(string id)
        {
            return await GetFileAsync(id, ThumbnailsContainer).ConfigureAwait(false);
        }

        public async Task<BinaryData> GetPhotoAsync(string id)
        {
            return await GetFileAsync(id, PhotosContainer).ConfigureAwait(false);
        }

        public async Task PutThumbnailAsync(string id, BinaryData data)
        {
            await PutFileAsync(id, data, ThumbnailsContainer).ConfigureAwait(false);
        }

        public async Task PutPhotoAsync(string id, BinaryData data)
        {
            await PutFileAsync(id, data, PhotosContainer).ConfigureAwait(false);
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

            var res = await blob.DownloadContentAsync().ConfigureAwait(false);
            return res.Value.Content;
        }

        private async Task PutFileAsync(string id, BinaryData data, string containerName)
        {
            var client = new BlobServiceClient(this.config.StorageConnectionString);
            var container = client.GetBlobContainerClient(containerName);

            await container.UploadBlobAsync(id, data).ConfigureAwait(false);
        }

        public async Task DeleteThumbnailAsync(string id)
        {
            await this.DeleteFileAsync(id, ThumbnailsContainer).ConfigureAwait(false);
        }

        public async Task DeletePhotoAsync(string id)
        {
            await this.DeleteFileAsync(id, PhotosContainer).ConfigureAwait(false);
        }

        private async Task DeleteFileAsync(string id, string containerName)
        {
            var client = new BlobServiceClient(this.config.StorageConnectionString);
            var container = client.GetBlobContainerClient(containerName);

            await container.DeleteBlobIfExistsAsync(id).ConfigureAwait(false);
        }
    }
}
