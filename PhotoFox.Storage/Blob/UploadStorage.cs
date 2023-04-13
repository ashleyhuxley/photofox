using System.Threading.Tasks;
using System;

namespace PhotoFox.Storage.Blob
{
    public class UploadStorage : StorageBase, IUploadStorage
    {
        private const string ContainerName = "uploads";

        public UploadStorage(IStorageConfig config)
            : base(config)
        {
        }

        public async Task DeleteFileAsync(string id)
        {
            await DeleteFileAsync(id, ContainerName);
        }

        public async Task<BinaryData> GetFileAsync(string id)
        {
            return await GetFileAsync(id, ContainerName).ConfigureAwait(false);
        }

        public async Task PutFileAsync(string id, BinaryData data)
        {
            await PutFileAsync(id, data, ContainerName);
        }
    }
}
