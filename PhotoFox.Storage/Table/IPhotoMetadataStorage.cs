using Azure;
using PhotoFox.Storage.Models;
using System;
using System.Threading.Tasks;

namespace PhotoFox.Storage.Table
{
    public interface IPhotoMetadataStorage
    {
        Task<PhotoMetadata> GetPhotoMetadataAsync(DateTime utcDate, string photoId);
        AsyncPageable<PhotoMetadata> GetAllPhotosAsync();
        AsyncPageable<PhotoMetadata> GetPhotosByDateAsync(DateTime date);
        Task AddPhotoAsync(PhotoMetadata photo);
        Task DeletePhotoAsync(string partitionKey, string rowKey);
        Task SavePhotoAsync(PhotoMetadata metadata);
    }
}
