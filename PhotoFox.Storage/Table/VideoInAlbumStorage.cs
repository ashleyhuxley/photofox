using Azure.Data.Tables;
using Azure;
using PhotoFox.Storage.Models;
using System.Threading.Tasks;

namespace PhotoFox.Storage.Table
{
    public class VideoInAlbumStorage : IVideoInAlbumStorage
    {
        private readonly IStorageConfig config;

        private const string TableName = "VideoInAlbum";

        public VideoInAlbumStorage(IStorageConfig storageConfig)
        {
            this.config = storageConfig;
        }

        public AsyncPageable<VideoInAlbum> GetVideosInAlbumAsync(string albumId)
        {
            var client = new TableServiceClient(config.StorageConnectionString);
            var tableClient = client.GetTableClient(TableName);
            return tableClient.QueryAsync<VideoInAlbum>(p => p.PartitionKey == albumId);
        }

        public async Task AddVideoInAlbumAsync(VideoInAlbum videoInAlbum)
        {
            var client = new TableServiceClient(config.StorageConnectionString);
            var tableClient = client.GetTableClient(TableName);
            await tableClient.AddEntityAsync(videoInAlbum).ConfigureAwait(false);
        }

        public async Task ModifyVideoInAlbumAsync(VideoInAlbum videoInAlbum)
        {
            var client = new TableServiceClient(config.StorageConnectionString);
            var tableClient = client.GetTableClient(TableName);
            await tableClient.UpdateEntityAsync(videoInAlbum, ETag.All).ConfigureAwait(false);
        }
    }
}
