using System.Threading.Tasks;
using System;

namespace PhotoFox.Storage.Blob
{
    public class VideoStorage : StorageBase, IVideoStorage
    {
        private const string VideosContainer = "videos";

        public VideoStorage(IStorageConfig config)
            :base(config)
        {
        }

        public async Task<BinaryData> GetVideoAsync(string id)
        {
            return await GetFileAsync(id, VideosContainer).ConfigureAwait(false);
        }

        public async Task PutVideoAsync(string id, BinaryData data)
        {
            await PutFileAsync(id, data, VideosContainer).ConfigureAwait(false);
        }

        public async Task DeleteVideoAsync(string id)
        {
            await this.DeleteFileAsync(id, VideosContainer).ConfigureAwait(false);
        }
    }
}
