using Azure.Data.Tables;
using PhotoFox.Storage.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhotoFox.Storage.Table
{
    public class UserStorage : IUserStorage
    {
        private const string TableName = "v7AspNetUsers";

        private readonly IStorageConfig config;

        public UserStorage(IStorageConfig storageConfig)
        {
            config = storageConfig;
        }

        public IAsyncEnumerable<User> GetUsersAsync()
        {
            var client = new TableServiceClient(config.StorageConnectionString);
            var tableClient = client.GetTableClient(TableName);
            return tableClient.QueryAsync<User>();
        }
    }
}
