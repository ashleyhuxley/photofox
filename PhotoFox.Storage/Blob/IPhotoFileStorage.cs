using System.Threading.Tasks;
using System;

namespace PhotoFox.Storage.Blob
{
    public interface IPhotoFileStorage
    {
        Task<BinaryData> GetThumbnailAsync(string id);
        Task<BinaryData> GetPhotoAsync(string id);
        Task PutThumbnailAsync(string id, BinaryData data);
        Task PutPhotoAsync(string id, BinaryData data);
        Task DeleteThumbnailAsync(string id);
        Task DeletePhotoAsync(string id);
    }
}
