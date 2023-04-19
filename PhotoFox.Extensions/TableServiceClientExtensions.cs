using Azure.Data.Tables;

namespace PhotoFox.Extensions
{
    public static class TableClientExtensions
    {
        public static async Task<bool> EntityExistsAsync<T>(this TableClient client, string partitionKey, string rowKey)
            where T : class, ITableEntity, new()
        {
            var result = client.QueryAsync<T>($"PartitionKey eq '{partitionKey}' and RowKey eq '{rowKey}'");

            var i = 0;
            await foreach (var res in result)
            {
                i++;
            }

            return i > 0;
        }
    }
}
