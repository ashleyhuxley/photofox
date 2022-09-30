using PhotoFox.Model;
using System;
using System.Collections.Generic;

namespace PhotoFox.Services
{
    public interface IPhotoService
    {
        IAsyncEnumerable<Photo> GetPhotosByDateTaken(DateTime dateTaken);
    }
}
