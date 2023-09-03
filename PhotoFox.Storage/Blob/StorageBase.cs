using Azure.Storage.Blobs;
using System.Threading.Tasks;
using System;
using PhotoFox.Core.Exceptions;
using Azure.Storage.Blobs.Models;

namespace PhotoFox.Storage.Blob
{
    public class StorageBase
    {
        private readonly IStorageConfig storageConfig;

        public StorageBase(IStorageConfig storageConfig) 
        {
            this.storageConfig = storageConfig;
        }

        protected async Task<BinaryData> GetFileAsync(string id, string containerName)
        {
            var client = new BlobServiceClient(this.storageConfig.StorageConnectionString);
            var container = client.GetBlobContainerClient(containerName);

            var blob = container.GetBlobClient(id);

            if (!blob.Exists())
            {
                throw new EntityNotFoundException(containerName, id);
            }

            var res = await blob.DownloadContentAsync().ConfigureAwait(false);
            return res.Value.Content;
        }

        protected async Task DeleteFileAsync(string id, string containerName)
        {
            var client = new BlobServiceClient(this.storageConfig.StorageConnectionString);
            var container = client.GetBlobContainerClient(containerName);

            await container.DeleteBlobIfExistsAsync(id).ConfigureAwait(false);
        }

        protected async Task PutFileAsync(string id, BinaryData data, string containerName)
        {
            var client = new BlobServiceClient(this.storageConfig.StorageConnectionString);
            var container = client.GetBlobContainerClient(containerName);

            await container.UploadBlobAsync(id, data).ConfigureAwait(false);
        }

        internal async Task<string> GetContentTypeAsync(string photoId, string containerName)
        {
            var client = new BlobServiceClient(this.storageConfig.StorageConnectionString);
            var container = client.GetBlobContainerClient(containerName);

            var blob = container.GetBlobClient(photoId);

            if (!blob.Exists())
            {
                throw new EntityNotFoundException(containerName, photoId);
            }

            BlobClient blobClient = container.GetBlobClient(blob.Name);

            BlobProperties blobProperties = await blobClient.GetPropertiesAsync()
                .ConfigureAwait(false);
            return blobProperties.ContentType;
        }

        internal async Task SetContentTypeAsync(string photoId, string contentType, string containerName)
        {
            var client = new BlobServiceClient(this.storageConfig.StorageConnectionString);
            var container = client.GetBlobContainerClient(containerName);

            var blob = container.GetBlobClient(photoId);

            if (!blob.Exists())
            {
                throw new EntityNotFoundException(containerName, photoId);
            }

            BlobClient blobClient = container.GetBlobClient(blob.Name);

            BlobHttpHeaders blobHttpHeaders = new BlobHttpHeaders
            {
                ContentType = contentType
            };

            await blobClient.SetHttpHeadersAsync(blobHttpHeaders)
                .ConfigureAwait(false);
        }
    }
}
