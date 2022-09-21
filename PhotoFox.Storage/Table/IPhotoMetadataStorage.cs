using Azure;
using PhotoFox.Model;
using System;
using System.Threading.Tasks;

namespace PhotoFox.Storage.Table
{
    public interface IPhotoMetadataStorage
    {
        Task<PhotoMetadata> GetPhotoMetadata(DateTime utcDate, string photoId);

        AsyncPageable<PhotoMetadata> GetAllPhotos();

        AsyncPageable<PhotoMetadata> GetPhotosByDate(DateTime date);

        Task AddPhotoAsync(PhotoMetadata photo);
    }
}
