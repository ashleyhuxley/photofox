using PhotoFox.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhotoFox.Services
{
    public interface IPhotoService
    {
        IAsyncEnumerable<Photo> GetPhotosByDateTaken(DateTime dateTaken);

        Task<Photo> GetPhotoAsync(DateTime dateTaken, string photoId);

        Task SavePhotoAsync(Photo photo);

        Task DeletePhotoAsync(Photo photo);
    }
}
