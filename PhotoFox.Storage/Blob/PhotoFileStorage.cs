using System;
using System.Threading.Tasks;

namespace PhotoFox.Storage.Blob
{
    public class PhotoFileStorage : StorageBase, IPhotoFileStorage
    {
        private const string ThumbnailsContainer = "thumbnails";

        private const string PhotosContainer = "images";

        public PhotoFileStorage(IStorageConfig config)
            :base(config)
        {
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

        public async Task DeleteThumbnailAsync(string id)
        {
            await this.DeleteFileAsync(id, ThumbnailsContainer).ConfigureAwait(false);
        }

        public async Task DeletePhotoAsync(string id)
        {
            await this.DeleteFileAsync(id, PhotosContainer).ConfigureAwait(false);
        }
    }
}
