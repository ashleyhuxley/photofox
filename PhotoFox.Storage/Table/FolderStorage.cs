using Azure.Data.Tables;
using PhotoFox.Storage.Models;
using System.Collections.Generic;

namespace PhotoFox.Storage.Table
{
    public class FolderStorage : IFolderStorage
    {
        private const string TableName = "Folders";

        private readonly IStorageConfig config;

        public FolderStorage(IStorageConfig config)
        {
            this.config = config;
        }

        public IAsyncEnumerable<Folder> GetFoldersAsync()
        {
            var client = new TableServiceClient(config.StorageConnectionString);
            var tableClient = client.GetTableClient(TableName);
            return tableClient.QueryAsync<Folder>();
        }
    }
}
