using System.Threading.Tasks;
using System;

namespace PhotoFox.Storage.Blob
{
    public interface IPhotoFileStorage
    {
        Task<BinaryData> GetFileAsync(string id);
    }
}
