using System.Threading.Tasks;
using System;

namespace PhotoFox.Storage.Blob
{
    public interface IUploadStorage
    {
        Task<BinaryData> GetFileAsync(string id);
        Task DeleteFileAsync(string id);
        Task PutFileAsync(string id, BinaryData data);
    }
}
