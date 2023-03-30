using System;
using System.Threading.Tasks;

namespace PhotoFox.Storage.Table
{
    public interface IPhotoHashStorage
    {
        Task AddHashAsync(string hash, string partitionKey, string rowKey);
        Task<Tuple<string, string>> HashExistsAsync(string hash);
        Task DeleteHashAsync(string hash);
    }
}
