using Azure.Data.Tables;
using PhotoFox.Core.Extensions;
using PhotoFox.Storage.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhotoFox.Storage.Table
{
    public class AlbumPermissionStorage : IAlbumPermissionStorage
    {
        private const string TableName = "AlbumPermissions";

        private readonly IStorageConfig config;

        public AlbumPermissionStorage(IStorageConfig config)
        {
            this.config = config;
        }

        public async Task<bool> HasPermissionAsync(string albumId, string username)
        {
            var client = new TableServiceClient(config.StorageConnectionString);
            var tableClient = client.GetTableClient(TableName);
            return await tableClient.EntityExistsAsync<AlbumPermission>(username, albumId).ConfigureAwait(false);
        }

        public IAsyncEnumerable<AlbumPermission> GetPermissionsByUsernameAsync(string username)
        {
            var client = new TableServiceClient(config.StorageConnectionString);
            var tableClient = client.GetTableClient(TableName);
            return tableClient.QueryAsync<AlbumPermission>(p => p.PartitionKey == username);
        }

        public async Task AddPermissionAsync(string albumId, string username)
        {
            var client = new TableServiceClient(config.StorageConnectionString);
            var tableClient = client.GetTableClient(TableName);
            await tableClient.AddEntityAsync<AlbumPermission>(new AlbumPermission { PartitionKey = username, RowKey = albumId }).ConfigureAwait(false);
        }

        public async Task RemovePermissionAsync(string albumId, string username)
        {
            var client = new TableServiceClient(config.StorageConnectionString);
            var tableClient = client.GetTableClient(TableName);
            await tableClient.DeleteEntityAsync(username, albumId).ConfigureAwait(false);
        }
    }
}
