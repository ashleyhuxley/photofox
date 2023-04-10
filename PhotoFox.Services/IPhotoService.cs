﻿using PhotoFox.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhotoFox.Services
{
    public interface IPhotoService
    {
        Task<Photo> GetPhotoAsync(DateTime dateTaken, string photoId);
        
        Task<Photo> GetPhotoAsync(string dateTaken, string photoId);

        Task SavePhotoAsync(Photo photo);

        Task DeletePhotoAsync(Photo photo);

        IAsyncEnumerable<Photo> GetPhotosInAlbumAsync(string albumId);

        Task<Photo> ReloadExifDataAsync(DateTime utcDate, string photoId);
    }
}
