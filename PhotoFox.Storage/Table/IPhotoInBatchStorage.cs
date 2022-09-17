using Azure;
using PhotoFox.Model;

namespace PhotoFox.Storage.Table
{
    public interface IPhotoInBatchStorage
    {
        AsyncPageable<PhotoInBatch> GetPhotosInBatch(int batchId);
    }
}
