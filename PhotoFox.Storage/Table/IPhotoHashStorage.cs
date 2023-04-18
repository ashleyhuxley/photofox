using PhotoFox.Storage.Models;
using System.Threading.Tasks;

namespace PhotoFox.Storage.Table
{
    public interface IPhotoHashStorage
    {
        Task AddHashAsync(string hash, string partitionKey, string rowKey);
        Task<HashSearchResult> HashExistsAsync(string hash);
        Task DeleteHashAsync(string hash);
    }
}
