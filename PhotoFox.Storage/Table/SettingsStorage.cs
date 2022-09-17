using Azure.Data.Tables;
using PhotoFox.Model;
using System.Threading.Tasks;

namespace PhotoFox.Storage.Table
{
    public class SettingsStorage : ISettingsStorage
    {
        private const string TableName = "settings";

        private readonly IStorageConfig config;

        public SettingsStorage(IStorageConfig storageConfig)
        {
            this.config = storageConfig;
        }

        public async Task<string> GetSetting(string name)
        {
            var client = new TableServiceClient(config.StorageConnectionString);
            var tableClient = client.GetTableClient(TableName);
            var result = await tableClient.GetEntityAsync<Setting>("setting", name);
            return result.Value.Value;
        }
    }
}
