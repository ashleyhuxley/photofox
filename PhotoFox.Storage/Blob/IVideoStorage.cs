using System.Threading.Tasks;
using System;

namespace PhotoFox.Storage.Blob
{
    public interface IVideoStorage
    {
        Task<BinaryData> GetVideoAsync(string id);
        Task PutVideoAsync(string id, BinaryData data);
        Task DeleteVideoAsync(string id);
    }
}
