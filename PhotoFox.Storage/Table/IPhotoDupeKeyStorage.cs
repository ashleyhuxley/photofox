using System.Threading.Tasks;
using System;

namespace PhotoFox.Storage.Table
{
    public interface IPhotoDupeKeyStorage
    {
        Task AddKeyAsync(DateTime utcDate, long fileSize, string partitionKey, string rowKey);
        Task DeleteKeyAsync(DateTime utcDate, long fileSize);
        Task<Tuple<string, string>?> KeyExistsAsync(DateTime utcDate, long fileSize, string partitionKey);
    }
}
